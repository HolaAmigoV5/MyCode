using Autofac;
using System;

namespace Wby.Demo.Shared.Common
{
    public class NetCoreProvider
    {
        public static IContainer Instance { get; private set; }

        public static void RegisterServiceLocator(IContainer locator)
        {
            if (Instance == null)
                Instance = locator;
        }

        public static T Resolve<T>()
        {
            if (!Instance.IsRegistered<T>())
                new ArgumentNullException(nameof(T));

            return Instance.Resolve<T>();
        }

        public static T ResolveNamed<T>(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                new ArgumentNullException(nameof(T));

            return Instance.ResolveNamed<T>(typeName);
        }
    }
}
