using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace My.Entity.Base_SysManage
{
    /// <summary>
    /// 描述：系统角色
    /// 作者：wby 2019/11/20 9:36:56
    /// </summary>
    [Table("Base_SysRole")]
    public class Base_SysRole
    {
        /// <summary>
        /// 角色Id主键
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }
    }
}
