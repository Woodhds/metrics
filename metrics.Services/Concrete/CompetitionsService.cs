using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Linq;
using Base.Contracts;
using Base.Contracts.Options;
using metrics.Services.Abstractions;
using Microsoft.Extensions.Options;

namespace metrics.Services.Concrete
{
    public class CompetitionsService : ICompetitionsService
    {
        private readonly IVkClient _vkClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CompetitionOptions _competitionOptions;
        const int Take = 10;

        public CompetitionsService(
            IVkClient vkClient,
            IHttpClientFactory httpClientFactory,
            IOptionsMonitor<CompetitionOptions> optionsMonitor
        )
        {
            _vkClient = vkClient;
            _httpClientFactory = httpClientFactory;
            _competitionOptions = optionsMonitor.CurrentValue;
        }

        public async Task<List<VkMessage>> Fetch(int page = 1)
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var data = new List<VkRepostViewModel>();
                for (var i = (page - 1) * Take; i < (page - 1) * Take + Take; i++)
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
                    var models = doc.DocumentNode
                        .SelectNodes(
                            "//div[@class='grid-item']/div[@class='post_container']/div[@class='post_footer']/a/@href")
                        .Where(d => d.Attributes.Any(h => h.Name == "href" && !string.IsNullOrEmpty(h.Value)))
                        .Select(d => d.GetAttributeValue("href", "").Replace("https://vk.com/wall", "").Split('_'))
                        .Select(d => new VkRepostViewModel {Owner_Id = int.Parse(d[0]), Id = int.Parse(d[1])});

                    data.AddRange(models);
                }

                return _vkClient.GetById(data)?.Response.Items;
            }
            catch (Exception)
            {
            }

            return default;
        }
    }
}