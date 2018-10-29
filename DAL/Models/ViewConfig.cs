using System.Collections.Generic;

namespace DAL.Models
{
    public class ViewConfig
    {
        public string Name { get; set; }
        public string LookupProperty { get; set; }
        public List<ColumnView> Columns { get; set; }

        public ViewConfig()
        {
            Columns = new List<ColumnView>();
        }
    }
}