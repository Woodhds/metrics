namespace Base.Contracts
{
    public class DataSourceResponseModel
    {
        public object Data { get; set; }
        public long Total { get; set; }

        public DataSourceResponseModel(object data, long total)
        {
            Data = data;
            Total = total;
        }
    }
}