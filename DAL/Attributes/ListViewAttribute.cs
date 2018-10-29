using System;

namespace DAL.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ListViewAttribute : Attribute
    {
        public string Name { get; set; }
    }
}