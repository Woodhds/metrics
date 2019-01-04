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
using Data.EF;
using DAL.Entities;
using metrics.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;

namespace metrics.Services.Concrete
{
    public class CompetitionsService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CompetitionsService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Fetch();
                await Task.Delay(TimeSpan.FromMinutes(120), stoppingToken);
            }
        }

        async ValueTask Fetch()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _dataContext = scope.ServiceProvider.GetService<DataContext>();
                var client = new HttpClient();
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
                                                                 $"&posts={string.Join(",", list.Select(c => c.Owner_Id + "_" + c.Id))}&extended=1&fields=description");
                            var json = await response.Content.ReadAsStringAsync();
                            var jobject = JObject.Parse(json);
                            /*var groupsJ = jobject["response"]["groups"];
                            var groups = new List<VkGroup>();
                            if (groupsJ.HasValues)
                            {
                                groups = groupsJ.ToObject<List<VkGroup>>();
                            }*/

                            var posts = jobject["response"]["items"].ToObject<List<VkMessage>>();
                            /*await es.IndexManyAsync(groups, nameof(VkGroup).ToLower());
                            posts.ForEach(post =>
                            {
                                post.Owner = groups.FirstOrDefault(z => z.Id == -post.Owner_Id);
                            });*/
                            return posts;
                        }

                        using (var fs = new MemoryStream())
                        {
                            var formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("page_num", i.ToString()),
                                new KeyValuePair<string, string>("our", string.Empty),
                                new KeyValuePair<string, string>("city_id", string.Empty)
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

                            var messages = posts.Select(c => new ParseMessage
                            {
                                Id = c.Id,
                                OwnerId = c.Owner_Id,
                                Text = c.Text,
                                Date = c.Date
                            }).ToList();
                            
                            messages.ForEach(async mes =>
                            {
                                try
                                {
                                    await _dataContext.ParseMessages.AddAsync(mes);
                                }
                                catch (Exception)
                                {
                                    
                                }
                            });
                            await _dataContext.SaveChangesAsync();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}