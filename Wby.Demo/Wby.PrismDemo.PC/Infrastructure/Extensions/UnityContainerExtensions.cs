using Unity;
using Wby.Demo.Service;
using Wby.Demo.Shared.DataInterfaces;
using Wby.PrismDemo.PC.Infrastructure.Common;

namespace Wby.PrismDemo.PC.Infrastructure.Extensions
{
    public static class UnityContainerExtensions
    {
        public static void RegisterServers(this IUnityContainer container)
        {
            container.RegisterType<IUserRepository, UserService>();
            container.RegisterType<IMenuRepository, MenuService>();
            container.RegisterType<IGroupRepository, GroupService>();
            container.RegisterType<ILog, WbyNLog>();
        }
    }
}
