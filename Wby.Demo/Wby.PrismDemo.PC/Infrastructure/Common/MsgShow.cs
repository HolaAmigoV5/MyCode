using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Wby.Demo.Shared.Common;

namespace Wby.PrismDemo.PC.Infrastructure.Common1
{
    public class MsgShow
    {
        public async static Task<bool> Question(string msg)
        {
            return await Show(Notify.Question, msg);
        }

        private async static Task<bool> Show(Notify notify, string msg)
        {
            string icon = string.Empty;
            string color = string.Empty;

            switch (notify)
            {
                case Notify.Error:
                    icon = "Error";
                    color = "#FF4500";
                    break;
                case Notify.Warning:
                    icon = "CommentWarning";
                    color = "#FF8247";
                    break;
                case Notify.Info:
                    icon = "CommentProcessingOutline";
                    color = "#1C86EE";
                    break;
                case Notify.Question:
                    icon = "CommentQuestionOutline";
                    color = "#20B2AA";
                    break;
            }
            //var dialog = NetCoreProvider.ResolveNamed<IMsgCenter>("MsgCenter");
            //var result = await dialog.Show(new MsgInfo { Msg = msg, Color = color, Icon = icon });
            //return result;
            return await Task.FromResult(true);
        }
    }

    public enum Notify
    {
        /// <summary>
        /// 错误
        /// </summary>
        [Description("错误")]
        Error,
        /// <summary>
        /// 警告
        /// </summary>
        [Description("警告")]
        Warning,
        /// <summary>
        /// 提示信息
        /// </summary>
        [Description("提示信息")]
        Info,
        /// <summary>
        /// 询问信息
        /// </summary>
        [Description("询问信息")]
        Question,
    }
}
