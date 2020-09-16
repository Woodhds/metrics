namespace Base.Contracts
{
    public class SimpleVkResponse<T> where T: class
    {
        public T? Response { get; set; }
    }
}
