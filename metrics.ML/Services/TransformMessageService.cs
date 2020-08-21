using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Services.Abstractions;
using Microsoft.Extensions.Hosting;

namespace metrics.ML.Services
{
    public class TransformMessageService : BackgroundService
    {
        private readonly IVkClient _vkClient;

        public TransformMessageService(IVkClient vkClient)
        {
            _vkClient = vkClient;
        }

        public async Task Transform(Stream stream, CancellationToken ct = default)
        {
            var lines = await ReadFile(stream);

            var skip = 0;
            const int take = 100;
            var result = new List<string>();
            while (skip < lines.Count)
            {
                var posts = lines.Skip(skip).Take(take)
                    .Select(GetPost)
                    .Select(x => new VkRepostViewModel {Id = x.id, Owner_Id = x.ownerId})
                    .ToArray();

                if (posts.Any())
                {
                    result.AddRange(await GetTexts(posts));
                }

                skip += take;
                await Task.Delay(3000, ct);
            }

            await File.WriteAllLinesAsync("Out.csv", result, ct);
        }

        private (int id, int ownerId, int categoryId) GetPost(string line)
        {
            var numbers = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return (Convert.ToInt32(numbers[0]), Convert.ToInt32(numbers[1]), Convert.ToInt32(numbers[2]));
        }

        private async Task<IEnumerable<string>> GetTexts(IEnumerable<VkRepostViewModel> posts)
        {
            return (await _vkClient.GetById(posts))?.Response?.Items?.Select(f => ProcessMessageText(f.Text)) ??
                   new string[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ProcessMessageText(string text)
        {
            return text.Replace("\n", string.Empty).Replace("\"", "\"\"");
        }

        private async Task<IList<string>> ReadFile(Stream stream)
        {
            using var sr = new StreamReader(stream);
            var lines = new List<string>();
            string line;
            while ((line = await sr.ReadLineAsync()) != null)
            {
                lines.Add(line);
            }

            return lines;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Transform(File.OpenRead(@""), stoppingToken);
        }
    }
}