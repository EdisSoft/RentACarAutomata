using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FunctionsCore.Utilities
{
    public static class FieldAndPropsExtension
    {
        public static IEnumerable<Either<FieldInfo, PropertyInfo>> FieldsAndProps(this Type T)
        {
            var lst = new List<Either<FieldInfo, PropertyInfo>>();
            lst.AddRange(T.GetFields().Select(field => new Either<FieldInfo, PropertyInfo>.Left(field)));
            lst.AddRange(T.GetProperties().Select(prop => new Either<FieldInfo, PropertyInfo>.Right(prop)));
            return lst;
        }
    }
}
