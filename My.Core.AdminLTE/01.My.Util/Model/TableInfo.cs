namespace My.Util
{
    /// <summary>
    /// 描述：表信息
    /// 作者：wby 2019/9/24 14:05:42
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// 字段Id
        /// </summary>
        public int ColumnId { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string ColumnType { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        private string _columnDescription;
        public string ColumnDescription
        {
            get => _columnDescription.IsNullOrEmpty() ? ColumnName : _columnDescription;
            set => _columnDescription = value;
        }
    }
}
