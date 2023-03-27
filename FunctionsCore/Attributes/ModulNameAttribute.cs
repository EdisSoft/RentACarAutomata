using FunctionsCore.Enums;
using System;

namespace FunctionsCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModulNameAttribute : Attribute
    {
        public Modulok ModulName { get; set; }
    }
}
