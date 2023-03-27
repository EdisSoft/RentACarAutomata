using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionsCore.Utilities
{
    public static class ModelHelper
    {
        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            if (source == null || dest == null)
                return;
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead && x.Name != null).ToList();
            var destProps = typeof(TU).GetProperties()
                    .Where(x => x.CanWrite && x.Name != null)
                    .ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (destProps.Any(x => x.Name == sourceProp.Name))
                {
                    var p = destProps.First(x => x.Name == sourceProp.Name);
                    if (p.CanWrite)
                    { // check if the property can be set or no.
                        p.SetValue(dest, sourceProp.GetValue(source, null), null);
                    }
                }
            }
        }

        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            var _propertyNames = propertyName.Split('.');

            for (var i = 0; i < _propertyNames.Length; i++)
            {
                if (obj != null)
                {
                    var _propertyInfo = obj.GetType().GetProperty(_propertyNames[i]);
                    if (_propertyInfo != null)
                        obj = _propertyInfo.GetValue(obj);
                    else
                        obj = null;
                }
            }

            if (obj == null) return default(T);

            return (T)obj;
        }
    }

}
