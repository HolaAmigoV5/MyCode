using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_SysManage
{
    /// <summary>
    /// AppIdȨ�ޱ�
    /// </summary>
    [Table("Base_PermissionAppId")]
    public class Base_PermissionAppId
    {

        /// <summary>
        /// ��������
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// AppId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Ȩ��ֵ
        /// </summary>
        public string PermissionValue { get; set; }

    }
}