using FunctionsCore.Commons.Functions;
using FunctionsCore.Enums;
using FunctionsCore.Utilities;
using FunctionsCore.Utilities.Extension.EnumExtension;
using System;

namespace FunctionsCore
{
    public class WarningException : Exception
    {
        public WarningExceptionLevel WarningExceptionLevel { get; private set; }
        public WarningException(string message, WarningExceptionLevel level = WarningExceptionLevel.Warning) : base(message)
        {
            WarningExceptionLevel = level;
            new Exception();
        }

        public WarningExceptionLevel GetExceptionLevel()
        {
            return WarningExceptionLevel;
        }
    }
}
