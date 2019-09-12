using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using metrics.Services.Abstract;
using metrics.Services.Models;
using System.Linq;

namespace metrics.Services.Concrete
{
    public class CompetitionsService : ICompetitionsService
    {
        private readonly IVkClient _vkClient;
        public CompetitionsService(IVkClient vkClient)
        {
            _vkClient = vkClient;
        }

        public async Task<List<VkMessage>> Fetch(int page = 1)
        {
            var client = new HttpClient();
            var messages = new List<VkMessage>();
            try
            {

                using (var fs = new MemoryStream())
                {
                    var formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("page_num", page.ToString()),
                            new KeyValuePair<string, string>("our", string.Empty),
                            new KeyValuePair<string, string>("city_id", "5")
                        });
                    var result = await client.PostAsync("https://wingri.ru/main/getPosts", formContent);

                    var content = await result.Content.ReadAsStringAsync();

                    var doc = new HtmlDocument();
                    doc.LoadHtml(content);
                    var data = doc.DocumentNode.SelectNodes("//div[@class='grid-item']/div[@class='post_container']/div[@class='post_footer']/a/@href").Where(d => d.Attributes.Any(d => d.Name == "href" && !string.IsNullOrEmpty(d.Value)))
                        .Select(d => d.GetAttributeValue("href", "").Replace("https://vk.com/wall", "").Split('_'))
                        .Select(d => new VkRepostViewModel { Owner_Id = int.Parse(d[0]), Id = int.Parse(d[1]) });

                    var posts = _vkClient.GetById(data);
                    messages.AddRange(posts.Response?.Items);
                }
            }
            catch (Exception)
            {
            }
            return messages;
        }
    }
}