using System;
using System.Collections.Generic;
using System.Text;

namespace metrics.Services.Models
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
