using Microsoft.ML.Data;

namespace metrics.ML.Contracts.Data
{
    public class VkMessagePredict
    {
        [ColumnName("PredictedLabel")]
        public string Category;

        public float[] Score;
    }
}