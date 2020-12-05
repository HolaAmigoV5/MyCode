using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_SysManage
{
    /// <summary>
    /// ��Ԫ���Ա�
    /// </summary>
    [Table("Base_UnitTest")]
    public class Base_UnitTest
    {

        /// <summary>
        /// ��������
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Age
        /// </summary>
        public int? Age { get; set; }

    }
}