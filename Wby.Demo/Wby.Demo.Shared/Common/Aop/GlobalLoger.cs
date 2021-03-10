using AspectInjector.Broker;
using System;
using Wby.Demo.Shared.DataInterfaces;

namespace Wby.Demo.Shared.Common.Aop
{
    /// <summary>
    /// 全局日志
    /// </summary>
    [Aspect(Scope.Global)]
    [Injection(typeof(GlobalLoger))]
    public class GlobalLoger : Attribute
    {
        private readonly ILog log;
        public GlobalLoger()
        {
            log = NetCoreProvider.Resolve<ILog>();
        }

        [Advice(Kind.Before, Targets = Target.Method)]
        public void Start([Argument(Source.Name)] string methodName, [Argument(Source.Arguments)] object[] arg)
        {
            log.Debug($"开始调用方法:{methodName}, 参数:{string.Join(",", arg)}");
        }

        [Advice(Kind.After, Targets = Target.Method)]
        public void End([Argument(Source.Name)] string methodName, [Argument(Source.ReturnValue)] object arg)
        {
            log.Debug($"结束调用方法:{methodName}, 返回值:{arg}");
        }
    }
}
