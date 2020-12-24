namespace My.Repository
{
    /// <summary>
    /// 描述：分片规则接口
    /// 作者：wby 2019/11/15 15:31:58
    /// </summary>
    public interface IShardingRule
    {
        /// <summary>
        /// 找表规则
        /// </summary>
        /// <param name="obj">实体对象</param>
        /// <returns></returns>
        string FindTable(object obj);
    }
}
