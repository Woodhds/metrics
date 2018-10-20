namespace metrics.Services.Models
{
    public class VkRepostMessage
    {
        public bool Success { get; set; }
        public uint Post_id { get; set; }
        public uint Reposts_count { get; set; }
        public uint Likes_count { get; set; }
    }

    public class RepostMessageResponse
    {
        public VkRepostMessage Response { get; set; }
    }
}