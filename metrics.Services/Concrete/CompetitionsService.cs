using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using metrics.Services.Models;
using metrics.Services.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace metrics.Services.Concrete
{
    public class CompetitionsService : BackgroundService
    {
        private readonly CompetitionOptions _options;
        private readonly ILogger<CompetitionsService> _logger;

        public CompetitionsService(IOptions<CompetitionOptions> options, ILogger<CompetitionsService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Fetch();
                await Task.Delay(TimeSpan.FromMinutes(_options.Delay), stoppingToken);
            }
        }

        async ValueTask Fetch()
        {
            var client = new HttpClient();
            ElasticClient es = new ElasticClient(new Uri(_options.Address));
            var transform = new XslTransform();
            transform.Load("transform.xslt");
            for (int i = 1; i < 100; i++)
            {
                try
                {
                    async Task<List<VkMessage>> GetPosts(List<VkRepostViewModel> list)
                    {
                        var response = await client.GetAsync("https://api.vk.com/method/wall.getById?v=5.85" +
                                                             "&access_token=00af0c15d3aef805fadf299ddc3c173647192af2becb3ea9c679157b2d03bcafd7caa759f527e59d95d7e&post" +
                                                             $"&posts={string.Join(",", list.Select(c => c.Owner_Id + "_" + c.Id))}&extended=1");
                        var json = await response.Content.ReadAsStringAsync();
                        var jobject = JObject.Parse(json);
                        return JsonConvert.DeserializeObject<List<VkMessage>>(jobject["response"]["items"].ToString());
                    }

                    using (var fs = new MemoryStream())
                    {
                        var formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("page_num", i.ToString()),
                            new KeyValuePair<string, string>("our", string.Empty),
                            new KeyValuePair<string, string>("city_id", 97.ToString())
                        });
                        var result = await client.PostAsync("https://wingri.ru/main/getPosts", formContent);
                        
                        var content = await result.Content.ReadAsStringAsync();
                        content = Regex.Replace("<div>" + content + "</div>", @"<br\s*?\/>|<br>", string.Empty);

                        var doc = new XmlDocument();
                        content = content.Replace("&", "");
                        doc.LoadXml(content);
                        var fileStream = new XmlSerializer(typeof(List<VkRepostViewModel>),
                            new XmlRootAttribute("PostItems"));

                        transform.Transform(doc.CreateNavigator(), null, fs);
                        fs.Seek(0, SeekOrigin.Begin);
                        var data = fileStream.Deserialize(fs);
                        var posts = await GetPosts(data as List<VkRepostViewModel>);
                        await es.IndexManyAsync(posts, _options.Index);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
        }
    }
}