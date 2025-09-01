using System;
using System.Collections.Generic;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> servicesDictionary = new Dictionary<Type, object>();

        public static void Register<T>(T service)
        {
            Type type = typeof(T);

            if (!servicesDictionary.TryAdd(type, service))
            {
                Logger.LogWarning($"Service of type {type} is already registered. Overwriting.");
                servicesDictionary[type] = service;
            }
        }
    
        public static void Unregister<T>()
        {
            Type type = typeof(T);

            if (ContainsService<T>())
            {
                servicesDictionary.Remove(type);
                //Logger.Log($"Service of type {type} has been unregistered.");
            }
            else
            {
                Logger.LogWarning($"Service of type {type} not found for unregister.");
            }
        }
    
        public static T GetService<T>()
        {
            Type type = typeof(T);

            if (servicesDictionary.TryGetValue(type, out var service))
            {
                return (T)service;
            }

            Logger.LogError($"Service of type {type} not found.");
            return default(T);
        }

        public static void ShowAllServices()
        {
            foreach (var service in servicesDictionary)
                Logger.Log( $"{service.Key} { service.Value }" );
        }
        
        public static bool ContainsService<T>() => servicesDictionary.ContainsKey(typeof(T));

    }
}

