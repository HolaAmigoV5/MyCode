using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace My.Entity.Base_SysManage
{
    /// <summary>
    /// 描述：部门表
    /// 作者：wby 2019/11/21 16:33:41
    /// </summary>
    [Table("Base_Department")]
    public class Base_Department
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, Column(Order = 1)]
        public string Id { get; set; }

        /// <summary>
        /// 部门名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 上级部门Id
        /// </summary>
        public string ParentId { get; set; }
    }
}
