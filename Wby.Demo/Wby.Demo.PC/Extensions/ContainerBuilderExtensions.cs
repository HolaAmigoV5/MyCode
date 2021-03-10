using Autofac;
using Wby.Demo.PC.Common;
using Wby.Demo.PC.ViewCenter;
using Wby.Demo.Service;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.ViewModel;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddViewCenter(this ContainerBuilder services)
        {

            services.RegisterType<LoginCenter>().Named(typeof(LoginCenter).Name, typeof(ILoginCenter));
            services.RegisterType<MainCenter>().Named(typeof(MainCenter).Name, typeof(IMainCenter));
            services.RegisterType<MsgCenter>().Named(typeof(MsgCenter).Name, typeof(IMsgCenter));
            services.RegisterType<HomeCenter>().Named(typeof(HomeCenter).Name, typeof(IHomeCenter));
            services.RegisterType<UserCenter>().Named(typeof(UserCenter).Name, typeof(IBaseCenter));
            services.RegisterType<MenuCenter>().Named(typeof(MenuCenter).Name, typeof(IBaseCenter));
            services.RegisterType<SkinCenter>().Named(typeof(SkinCenter).Name, typeof(IBaseCenter));
            services.RegisterType<GroupCenter>().Named(typeof(GroupCenter).Name, typeof(IBaseCenter));
            services.RegisterType<BasicCenter>().Named(typeof(BasicCenter).Name, typeof(IBaseCenter));
            services.RegisterType<DashboardCenter>().Named(typeof(DashboardCenter).Name, typeof(IBaseCenter));
            return services;
        }

        public static ContainerBuilder AddRepository(this ContainerBuilder services)
        {
            services.RegisterType<UserService>().As<IUserRepository>();
            services.RegisterType<GroupService>().As<IGroupRepository>();
            services.RegisterType<MenuService>().As<IMenuRepository>();
            services.RegisterType<BasicService>().As<IBasicRepository>();
            services.RegisterType<WbyNLog>().As<ILog>();
            return services;
        }

        public static ContainerBuilder AddViewModel(this ContainerBuilder services)
        {
            services.RegisterType<UserViewModel>().As<IUserViewModel>();
            services.RegisterType<LoginViewModel>().As<ILoginViewModel>();
            services.RegisterType<MainViewModel>().As<IMainViewModel>();
            services.RegisterType<GroupViewModel>().As<IGroupViewModel>();

            services.RegisterType<MenuViewModel>().As<IMenuViewModel>();
            services.RegisterType<BasicViewModel>().As<IBasicViewModel>();
            services.RegisterType<SkinViewModel>().As<ISkinViewModel>();
            services.RegisterType<HomeViewModel>().As<IHomeViewModel>();
            services.RegisterType<DashboardViewModel>().As<IDashboardViewModel>();
            return services;
        }
    }
}
