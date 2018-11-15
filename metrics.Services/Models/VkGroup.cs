using Nest;

namespace metrics.Services.Models
{
    [ElasticsearchType(Name = nameof(VkGroup), IdProperty = nameof(Id))]
    public class VkGroup : Owner
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
