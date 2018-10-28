using System.Collections.Generic;

namespace DAL.Models
{
    public class ViewConfig
    {
        public List<ColumnView> Columns { get; set; }

        public ViewConfig() 
        {
            Columns = new List<ColumnView>();
        }
    }
}