namespace My.Business
{
    /// <summary>
    /// 描述：日志配置
    /// 作者：wby 2019/9/26 15:16:52
    /// </summary>
    public class LoggerConfig
    {
        public static readonly string LoggerName = "SysLog";
        public static readonly string LogType = "LogType";
        public static readonly string OpUserName = "OpUserName";
        public static readonly string Data = "Data";
        public static readonly string Layout = $@"${{date:format=yyyy-MM-dd HH\:mm\:ss}}|${{level}}|日志类型:${{event-properties:item={LogType}}}|
                                                    操作员:${{event-properties:item={OpUserName}}}|内容:${{message}}|备份数据:${{event-properties:item={Data}}}";
    }
}
