using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;
using Wby.Demo.Shared.Common;
using Wby.Demo.Shared.HttpContact;

namespace Wby.Demo.Service
{
    public class BaseServiceRequest
    {
        private readonly string _requestUrl = Contract.ServerUrl;

        public string RequestUrl
        {
            get { return _requestUrl; }
        }

        public RestSharpCertificateMethod restSharp = new RestSharpCertificateMethod();

        /// <summary>
        /// 请求
        /// </summary>
        /// <typeparam name="Response"></typeparam>
        /// <param name="request">参数</param>
        /// <param name="method">方法</param>
        /// <returns></returns>
        public async Task<Response> GetRequest<Response>(BaseRequest request, Method method) where Response : class
        {
            string pms = request.GetPropertiesObject();
            string url = RequestUrl + request.Route;
            if (!string.IsNullOrWhiteSpace(request.GetParameter))
                url += request.GetParameter;
            Response result = await restSharp.RequestBehavior<Response>(url, method, pms);
            return result;
        }

        public async Task<Response> GetRequest<Response>(string route, object obj, Method method) where Response : class
        {
            string pms = string.Empty;
            if (string.IsNullOrWhiteSpace(obj?.ToString()))
                pms = JsonConvert.SerializeObject(obj);
            Response result = await restSharp.RequestBehavior<Response>(RequestUrl + route, method, pms);
            return result;
        }
    }
}
