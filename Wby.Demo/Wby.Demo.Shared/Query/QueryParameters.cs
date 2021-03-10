namespace Wby.Demo.Shared.Query
{
    /// <summary>
    /// 搜索基类
    /// </summary>
    public class QueryParameters
    {
        private int _pageIndex = 0;

        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        private int _pageSize = 10;

        public virtual int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public string Search { get; set; }
    }
}
