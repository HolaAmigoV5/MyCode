using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace My.Entity.Base_SysManage
{
    /// <summary>
    /// 描述：用户权限表
    /// 作者：wby 2019/11/21 15:19:55
    /// </summary>
    [Table("Base_PermissionUser")]
    public class Base_PermissionUser
    {
        /// <summary>
        /// 代理主键
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 用户主键Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public string PermissionValue { get; set; }
    }
}
