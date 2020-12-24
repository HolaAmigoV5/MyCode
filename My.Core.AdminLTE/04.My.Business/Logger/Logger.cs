using My.Util;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;

namespace My.Business
{
    /// <summary>
    /// 描述：日志类
    /// 作者：wby 2019/9/26 14:33:25
    /// </summary>
    public class Logger : ILogger,IDependency
    {
        #region 私有成员
        private IOperator _operator { get; } = AutofacHelper.GetScopeService<IOperator>(); 
        #endregion

        #region 构造函数
        public Logger()
        {
            var config = new LoggingConfiguration();
            string layout = LoggerConfig.Layout;

            //控制台
            if (GlobalSwitch.LoggerType.HasFlag(LoggerType.Console))
            {
                AddTarget(new ColoredConsoleTarget
                {
                    Name = LoggerConfig.LoggerName,
                    Layout = layout
                });
            }
            //文件
            if (GlobalSwitch.LoggerType.HasFlag(LoggerType.File))
            {
                AddTarget(new FileTarget
                {
                    Name = LoggerConfig.LoggerName,
                    Layout = layout,
                    FileName = Path.Combine(Directory.GetCurrentDirectory(), "logs", "${date:format=yyyy-MM-dd}.txt")
                });
            }

            //数据库
            if (GlobalSwitch.LoggerType.HasFlag(LoggerType.RDBMS))
            {
                AddTarget(new RDBMSTarget { Layout = layout });
            }

            //ElasticSearch
            if (GlobalSwitch.LoggerType.HasFlag(LoggerType.ElasticSearch))
            {
                AddTarget(new ElasticSearchTarget { Layout = layout });
            }

            NLog.LogManager.Configuration = config;

            void AddTarget(Target target)
            {
                config.AddTarget(target);
                config.AddRuleForAllLevels(target);
            }
        }
        #endregion

        #region 接口实现
        public void Log(LogLevel logLevel, LogType logType, string msg)
        {
            Log(logLevel, logType, msg, "");
        }

        public void Log(LogLevel logLevel, LogType logType, string msg, string data)
        {
            NLog.Logger _nLogger = NLog.LogManager.GetLogger("sysLogger");
            NLog.LogEventInfo log = new NLog.LogEventInfo(NLog.LogLevel.FromString(logLevel.ToString()), "sysLogger", msg);
            log.Properties[LoggerConfig.Data] = data;
            log.Properties[LoggerConfig.LogType] = logType.ToString();
            log.Properties[LoggerConfig.OpUserName] = _operator?.Property?.UserName;

            _nLogger.Log(log);
        }

        public void Trace(LogType logType, string msg)
        {
            Log(LogLevel.trace, logType, msg);
        }

        public void Trace(LogType logType, string msg, string data)
        {
            Log(LogLevel.trace, logType, msg, data);
        }

        public void Debug(LogType logType, string msg)
        {
            Log(LogLevel.Debug, logType, msg);
        }

        public void Debug(LogType logType, string msg, string data)
        {
            Log(LogLevel.Debug, logType, msg, data);
        }

        public void Info(LogType logType, string msg)
        {
            Log(LogLevel.Info, logType, msg);
        }

        public void Info(LogType logType, string msg, string data)
        {
            Log(LogLevel.Info, logType, msg, data);
        }

        public void Warn(LogType logType, string msg)
        {
            Log(LogLevel.Warn, logType, msg);
        }

        public void Warn(LogType logType, string msg, string data)
        {
            Log(LogLevel.Warn, logType, msg, data);
        }

        public void Error(LogType logType, string msg)
        {
            Log(LogLevel.Error, logType, msg);
        }

        public void Error(LogType logType, string msg, string data)
        {
            Log(LogLevel.Error, logType, msg, data);
        }

        public void Error(Exception ex)
        {
            Log(LogLevel.Error, LogType.系统异常, ExceptionHelper.GetExceptionAllMsg(ex));
        }

        public void Fatal(LogType logType, string msg)
        {
            Log(LogLevel.Fatal, logType, msg);
        }

        public void Fatal(LogType logType, string msg, string data)
        {
            Log(LogLevel.Fatal, logType, msg, data);
        }


        #endregion
    }
}
