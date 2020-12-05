using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_SysManage
{
    /// <summary>
    /// 角色权限表
    /// </summary>
    [Table("Base_PermissionRole")]
    public class Base_PermissionRole
    {

        /// <summary>
        /// 逻辑主键
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 角色主键Id
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 权限值
        /// </summary>
        public string PermissionValue { get; set; }

    }
}