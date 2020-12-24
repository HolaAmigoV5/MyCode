using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace My.Entity.Base_SysManage
{
    /// <summary>
    /// 描述：应用密钥
    /// 作者：wby 2019/11/25 14:21:36
    /// </summary>
    [Table("Base_AppSecret")]
    public class Base_AppSecret
    {
        /// <summary>
        /// 代理主键
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 应用密钥
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 应用名
        /// </summary>
        public string AppName { get; set; }
    }
}
