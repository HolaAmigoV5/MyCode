using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace My.Entity.Base_SysManage
{
    /// <summary>
    /// 描述：AppId权限表
    /// 作者：wby 2019/11/20 14:24:35
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
