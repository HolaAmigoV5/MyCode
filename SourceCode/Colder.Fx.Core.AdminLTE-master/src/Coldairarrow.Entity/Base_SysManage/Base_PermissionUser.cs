using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_SysManage
{
    /// <summary>
    /// �û�Ȩ�ޱ�
    /// </summary>
    [Table("Base_PermissionUser")]
    public class Base_PermissionUser
    {

        /// <summary>
        /// ��������
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// �û�����Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Ȩ��
        /// </summary>
        public string PermissionValue { get; set; }

    }
}