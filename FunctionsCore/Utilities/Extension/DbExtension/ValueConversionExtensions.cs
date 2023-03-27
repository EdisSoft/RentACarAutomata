using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;

namespace FunctionsCore.Utilities
{
    public static class ValueConversionExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
        {
            try
            {
                ValueConverter<T, String> converter = new ValueConverter<T, String>(
                    v => JsonConvert.SerializeObject(v, new JsonSerializerSettings
                    {
                        DateFormatString = "yyyy-MM-ddTHH:mm:ss.fff",
                        DateTimeZoneHandling = DateTimeZoneHandling.Local
                    }),
                    v => JsonConvert.DeserializeObject<T>(v));

                ValueComparer<T> comparer = new ValueComparer<T>(
                    (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r),
                    v => v == null ? 0 : JsonConvert.SerializeObject(v).GetHashCode(),
                    v => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v)));

                propertyBuilder.HasConversion(converter);
                propertyBuilder.Metadata.SetValueConverter(converter);
                propertyBuilder.Metadata.SetValueComparer(comparer);

            }
            catch (Exception)
            {
                Log.Info($"Hiba a HasJsonConversion közben: {propertyBuilder.Metadata.ClrType.AssemblyQualifiedName} / {propertyBuilder.Metadata.FieldInfo.DeclaringType.FullName}");
                throw;
            }

            return propertyBuilder;
        }
    }
}
