namespace Base.Contracts
{
    public class VkGroup : Owner
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Is_member { get; set; }
    }
}
