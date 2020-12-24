using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace My.Entity.Base_SysManage
{
    /// <summary>
    /// 描述：Base_UserRoleMap
    /// 作者：wby 2019/11/21 15:39:52
    /// </summary>
    [Table("Base_UserRoleMap")]
    public class Base_UserRoleMap
    {
        [Key]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string RoleId { get; set; }
    }
}
