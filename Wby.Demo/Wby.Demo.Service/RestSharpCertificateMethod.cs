using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;
using Wby.Demo.Shared.Common.Aop;
using Wby.Demo.Shared.HttpContact;

namespace Wby.Demo.Service
{
    /// <summary>
    /// 请求服务基类
    /// </summary>
    public class RestSharpCertificateMethod
    {
        /// <summary>
        /// 请求数据
        /// </summary>
        /// <typeparam name="Response"></typeparam>
        /// <param name="url">地址</param>
        /// <param name="method">请求方法</param>
        /// <param name="pms">参数</param>
        /// <param name="isToken">是否Token</param>
        /// <param name="isJson">是否Json</param>
        /// <returns></returns>
        [GlobalLoger]
        public async Task<Response> RequestBehavior<Response>(string url, Method method, string pms,
            bool isToken = true, bool isJson = true) where Response : class
        {
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(method);
            if (isToken)
                client.AddDefaultHeader("token", "");

            request.AddHeader("Content-Type", "application/json");
            switch (method)
            {
                case Method.GET:
                    break;
                case Method.POST:
                    request.AddHeader("Content-Type", "application/json");
                    if (isJson)
                        request.AddJsonBody(pms);
                    else
                        request.AddParameter("application/x-www-form-urlencoded",
                           pms, ParameterType.RequestBody);
                    break;
                case Method.PUT:
                    break;
                case Method.DELETE:
                    break;
                default:
                    break;
            }
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Response>(response.Content);
            else
                return new BaseResponse()
                {
                    StatusCode = (int)response.StatusCode,
                    Message = response.StatusDescription ?? response.ErrorMessage
                } as Response;


        }
    }
}
