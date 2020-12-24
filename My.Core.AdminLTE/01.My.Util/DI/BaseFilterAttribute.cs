using System;
using Castle.DynamicProxy;

namespace My.Util
{
    /// <summary>
    /// 描述：过滤器
    /// 作者：wby 2019/9/26 13:49:45
    /// </summary>
    public abstract class BaseFilterAttribute : Attribute, IFilter
    {
        public abstract void OnActionExecuted(IInvocation invocation);

        public abstract void OnActionExecuting(IInvocation invocation);
    }
}
