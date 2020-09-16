using System;

namespace Base.Contracts
{
    public class VkRepostViewModel : IEquatable<VkRepostViewModel>
    {
        public int OwnerId { get; set; }
        public int Id { get; set; }

        public VkRepostViewModel()
        {
            //For deserialization
        }

        public VkRepostViewModel(int ownerId, int id)
        {
            OwnerId = ownerId;
            Id = id;
        }

        public override int GetHashCode() 
        {
            return HashCode.Combine(OwnerId, Id);
        }

        public bool Equals(VkRepostViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return OwnerId == other.OwnerId && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VkRepostViewModel) obj);
        }
    }
}
