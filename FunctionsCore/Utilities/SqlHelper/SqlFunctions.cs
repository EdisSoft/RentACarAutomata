using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;

namespace FunctionsCore.Utilities.SqlHelper
{
    public static class SqlFunctions
    {
        public enum DatePart
        {
            year,
            quarter,
            month,
            dayofyear,
            day,
            week,
            hour,
            minute,
            second,
            millisecond,
            microsecond,
            nanosecond
        }

        [DbFunction("JSON_VALUE", "")]
        public static string JsonValue(object source, [NotParameterized] string path)
        {
            throw new NotSupportedException();
        }
        [DbFunction("DATEDIFF", "")]
        public static int? DateDiff(DatePart datePartArg, string startDate, string endDate)
        {
            throw new NotSupportedException();
        }

    }
}
