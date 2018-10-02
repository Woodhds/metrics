using System;
using System.Collections.Generic;
using System.Text;
using DAL;

namespace Data.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
