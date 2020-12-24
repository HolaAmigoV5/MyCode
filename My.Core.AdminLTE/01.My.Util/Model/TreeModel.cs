using System.Collections.Generic;

namespace My.Util
{
    /// <summary>
    /// 描述：树模型（可以作为父类）
    /// 作者：wby 2019/10/25 14:19:30
    /// </summary>
    public class TreeModel
    {
        /// <summary>
        /// 唯一标识符Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 数据值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 父Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 节点深度
        /// </summary>
        public int? Level { get; set; } = 1;

        /// <summary>
        /// 显示内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<object> Children { get; set; }
    }
}
