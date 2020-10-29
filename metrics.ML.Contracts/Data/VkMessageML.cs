using Microsoft.ML.Data;

namespace metrics.ML.Contracts.Data
{
    public class VkMessageML
    {
        [LoadColumn(1)]
        public int Id { get; set; }
        [LoadColumn(0)]
        public int OwnerId { get; set; }
        [LoadColumn(3)]
        public string? Text { get; set; }
        [LoadColumn(3)]
        public string? Category { get; set; }
    }
}