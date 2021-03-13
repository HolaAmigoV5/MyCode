using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Wby.Demo.Shared.DataModel
{
    /// <summary>
    /// 用户个性化配置
    /// </summary>
    public class UserConfig : BaseEntity
    {
        /// <summary>
        /// 账户名
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 预计支出
        /// </summary>
        [Column(TypeName= "decimal(18,4)")]
        public decimal ExpectedOut { get; set; }

        /// <summary>
        /// 预计收入
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal ExpectedIn { get; set; }
    }
}
