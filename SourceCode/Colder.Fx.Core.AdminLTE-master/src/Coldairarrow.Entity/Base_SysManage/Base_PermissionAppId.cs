using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_SysManage
{
    /// <summary>
    /// AppId权限表
    /// </summary>
    [Table("Base_PermissionAppId")]
    public class Base_PermissionAppId
    {

        /// <summary>
        /// 代理主键
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// AppId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 权限值
        /// </summary>
        public string PermissionValue { get; set; }

    }
}