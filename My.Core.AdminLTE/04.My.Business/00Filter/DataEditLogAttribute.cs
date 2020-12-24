using System;
using Castle.DynamicProxy;
using My.Util;

namespace My.Business
{
    /// <summary>
    /// 描述：修改数据日志
    /// 作者：wby 2019/11/18 10:53:22
    /// </summary>
    public class DataEditLogAttribute : WriteDataLogAttribute
    {
        public DataEditLogAttribute(LogType logType, string nameField, string dataName) : base(logType, nameField, dataName) { }

        public override void OnActionExecuted(IInvocation invocation)
        {
            if((invocation.ReturnValue as AjaxResult).Success)
            {
                var obj = invocation.Arguments[0];
                Logger.Info(LogType, $"修改{DataName}:{obj.GetPropertyValue(NameField)?.ToString()}");
            }
        }

        public override void OnActionExecuting(IInvocation invocation)
        {
            throw new NotImplementedException();
        }
    }
}
