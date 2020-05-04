using Microsoft.ML.Data;

namespace metrics.ML.Data
{
    public class VkMessagePredict
    {
        [ColumnName("PredictedLabel")]
        public string Category;

        public float[] Score;
    }
}