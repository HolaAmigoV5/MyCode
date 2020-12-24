using Castle.DynamicProxy;
using System.Linq;
using System.Reflection;

namespace My.Util
{
    /// <summary>
    /// 描述：拦截器
    /// 作者：wby 2019/10/22 15:00:06
    /// </summary>
    public class Interceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var allFilers = invocation.MethodInvocationTarget.GetCustomAttributes<BaseFilterAttribute>(true)
                .Concat(invocation.InvocationTarget.GetType().GetCustomAttributes<BaseFilterAttribute>(true))
                .Where(x => x is IFilter).Select(x => (IFilter)x).ToList();

            //执行前
            foreach (var aFiler in allFilers)
            {
                aFiler.OnActionExecuting(invocation);
                if (!invocation.ReturnValue.IsNullOrEmpty())
                    return;
            }

            //执行
            invocation.Proceed();

            //执行后
            allFilers.ForEach(aFilter =>
            {
                aFilter.OnActionExecuted(invocation);
            });
        }
    }
}
