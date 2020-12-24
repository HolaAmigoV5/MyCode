namespace My.Util
{
    /// <summary>
    /// 描述：数据库所有表信息
    /// 作者：wby 2019/9/24 13:57:22
    /// </summary>
    public class DbTableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表描述说明
        /// </summary>
        private string _description;
        public string Description
        {
            get => _description.IsNullOrEmpty() ? TableName : _description;
            set => _description = value;
        }
    }
}
