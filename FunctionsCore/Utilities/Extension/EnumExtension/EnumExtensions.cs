using FunctionsCore.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FunctionsCore.Utilities.Extension.EnumExtension
{
    public static class EnumExtensions
    {
        public static string ToDescriptionString(this Enum val)
        {
            var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }


        public static string ToResourceKeyString(this Enum val)
        {
            var attributes = (ResourceKeyAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(ResourceKeyAttribute), false);
            return attributes.Length > 0 ? attributes[0].ResourceKeyName : string.Empty;
        }

        public static string ToIconNameString(this Enum val)
        {
            var attributes = (IconNameAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(IconNameAttribute), false);
            return attributes.Length > 0 ? attributes[0].IconName : string.Empty;
        }

        public static int CastToInt(this Enum val)
        {
            return Convert.ToInt32(val);
        }

        public static bool CastToBool(this Enum val)
        {
            var integer = Convert.ToInt32(val);
            if (integer > 1)
                throw new ArgumentException("Az enum értéke nagyobb, mint 1 a bool castolás során!");

            return Convert.ToBoolean(integer);
        }

        public static string GetNameToLower(this Enum val)
        {
            return val.ToString().ToLower();
        }

        public static int GetSortOrder(this Enum val)
        {
            var attributes = (OrderAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(OrderAttribute), false);
            return attributes.Length > 0 ? attributes[0].Order : -1;
        }

        public static bool Visible(this Enum val)
        {
            var field = val.GetType().GetField(val.ToString());
            var exists = field.IsDefined(typeof(NotVisibleAttribute), false);
            return !exists;
        }

        public static T CastToEnum<T>(this int number)
        {
            if (!Enum.IsDefined(typeof(T), number))
                throw new Exception("Nem definiált enum elem");
            return (T)Enum.ToObject(typeof(T), number);
        }

        public static IEnumerable<TEnum> GetValues<TEnum>(bool visibleFilter = false) where TEnum : Enum, IComparable, IFormattable, IConvertible
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentException();

            if (visibleFilter)
                return Enum.GetValues(enumType).Cast<TEnum>().Where(w => w.Visible());

            return Enum.GetValues(enumType).Cast<TEnum>();

        }

        public static IEnumerable<int> GetValuesIntList<TEnum>() where TEnum : Enum, IComparable, IFormattable, IConvertible
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentException();            

            return Enum.GetValues(enumType).Cast<int>();

        }

        public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Enum konvertálás nem lehetséges");

            return Enum.Parse<TEnum>(value);
        }

        public static bool ValidEnum<TEnum>(this int value) 
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentException();
            return Enum.GetValues(enumType).Cast<int>().Any(a => a == value);
        }
    }
}
