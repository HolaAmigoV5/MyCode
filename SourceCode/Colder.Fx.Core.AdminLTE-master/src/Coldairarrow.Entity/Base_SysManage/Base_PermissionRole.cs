using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_SysManage
{
    /// <summary>
    /// ��ɫȨ�ޱ�
    /// </summary>
    [Table("Base_PermissionRole")]
    public class Base_PermissionRole
    {

        /// <summary>
        /// �߼�����
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// ��ɫ����Id
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// Ȩ��ֵ
        /// </summary>
        public string PermissionValue { get; set; }

    }
}