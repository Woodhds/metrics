using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Base.Contracts;
using Base.Contracts.Models;
using Base.Contracts.Options;
using HtmlAgilityPack;
using metrics.Competitions.Abstractions;
using metrics.Services.Abstractions.VK;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace metrics.Competitions.Hosted.Services
{
    public class CompetitionsService : ICompetitionsService
    {
        private readonly IVkWallService _vkClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CompetitionOptions _competitionOptions;
        private const int Take = 10;
        private readonly ILogger<CompetitionsService> _logger;

        public CompetitionsService(
            IVkWallService vkClient,
            IHttpClientFactory httpClientFactory,
            IOptionsMonitor<CompetitionOptions> optionsMonitor,
            ILogger<CompetitionsService> logger
        )
        {
            _vkClient = vkClient;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _competitionOptions = optionsMonitor.CurrentValue;
        }

        public async Task Fetch(
            ChannelWriter<VkMessageModel> writer, int page = 10, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient();

            for (var i = (page - 1) * Take; i < (page - 1) * Take + Take; i++)
            {
                try
                {
                    FormUrlEncodedContent formContent = new(new List<KeyValuePair<string, string>>
                    {
                        new("page_num", i.ToString()),
                        new("our", string.Empty),
                        new("city_id", _competitionOptions.CityId)
                    });

                    var result = await client.PostAsync("https://wingri.ru/main/getPosts", formContent, cancellationToken);

                    var content = await result.Content.ReadAsStringAsync(cancellationToken);

                    var doc = new HtmlDocument();
                    doc.LoadHtml(content);
                    if (doc?.DocumentNode == null) continue;

                    var models = doc.DocumentNode?
                        .SelectNodes(
                            "//div[@class='grid-item']/div[@class='post_container']/div[@class='post_footer']/a/@href")
                        .Where(d => d.Attributes != null &&
                                    d.Attributes.Any(h => h.Name == "href" && !string.IsNullOrEmpty(h.Value)))
                        .Select(d => d.GetAttributeValue("href", "")?.Replace("https://vk.com/wall", "").Split('_'))
                        .Where(h => h != null && h.Length > 1)
                        .Select(d => new VkRepostViewModel(int.Parse(d[0]), int.Parse(d[1])))
                        .ToList();

                    if (models == null || !models.Any()) continue;
                    var response = await _vkClient.GetById(models);

                    if (response?.Response?.Items == null) continue;

                    response.Response.Items.Select(f => new VkMessageModel(f, response.Response.Groups)).ToList().ForEach(
                        async x =>
                        {
                            await writer.WriteAsync(x, cancellationToken);
                        });
                }
                catch (Exception e)
                {
                    _logger.LogError("ERROR Parsing", e);
                }
            }
        }
    }
}