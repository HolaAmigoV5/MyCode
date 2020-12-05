using Castle.DynamicProxy;
using System;

namespace DependencyInjectionAutofacDemo.Services
{
    public class Interceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"Intercept before,Method:{invocation.Method.Name}");
            invocation.Proceed();  //原有方法执行
            Console.WriteLine($"Intercept after,Method:{invocation.Method.Name}");
        }
    }
}
