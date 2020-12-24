namespace My.Util
{
    /// <summary>
    /// 描述：Ajax请求结果
    /// 作者：wby 2019/10/12 10:51:54
    /// </summary>
    public class AjaxResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误代码
        /// 1：未登录
        /// 其他待定义
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }
    }
}
