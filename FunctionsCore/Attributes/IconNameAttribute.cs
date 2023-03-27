using System;

namespace FunctionsCore.Attributes
{
    public class IconNameAttribute : Attribute
    {
        public string IconName { get; set; }
        public IconNameAttribute(string iconName) { IconName = iconName; }
    }
}
