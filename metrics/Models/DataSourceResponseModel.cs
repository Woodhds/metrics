namespace metrics.Models
{
    public class DataSourceResponseModel<T>
    {
        public T Data { get; set; }
        public uint Total { get; set; }

        public DataSourceResponseModel(T data, uint total) 
        {
            Data = data;
            Total = total;
        }
    }
}