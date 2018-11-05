using System;
using DAL.Models;

namespace DAL.Attributes
{
    public class PropertyDataTypeAttribute : Attribute
    {
        public readonly PropertyDataType Type;
        public PropertyDataTypeAttribute(PropertyDataType type)
        {
            Type = type;
        }
    }
}