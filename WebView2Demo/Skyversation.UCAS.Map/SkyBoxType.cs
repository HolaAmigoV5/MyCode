using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyversation.UCAS.Map
{
    public enum SkyBoxType
    {
        [Description("无")]
        WU = 0,

        /// <summary>
        /// 金色晨曦
        /// </summary>
        [Description("金色晨曦")]
        JSCX = 1,

        /// <summary>
        /// 光暗之手
        /// </summary>
        [Description("光暗之手")]
        GYZS = 2,

        /// <summary>
        /// 天马行空
        /// </summary>
        [Description("天马行空")]
        TMXK = 4,

        /// <summary>
        /// 飘絮人间
        /// </summary>
        [Description("飘絮人间")]
        PXRJ = 7,

        /// <summary>
        /// 七彩紫罗
        /// </summary>
        [Description("七彩紫罗")]
        QCZL = 9,

        /// <summary>
        /// 云中之触
        /// </summary>
        [Description("云中之触")]
        YZZC = 10,

        /// <summary>
        /// 鲲鹏万里
        /// </summary>
        [Description("鲲鹏万里")]
        KPWL = 11,

        /// <summary>
        /// 血色苍穹
        /// </summary>
        [Description("血色苍穹")]
        XSCQ = 12,

        /// <summary>
        /// 白云旋天
        /// </summary>
        [Description("白云旋天")]
        BTXY = 13,

        /// <summary>
        /// 长空破日
        /// </summary>
        [Description("长空破日")]
        CKPR = 22,

        /// <summary>
        /// 霞光掩影
        /// </summary>
        [Description("霞光掩影")]
        XGYY = 44,

        /// <summary>
        /// 混沌沧海
        /// </summary>
        [Description("混沌沧海")]
        HDCH = 99,

        /// <summary>
        /// 梦境之末
        /// </summary>
        [Description("梦境之末")]
        MJZM = 100,

        /// <summary>
        /// 玄浑宇宙
        /// </summary>
        [Description("玄浑宇宙")]
        XHYZ = 120,

        /// <summary>
        /// 月神之眼
        /// </summary>
        [Description("月神之眼")]
        YSZY = 130
    }
}
