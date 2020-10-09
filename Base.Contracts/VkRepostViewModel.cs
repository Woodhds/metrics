using System;

namespace Base.Contracts
{
    public struct VkRepostViewModel : IEquatable<VkRepostViewModel>
    {
        public int OwnerId { get; set; }
        public int Id { get; set; }

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
            return OwnerId == other.OwnerId && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == this.GetType() && Equals((VkRepostViewModel) obj);
        }

        public static bool operator ==(VkRepostViewModel left, VkRepostViewModel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VkRepostViewModel left, VkRepostViewModel right)
        {
            return !(left == right);
        }
    }
}
