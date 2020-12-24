using Castle.DynamicProxy;
using My.Util;
using System;

namespace My.Business
{
    /// <summary>
    /// 描述：添加数据日志
    /// 作者：wby 2019/11/18 10:22:22
    /// </summary>
    public class DataAddLogAttribute: WriteDataLogAttribute
    {
        public DataAddLogAttribute(LogType logType, string nameField, string dataName) : base(logType, nameField, dataName) { }

        public override void OnActionExecuted(IInvocation invocation)
        {
            if ((invocation.ReturnValue as AjaxResult).Success)
            {
                var obj = invocation.Arguments[0];
                Logger.Info(LogType, $"添加{DataName}:{obj.GetPropertyValue(NameField)?.ToString()}");
            }
        }

        public override void OnActionExecuting(IInvocation invocation)
        {
        }
    }
}
