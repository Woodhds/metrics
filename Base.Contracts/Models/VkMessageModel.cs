using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Base.Contracts.Models
{
    public class VkMessageModel
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int FromId { get; set; }
        public DateTime Date { get; set; }
        public string? Text { get; set; }
        public uint LikesCount { get; set; }
        public uint RepostsCount { get; set; }
        public string[] Images { get; set; }
        public int Identifier => (FromId ^ Id).GetHashCode();
        [JsonIgnore] public int RepostedFrom { get; set; }
        public int? MessageCategoryId { get; set; }
        [JsonIgnore] public string? MessageCategory { get; set; }
        public string? MessageCategoryPredict { get; set; }

        public VkMessageModel(VkMessage message)
        {
            Id = message.Id;
            OwnerId = message.OwnerId;
            Date = message.Date;
            Text = message.Text;
            Images = message.Attachments
                .Where(f =>
                    f.Type == MessageAttachmentType.photo &&
                    f.Photo?.Sizes != null &&
                    f.Photo.Sizes.Count > 2
                )
                .Select(f => f.Photo.Sizes[3].Url)
                .ToArray();
            LikesCount = message.Likes?.Count ?? 0;
            RepostsCount = message.Reposts?.Count ?? 0;
        }
    }
}