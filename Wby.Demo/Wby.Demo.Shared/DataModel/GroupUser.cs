using System;
using System.Collections.Generic;
using System.Text;

namespace Wby.Demo.Shared.DataModel
{
    /// <summary>
    /// 组用户
    /// </summary>
    public class GroupUser : BaseEntity
    {
        /// <summary>
        /// 组代码
        /// </summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
    }
}
