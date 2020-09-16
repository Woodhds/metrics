using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Base.Contracts;
using Base.Contracts.Models;
using Base.Contracts.Options;
using HtmlAgilityPack;
using metrics.Competitions.Abstractions;
using metrics.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace metrics.Competitions.Hosted.Services
{
    public class CompetitionsService : ICompetitionsService
    {
        private readonly IVkClient _vkClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CompetitionOptions _competitionOptions;
        private const int Take = 10;
        private readonly ILogger<CompetitionsService> _logger;

        public CompetitionsService(
            IVkClient vkClient,
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

        public async Task<IList<VkMessageModel>> Fetch(int page = 10)
        {
            var client = _httpClientFactory.CreateClient();

            var data = new List<VkMessageModel>();
            for (var i = (page - 1) * Take; i < (page - 1) * Take + Take; i++)
            {
                try
                {
                    var formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("page_num", i.ToString()),
                        new KeyValuePair<string, string>("our", string.Empty),
                        new KeyValuePair<string, string>("city_id", _competitionOptions.CityId)
                    });

                    var result = await client.PostAsync("https://wingri.ru/main/getPosts", formContent);

                    var content = await result.Content.ReadAsStringAsync();

                    var doc = new HtmlDocument();
                    doc.LoadHtml(content);
                    if (doc?.DocumentNode == null) continue;
                    
                    var models = doc.DocumentNode?
                        .SelectNodes(
                            "//div[@class='grid-item']/div[@class='post_container']/div[@class='post_footer']/a/@href")
                        .Where(d => d.Attributes != null && d.Attributes.Any(h => h.Name == "href" && !string.IsNullOrEmpty(h.Value)))
                        .Select(d => d.GetAttributeValue("href", "")?.Replace("https://vk.com/wall", "").Split('_'))
                        .Where(h => h != null && h.Length > 1)
                        .Select(d => new VkRepostViewModel(int.Parse(d[0]), int.Parse(d[1])))
                        .ToList();

                    if (models != null && models.Any())
                    {
                        data.AddRange((await _vkClient.GetById(models)).Response.Items.Select(f => new VkMessageModel(f)));
                    }
                    
                }
                catch (Exception e)
                {
                    _logger.LogError("ERROR Parsing", e);
                }
            }

            return data;
        }
    }
}