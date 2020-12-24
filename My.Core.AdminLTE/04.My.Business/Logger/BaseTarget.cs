using Coldairarrow.Util;
using My.Entity.Base_SysManage;
using NLog;
using NLog.Targets;

namespace My.Business
{
    /// <summary>
    /// 描述：BaseTarget
    /// 作者：wby 2019/9/26 15:40:37
    /// </summary>
    public class BaseTarget: TargetWithLayout
    {
        public BaseTarget()
        {
            Name = "系统日志";
            Layout = LoggerConfig.Layout;
        }

        protected Base_SysLog GetBase_SysLogInfo(LogEventInfo logEventInfo)
        {
            Base_SysLog newLog = new Base_SysLog
            {
                Id = IdHelper.GetId(),
                Data = logEventInfo.Properties[LoggerConfig.Data] as string,
                Level = logEventInfo.Level.ToString(),
                LogContent = logEventInfo.Message,
                LogType = logEventInfo.Properties[LoggerConfig.LogType] as string,
                OpTime = logEventInfo.TimeStamp,
                OpUserName = logEventInfo.Properties[LoggerConfig.OpUserName] as string
            };
            return newLog;
        }
    }
}
