namespace Wby.Demo.Shared.Query
{
    /// <summary>
    /// 搜索基类
    /// </summary>
    public class QueryParameters
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;

        public string Search { get; set; }
    }
}
