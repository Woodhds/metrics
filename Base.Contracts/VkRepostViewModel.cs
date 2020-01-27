namespace Base.Contracts
{
    public class VkRepostViewModel
    {
        public int Owner_Id { get; set; }
        public int Id { get; set; }

        public override int GetHashCode() 
        {
            return $"{Owner_Id}{Id}".GetHashCode();
        }
    }
}
