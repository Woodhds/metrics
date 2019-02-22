using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using metrics.Services.Abstract;
using metrics.Services.Models;

namespace metrics.Services.Concrete
{
    public class CompetitionsService : ICompetitionsService
    {
        private readonly IVkClient _vkClient;
        public CompetitionsService(IVkClient vkClient)
        {
            _vkClient = vkClient;
        }
        
        public async Task<List<VkMessage>> Fetch()
        {
            var client = new HttpClient();
            var transform = new XslTransform();
            transform.Load("transform.xslt");
            var messages = new List<VkMessage>();
            for (int i = 1; i < 50; i++)
            {
                try
                {

                    using (var fs = new MemoryStream())
                    {
                        var formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("page_num", i.ToString()),
                            new KeyValuePair<string, string>("our", string.Empty),
                            new KeyValuePair<string, string>("city_id", "5")
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
                        var posts = _vkClient.GetById(data as List<VkRepostViewModel>);
                        messages.AddRange(posts.Response?.Items);
                    }
                }
                catch (Exception)
                {
                }
            }

            return messages;
        }
    }
}