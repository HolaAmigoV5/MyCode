using System;

namespace My.Util
{
    /// <summary>
    /// 描述：GUID帮助类
    /// 作者：wby 2019/10/24 9:07:09
    /// </summary>
    public static class GuidHelper
    {
        /// <summary>
        /// 生成主键
        /// </summary>
        /// <returns></returns>
        public static string GenerateKey()
        {
            return Guid.NewGuid().ToSequentialGuid().ToUpper();
        }
    }
}
