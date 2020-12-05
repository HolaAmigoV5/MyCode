using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_SysManage
{
    /// <summary>
    /// Base_UserRoleMap
    /// </summary>
    [Table("Base_UserRoleMap")]
    public class Base_UserRoleMap
    {

        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// RoleId
        /// </summary>
        public string RoleId { get; set; }

    }
}