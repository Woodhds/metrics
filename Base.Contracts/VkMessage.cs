using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Base.Contracts
{
    public class VkMessage
    {
        public int Id { get; set; }
        public int Owner_Id { get; set; }
        public int From_Id { get; set; }
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public List<VkMessage> Copy_History { get; set; }
        public List<MessageAttachment> Attachments { get; set; }
        public MessageReposts Reposts { get; set; }
        public VkLike Likes { get; set; }
        public Owner Owner { get; set; }
        public int Identifier => (From_Id ^ Id).GetHashCode();
        
        public int RepostedFrom { get; set; }
    }

    public class EqualityVkMessage : IEqualityComparer<VkMessage>
    {
        public bool Equals(VkMessage x, VkMessage y)
        {
            return x.Owner_Id == y.Owner_Id && x.Id == y.Id;
        }

        public int GetHashCode(VkMessage obj)
        {
            return (obj.From_Id + obj.Id).GetHashCode();
        }
    }

    public enum PostType
    {
        Post = 0,
        Video = 1
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

    public class Owner
    {
        public int Id { get; set; }
        public string Screen_Name { get; set; }
    }

    public class Profile : Owner
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
    }
}
