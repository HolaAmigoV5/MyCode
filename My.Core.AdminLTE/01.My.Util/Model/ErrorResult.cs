namespace My.Util
{
    /// <summary>
    /// 描述：异常结果类
    /// 作者：wby 2019/10/25 15:30:12
    /// </summary>
    public class ErrorResult: AjaxResult
    {
        public ErrorResult(string msg=null)
        {
            Msg = msg;
            Success = false;
        }
    }
}
