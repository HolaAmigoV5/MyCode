using Microsoft.AspNetCore.Http;

namespace My.Util
{
    /// <summary>
    /// 描述：HttpContextCore
    /// 作者：wby 2019/10/12 15:23:42
    /// </summary>
    public static class HttpContextCore
    {
        public static HttpContext Current { get => AutofacHelper.GetService<IHttpContextAccessor>().HttpContext; }
    }
}
