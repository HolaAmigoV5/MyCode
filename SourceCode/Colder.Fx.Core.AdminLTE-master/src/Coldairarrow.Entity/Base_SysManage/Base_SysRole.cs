using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_SysManage
{
    /// <summary>
    /// ϵͳ��ɫ
    /// </summary>
    [Table("Base_SysRole")]
    public class Base_SysRole
    {
        /// <summary>
        /// ��ɫId����
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// ��ɫ��
        /// </summary>
        public string RoleName { get; set; }
    }
}