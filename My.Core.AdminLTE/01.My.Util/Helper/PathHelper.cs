using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.IO;

namespace My.Util
{
    /// <summary>
    /// 描述：路径帮助类
    /// 作者：wby 2019/10/25 10:10:07
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// 获取URL
        /// </summary>
        /// <param name="virtualUrl">虚拟URL</param>
        /// <returns></returns>
        public static string GetUrl(string virtualUrl)
        {
            if (virtualUrl.IsNullOrEmpty())
                throw new ArgumentNullException("虚拟Url不能为NULL或者空!");
            else
            {
                UrlHelper urlHelper = new UrlHelper(AutofacHelper.GetScopeService<IActionContextAccessor>().ActionContext);
                return urlHelper.Content(virtualUrl);
            }
        }

        /// <summary>
        /// 获取绝对路径
        /// </summary>
        /// <param name="virtualPath">虚拟路径</param>
        /// <returns></returns>
        public static string GetAbsolutePath(string virtualPath)
        {
            string path = virtualPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (path[0] == '~')
                path = path.Remove(0, 2);
            string rootPath = AutofacHelper.GetScopeService<IHostingEnvironment>().WebRootPath;

            return Path.Combine(rootPath, path);
        }
    }
}
