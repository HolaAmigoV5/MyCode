using Castle.DynamicProxy;

namespace My.Util
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="invocation"></param>
        void OnActionExecuting(IInvocation invocation);

        /// <summary>
        /// 执行后
        /// </summary>
        /// <param name="invocation"></param>
        void OnActionExecuted(IInvocation invocation);
    }
}
