using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Castle.DynamicProxy;
using My.Util;

namespace My.Business
{
    /// <summary>
    /// 描述：删除数据日志
    /// 作者：wby 2019/11/18 10:31:44
    /// </summary>
    public class DataDeleteLogAttribute : WriteDataLogAttribute
    {
        private List<object> DeleteList { get; set; }
        public DataDeleteLogAttribute(LogType logType, string nameField, string dataName) : base(logType, nameField, dataName) { }
        public override void OnActionExecuted(IInvocation invocation)
        {
            if ((invocation.ReturnValue as AjaxResult).Success)
            {
                string names = string.Join(",", DeleteList.Select(x => x.GetPropertyValue(NameField)?.ToString()));
                Logger.Info(LogType, $"删除{DataName}:{names}", DeleteList.ToJson());
            }
        }

        public override void OnActionExecuting(IInvocation invocation)
        {
            List<string> ids = invocation.Arguments[0] as List<string>;
            var q = invocation.InvocationTarget.GetType().GetMethod("GetIQueryable").Invoke(invocation.InvocationTarget, new object[] { }) as IQueryable;
            DeleteList = q.Where("@0.Contains(Id)", ids).CastToList<object>();
        }
    }
}
