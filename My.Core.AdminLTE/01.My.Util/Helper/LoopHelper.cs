using System;

namespace My.Util
{
    /// <summary>
    /// 描述：循环帮助类
    /// 作者：wby 2019/10/25 9:58:20
    /// </summary>
    public class LoopHelper
    {
        /// <summary>
        /// 循环指定次数执行
        /// </summary>
        /// <param name="count">循环次数</param>
        /// <param name="method">执行方法</param>
        public static void Loop(int count,Action method)
        {
            for (int i = 0; i < count; i++)
            {
                method();
            }
        }

        /// <summary>
        /// 循环指定次数
        /// </summary>
        /// <param name="count">循环次数</param>
        /// <param name="method">执行的方法</param>
        public static void Loop(int count, Action<int> method)
        {
            for (int i = 0; i < count; i++)
            {
                method(i);
            }
        }
    }
}
