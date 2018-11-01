namespace DAL.Models
{
    public class ColumnView
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public bool Required { get; set; }
        public PropertyDataType Type { get; set; }
    }
}