using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Linq;
using System.Text;

namespace My.Util
{
    /// <summary>
    /// 描述：异常处理帮助类
    /// 作者：wby 2019/10/12 14:55:40
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// 获取异常位置
        /// </summary>
        /// <param name="e">异常</param>
        /// <returns></returns>
        private static string GetExceptionAddr(Exception e)
        {
            StringBuilder excAddrBuilder = new StringBuilder();
            e?.StackTrace?.Split("\r\n".ToArray())?.ToList()?.ForEach(item =>
            {
                if (item.Contains("行号") || item.Contains("line"))
                    excAddrBuilder.Append($"    {item}\r\n");
            });
            string addr = excAddrBuilder.ToString();
            return addr.IsNullOrEmpty() ? "    无" : addr;
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="ex">捕捉的异常</param>
        /// <param name="level">内部异常层级</param>
        /// <returns></returns>
        private static string GetExceptionAllMsg(Exception ex, int level)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($@"{level}层错误：
                                       消息：
                                            {ex?.Message}
                                       位置：
                                            {GetExceptionAddr(ex)}");

            if (ex.InnerException != null)
                builder.Append(GetExceptionAllMsg(ex, level + 1));
            return builder.ToString();
        }

        /// <summary>
        /// 获取异常消息
        /// </summary>
        /// <param name="ex">捕捉的异常</param>
        /// <returns></returns>
        public static string GetExceptionAllMsg(Exception ex)
        {
            string msg = GetExceptionAllMsg(ex, 1);
            try
            {
                msg += $@"url:{HttpContextCore.Current.Request.GetDisplayUrl()}
                        body:{HttpContextCore.Current.Request.Body.ReadToString()}";
            }
            catch (Exception e)
            {
                throw e;
            }
            return msg;
        }
    }
}
