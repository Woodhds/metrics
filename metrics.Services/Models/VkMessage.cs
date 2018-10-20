using System;
using System.Collections.Generic;
using System.Text;

namespace metrics.Services.Models
{
    public class VkMessage
    {
        public uint id { get; set; }
        public int owner_id { get; set; }
        public int from_id { get; set; }
        public uint date { get; set; }
        public string Text { get; set; }
        public PostType Post_type { get; set; }
        public List<VkMessage> Copy_History { get; set; }
        public List<MessageAttachment> Attachments { get; set; }
        public MessageReposts Reposts { get; set; }
    }

    public class EqualityVkMessage : IEqualityComparer<VkMessage>
    {
        public bool Equals(VkMessage x, VkMessage y)
        {
            return x.owner_id == y.owner_id && x.id == y.id;
        }

        public int GetHashCode(VkMessage obj)
        {
            return (obj.from_id + obj.id).GetHashCode();
        }
    }

    public enum PostType
    {
        Post = 0
    }

    public enum MessageAttachmentType
    {
        photo = 0,
        audio = 1,
        video = 2,
        link = 3,
        poll = 4,
        page = 5,
        album = 6,
        doc = 7,
        posted_photo = 8,
        graffiti = 9,
        note = 10,
        app = 11,
        photos_list = 12,
        market = 13,
        market_album = 14,
        sticker = 15,
        pretty_cards = 16
    }

    public class MessageAttachment
    {
        public MessageAttachmentType Type { get; set; }
        public AttachmentPhoto Photo { get; set; }
    }

    public class AttachmentPhoto
    {
        public uint id { get; set; }
        public List<PhotoSize> Sizes { get; set; } = new List<PhotoSize>();
    }

    public class PhotoSize
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public PhotoSizeType Type { get; set; }
        public string Url { get; set; }
    }

    public enum PhotoSizeType
    {
        m = 0,
        o = 1,
        p = 2,
        q = 3,
        r = 4,
        s = 5,
        x = 6,
        y = 7,
        z = 8,
        w = 9
    }
}
