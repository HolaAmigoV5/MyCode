using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace My.Entity.Base_SysManage
{
    /// <summary>
    /// 描述：角色权限表
    /// 作者：wby 2019/11/20 10:32:07
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
        /// 角色主键
        /// </summary>
        public string  RoleId { get; set; }

        /// <summary>
        /// 权限值
        /// </summary>
        public string PermissionValue { get; set; }
    }
}
