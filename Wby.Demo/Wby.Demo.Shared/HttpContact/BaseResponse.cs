namespace Wby.Demo.Shared.HttpContact
{
    public class BaseResponse
    {
        public string Message { get; set; }

        public int StatusCode { get; set; }
        public object Result { get; set; }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public new T Result { get; set; }
    }
}
