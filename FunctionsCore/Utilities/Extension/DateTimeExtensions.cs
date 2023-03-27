using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FunctionsCore.Utilities.Extension
{
    public static class DateTimeExtensions
    {
        public static bool UgyanarraAHetreEsnek(this DateTime date1, DateTime date2)
        {
            return date1.HetElsoNapja() == date2.HetElsoNapja();
        }

        public static bool UgyanarraAHonapraEsnek(this DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month;
        }

        public static DateTime HetElsoNapja(this DateTime date)
        {
            return date.Date.AddDays(-(int)DateTime.Now.DayOfWeek + 1);
        }

        public static bool TimeBetween(this DateTime datetime, TimeSpan start, TimeSpan end)
        {
            TimeSpan now = datetime.TimeOfDay;
            if (start < end)
                return start <= now && now <= end;
            return !(end < now && now < start);
        }

        public static string DatumNapRagozasTol(this DateTime datetime)
        {
            var tol = new HashSet<int>{ 1,2,4,5,7,9,10,11,12,14,15,17,19,21,22,24,25,27,29,31};

            if(tol.Contains(datetime.Day))
            {
                return "től";
            }
            return "tól";

        }

        public static DateTime ZeroMilliseconds(this DateTime dt)
        {
            return new DateTime(((dt.Ticks / 10000000) * 10000000), dt.Kind);
        }

        public static DateTime SetUtolsoValtozasDatuma(DateTime? d1, DateTime? d2, DateTime? d3)
        {
            List<DateTime?> datumok = new List<DateTime?> { d1, d2, d3 };
            DateTime? maxDate = datumok.Max();
            if (maxDate == default(DateTime))
            {
                maxDate = null;
            }

            return maxDate ?? DateTime.Now;
        }

        public static DateTime Max(params DateTime?[] dates) => dates.Where(w => w.HasValue).Select(s => s.Value).DefaultIfEmpty().Max();

        public static DateTime SpecialDateParse(string s) 
        {
            Regex regex = new Regex(@"(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2})");
            Match match = regex.Match(s);
            if (!match.Success)
                throw new FormatException("Not a valid DateTime format (yyyy-MM-dd HH:mm)");

            int[] parts = match.Groups.Cast<Group>().Skip(1).Select(x => int.Parse(x.Value)).ToArray();

            int year = parts[0];
            int month = parts[1];
            int day = parts[2];
            int hour = parts[3];
            int minute = parts[4];

            bool addDay = false;
            if (hour >= 24)
            {
                addDay = true;
                hour -= 24;
            }            

            DateTime dateTime = new DateTime(year, month, day, hour, minute, 0);
            if (addDay)
                dateTime = dateTime.AddDays(1);
            return dateTime;
        }

        public static DateTime MinOf(this DateTime instance, DateTime dateTime)
        {
            return instance < dateTime ? instance : dateTime;
        }

        public static DateTime MaxOf(this DateTime instance, DateTime dateTime)
        {
            return instance > dateTime ? instance : dateTime;
        }

        public static DateTime StartOfMonth(this DateTime d) 
        {
            return new DateTime(d.Year, d.Month, 1);
        }
        public static DateTime EndOfMonth(this DateTime d)
        {
            return d.StartOfMonth().AddMonths(1).AddDays(-1);
        }

        public static string MinutesToHoursMinutes(this double minutes) 
        {
            TimeSpan spWorkMin = TimeSpan.FromMinutes(minutes);
            return $"{(int)spWorkMin.TotalHours:00} óra {spWorkMin.Minutes:00} perc";
        }

        public static DateTime AddBusinessDays(this DateTime dt, int nDays)
        {
            int weeks = nDays / 5;
            nDays %= 5;
            while (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                dt = dt.AddDays(1);

            while (nDays-- > 0)
            {
                dt = dt.AddDays(1);
                if (dt.DayOfWeek == DayOfWeek.Saturday)
                    dt = dt.AddDays(2);
            }
            return dt.AddDays(weeks * 7);
        }


        /// <summary>
        /// 2023. január 13.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToHuString(this DateTime dt)
        {
            return dt.ToString("yyyy. MMMM dd.", CultureInfo.GetCultureInfo("hu-HU"));
        }
    }
}
