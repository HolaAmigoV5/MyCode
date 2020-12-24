using System;
using System.Collections.Generic;
using System.Text;

namespace My.Util
{
    /// <summary>
    /// 描述：日志枚举
    /// 作者：wby 2019/9/26 14:09:42
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 跟踪
        /// </summary>
        trace,

        /// <summary>
        /// 调试
        /// </summary>
        Debug,

        /// <summary>
        /// 信息
        /// </summary>
        Info,

        /// <summary>
        /// 警告
        /// </summary>
        Warn,

        /// <summary>
        /// 错误
        /// </summary>
        Error,

        /// <summary>
        /// 致命
        /// </summary>
        Fatal
    }
}
