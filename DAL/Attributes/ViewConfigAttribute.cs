using System;

namespace DAL.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewConfigAttribute : Attribute
    {
        public string LookupProperty { get; set; }

        public ViewConfigAttribute(string lookupProperty)
        {
            LookupProperty = lookupProperty;
        }
    }
}