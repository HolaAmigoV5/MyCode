using NLog;
using System;

namespace DaJuTestDemo.Core
{
    public class LoggerHelper
    {
        #region Init
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static LoggerHelper _obj;

        public static LoggerHelper Logger
        {
            get => _obj ?? (new LoggerHelper());
            set => _obj = value;
        }
        #endregion

        #region 调试
        public void Debug(string message)
        {
            logger.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            logger.Debug(message, args);
        }

        public void Debug(Exception exception, string message)
        {
            logger.Debug(exception, message);
        }
        #endregion

        #region 信息
        public void Info(string message)
        {
            logger.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            logger.Info(message, args);
        }

        public void Info(Exception exception, string message)
        {
            logger.Info(exception, message);
        }
        #endregion

        #region 警告
        public void Warn(string message)
        {
            logger.Debug(message);
        }

        public void Warn(string message, params object[] args)
        {
            logger.Debug(message, args);
        }

        public void Warn(Exception exception, string message)
        {
            logger.Debug(exception, message);
        }
        #endregion

        #region 错误
        public void Error(Exception exception, string message)
        {
            logger.Error(exception, message);
        }

        public void Error(string message, params object[] args)
        {
            logger.Error(message, args);
        }

        public void Error(string message)
        {
            logger.Fatal(message);
        }

        public void Fatal(Exception exception, string message)
        {
            logger.Fatal(exception, message);
        }

        public void Fatal(string message, params object[] args)
        {
            logger.Fatal(message, args);
        }

        public void Fatal(string message)
        {
            logger.Fatal(message);
        }
        #endregion

    }
}
