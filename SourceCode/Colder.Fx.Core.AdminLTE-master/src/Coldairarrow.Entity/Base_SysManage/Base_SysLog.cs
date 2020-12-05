using Nest;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_SysManage
{
    /// <summary>
    /// ϵͳ��־��
    /// </summary>
    [Table("Base_SysLog")]
    public class Base_SysLog
    {

        /// <summary>
        /// ��������
        /// </summary>
        [Key]
        [Keyword]
        public string Id { get; set; }

        /// <summary>
        /// ��־����
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// ��־����
        /// </summary>
        [Keyword]
        public string LogType { get; set; }

        /// <summary>
        /// ��־����
        /// </summary>
        [Keyword]
        public string LogContent { get; set; }

        /// <summary>
        /// ����Ա�û���
        /// </summary>
        [Keyword]
        public string OpUserName { get; set; }

        /// <summary>
        /// ��־��¼ʱ��
        /// </summary>
        [Keyword]
        public DateTime? OpTime { get; set; }

        /// <summary>
        /// ���ݱ��ݣ�תΪJSON�ַ�����
        /// </summary>
        public string Data { get; set; }
    }
}