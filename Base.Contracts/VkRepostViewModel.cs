using System;

namespace Base.Contracts
{
    public class VkRepostViewModel : IEquatable<VkRepostViewModel>
    {
        public int Owner_Id { get; set; }
        public int Id { get; set; }

        public VkRepostViewModel(int ownerId, int id)
        {
            Owner_Id = ownerId;
            Id = id;
        }

        public override int GetHashCode() 
        {
            return HashCode.Combine(Owner_Id, Id);
        }

        public bool Equals(VkRepostViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Owner_Id == other.Owner_Id && Id == other.Id;
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
