using Microsoft.ML;

namespace metrics.ML.Services.Models
{
    public class MLContextModel
    {
        public ITransformer Transformer { get; set; }
        public MLContext Context { get; set; }
        public DataViewSchema Schema { get; set; }
    }
}