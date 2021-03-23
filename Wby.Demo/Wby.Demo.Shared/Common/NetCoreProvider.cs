using Autofac;
using System;
using Unity;

namespace Wby.Demo.Shared.Common
{
    public class NetCoreProvider
    {
        public static IContainer Instance { get; private set; }

        public static IUnityContainer UnityContainer { get; private set; }

        public static void RegisterServiceLocator(IContainer locator)
        {
            if (Instance == null)
                Instance = locator;
        }

        public static void RegisterUnityContainer(IUnityContainer locator)
        {
            if (Instance == null)
                UnityContainer = locator;
        }

        public static T Resolve<T>()
        {
            if (Instance == null)
            {
                if (!UnityContainer.IsRegistered<T>())
                    new ArgumentNullException(nameof(T));
                return UnityContainer.Resolve<T>();
            }
            else
            {
                if (!Instance.IsRegistered<T>())
                    new ArgumentNullException(nameof(T));
                return Instance.Resolve<T>();
            }
            
        }

        public static T ResolveNamed<T>(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                new ArgumentNullException(nameof(T));

            return Instance.ResolveNamed<T>(typeName);
        }
    }
}
