using FunctionsCore.Commons;
using FunctionsCore.Commons.Base;
using FunctionsCore.Models;
using FunctionsCore.Utilities.Extension;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FunctionsCore.Contexts
{
    public class AppSettingsBase
    {
        static IConfigurationRoot config;
        public static IConfigurationRoot Getbuilder()
        {
            if (config == null)
                config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            return config;
        }

        #region Általános lekérdezések
        private static string GetStringResultSetting(string name, string key)
        {
            var builder = Getbuilder();
            var result = builder.GetValue<string>($"{name}:{key}");
            return result;
        }

        private static string GetStringResultSetting(string name)
        {
            var builder = Getbuilder();
            var result = builder.GetValue<string>($"{name}");
            return result;
        }

        private static bool GetBooleanResultSetting(string name)
        {
            var builder = Getbuilder();
            var result = builder.GetValue<bool>($"{name}");
            return result;
        }

        private static T GetResultSetting<T>(string name)
        {
            var builder = Getbuilder();
            var result = builder.GetValue<T>($"{name}");
            return result;
        }

        public static T GetObjectResultSetting<T>()
        {
            var builder = Getbuilder();
            return builder.GetSection(typeof(T).Name).Get<T>();
        }

        public static List<T> GetListResultSetting<T>(string name)
        {
            var builder = Getbuilder();
            return builder.GetSection(name).Get<List<T>>();
        }

        #endregion

        public static string GetAppSetting(string key)
        {
            return GetStringResultSetting("AppSettings", key);
        }

        public static T GetAppSetting<T>(string key)
        {
            var builder = Getbuilder();
            var result = builder.GetValue<T>($"AppSettings:{key}");
            return result;
        }

        public static Timings GetTimings()
        {
            return GetObjectResultSetting<Timings>();
        }

        public static LockerAddresses GetLockerAddresses()
        {
            return GetObjectResultSetting<LockerAddresses>();
        }
        public static string GetErtesitesiCenterSetting(string key)
        {
            return GetStringResultSetting("ErtesitesiCenter", key);
        }

        public static string GetEmailSetting(string key)
        {
            return GetStringResultSetting("EmailSettings", key);
        }

        public static string GetConnectionString(string key = "DefaultName")
        {
            return GetStringResultSetting("ConnectionStrings", key);
        }

        public static AdSettings GetAdBeallitasok()
        {
            return GetObjectResultSetting<AdSettings>();
        }

        //public static TokenManagement GetTokenBeallitasok()
        //{
        //    return GetObjectResultSetting<TokenManagement>();
        //}

        public static TrackTimeFilter GetTrackTimeFilterBeallitasok()
        {
            return GetObjectResultSetting<TrackTimeFilter>();
        }

        public static SmsTrackTime GetSmsTrackTimeFilterBeallitasok()
        {
            return GetObjectResultSetting<SmsTrackTime>();
        }



        public static int GetModulCimkeId()
        {
            var cimkeIdStr = GetStringResultSetting("ModulCimkeId");

            if (!int.TryParse(cimkeIdStr, out int resultCimkeId))
                throw new Exception("appsettings.json-ből hiányzik a ModulCimkeId");

            return resultCimkeId;
        }
        public static bool GetSqlTableDependencyEnabled()
        {
            return GetBooleanResultSetting("SqlTableDependencyEnabled");
        }


        private static DateTime TimeStrToTodayDateTime(string time)
        {
            var todayStr = DateTime.Today.ToString("yyyy-MM-dd");
            return DateTimeExtensions.SpecialDateParse($"{todayStr} {time}");
        }


    }
}
