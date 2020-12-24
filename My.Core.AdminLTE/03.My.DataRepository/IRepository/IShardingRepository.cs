namespace My.Repository
{
    public interface IShardingRepository: IBaseRepository
    {
        /// <summary>
        /// 获取IShardingQueryable
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <returns></returns>
        IShardingQueryable<T> GetIShardingQueryable<T>() where T : class, new();
    }
}
