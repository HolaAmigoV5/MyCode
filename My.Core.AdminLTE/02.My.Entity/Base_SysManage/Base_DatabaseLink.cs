using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace My.Entity.Base_SysManage
{
    /// <summary>
    /// 描述：数据库连接
    /// 作者：wby 2019/11/25 15:03:36
    /// </summary>
    [Table("Base_DatabaseLink")]
    public class Base_DatabaseLink
    {
        /// <summary>
        /// 代理主键
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 连接名
        /// </summary>
        public string LinkName { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionStr { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 排序编号
        /// </summary>
        public string SortNum { get; set; }
    }
}
