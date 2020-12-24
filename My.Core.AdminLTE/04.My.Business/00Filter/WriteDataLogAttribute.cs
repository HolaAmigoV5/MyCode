using My.Util;
using System;

namespace My.Business
{
    /// <summary>
    /// 描述：写数据日志
    /// 作者：wby 2019/11/18 9:42:43
    /// </summary>
    public abstract class WriteDataLogAttribute: BaseFilterAttribute
    {
        protected LogType LogType { get; }
        protected string DataName { get; }
        protected string NameField { get; }
        protected Type EntityType { get; }
        protected ILogger Logger { get; } = AutofacHelper.GetScopeService<ILogger>();

        public WriteDataLogAttribute(LogType logType,string nameField,string dataName)
        {
            LogType = logType;
            NameField = nameField;
            DataName = dataName;
        }
    }
}
