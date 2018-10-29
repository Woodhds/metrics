namespace metrics.Models
{
    public class DataSourceResponseModel
    {
        public object Data { get; set; }
        public int Total { get; set; }

        public DataSourceResponseModel(object data, int total) 
        {
            Data = data;
            Total = total;
        }
    }
}