using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wby.Demo.Shared.Collections
{
    public class PagedList<T> : IPagedList<T>
    {
        /// <summary>
        /// 起始页
        /// </summary>
        public int IndexFrom { get; set; }

        /// <summary>
        /// 目标页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 条目总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 获取页数据
        /// </summary>
        public IList<T> Items { get; set; }


        /// <summary>
        /// 是否有前一页
        /// </summary>
        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        /// <summary>
        /// 是否有后一页
        /// </summary>
        public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

        public PagedList()
        {
            Items = new T[0];
        }

        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom)
        {
            if (indexFrom > pageIndex)
                throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");

            PageIndex = pageIndex;
            PageSize = PageSize;
            IndexFrom = indexFrom;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

            Items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();

            //if (source is IQueryable<T> querable)
            //{
            //    Items = querable.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
            //}
            //else
            //{
            //    Items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
            //}
        }
    }

    internal class PagedList<TSource, TResult> : PagedList<TResult>
    {
        public PagedList(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            PageIndex = source.PageIndex;
            PageSize = source.PageSize;
            IndexFrom = source.IndexFrom;
            TotalCount = source.TotalCount;
            TotalPages = source.TotalPages;

            Items = new List<TResult>(converter(source.Items));
        }

        public PagedList(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter,
            int pageIndex, int pageSize, int indexFrom)
        {
            if (indexFrom > pageIndex)
                throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");

            PageIndex = pageIndex;
            PageSize = pageSize;
            IndexFrom = indexFrom;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            var items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

            Items = new List<TResult>(converter(items));
        }
    }
}
