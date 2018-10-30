namespace metrics.Services.Models
{
    public class VkRepostMessage
    {
        public bool Success { get; set; }
        public uint Post_Id { get; set; }
        public uint Reposts_Count { get; set; }
        public uint Likes_Count { get; set; }
    }

    public class RepostMessageResponse
    {
        public VkRepostMessage Response { get; set; }
    }
}