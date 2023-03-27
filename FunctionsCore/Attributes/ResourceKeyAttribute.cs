using System;

namespace FunctionsCore.Attributes
{
    public class ResourceKeyAttribute : Attribute
    {
        public string ResourceKeyName { get; set; }
        public ResourceKeyAttribute(string resourceKeyName) { ResourceKeyName = resourceKeyName; }
    }
}
