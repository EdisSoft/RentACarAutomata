using System;
using System.Collections.Concurrent;

namespace FunctionsCore.Contexts
{
    public abstract class AppContextInstanceManager
    {
        private static readonly ConcurrentDictionary<Type, object> instances = new ConcurrentDictionary<Type, object>();

        public static object GetInstance(Type type)
        {
            if (!instances.ContainsKey(type))
                instances[type] = Activator.CreateInstance(type);

            return instances[type];
        }
    }
}
