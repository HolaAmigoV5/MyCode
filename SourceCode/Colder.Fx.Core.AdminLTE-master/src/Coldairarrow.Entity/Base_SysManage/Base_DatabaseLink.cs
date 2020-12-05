using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_SysManage
{
    /// <summary>
    /// ���ݿ�����
    /// </summary>
    [Table("Base_DatabaseLink")]
    public class Base_DatabaseLink
    {

        /// <summary>
        /// ��������
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public string LinkName { get; set; }

        /// <summary>
        /// �����ַ���
        /// </summary>
        public string ConnectionStr { get; set; }

        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public string SortNum { get; set; }

    }
}