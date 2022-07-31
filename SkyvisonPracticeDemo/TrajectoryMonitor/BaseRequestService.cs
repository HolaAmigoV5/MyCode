using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrajectoryMonitor
{
    public class BaseRequestService
    {
        public async Task<Response> RequestFromThirdPartyService<Response>
            (string route, object obj, Method method, bool isCamelCaseProperty = false) where Response : class
        {
            string pms = string.Empty;
            if (!string.IsNullOrWhiteSpace(obj?.ToString()))
            {
                var serializeSettings = new JsonSerializerSettings();
                IContractResolver contractResolver;

                if (isCamelCaseProperty)
                {
                    contractResolver = new CamelCasePropertyNamesContractResolver();
                }
                else
                {
                    contractResolver = new DefaultContractResolver();
                }

                serializeSettings.ContractResolver = contractResolver;
                pms = JsonConvert.SerializeObject(obj, serializeSettings);
            }

            Response result = await RequestBehavior<Response>(route, method, pms);
            return result;
        }

        private async Task<Response> RequestBehavior<Response>(string url, Method method, object pms,
            bool isToken = true, bool isJson = true, string token = "") where Response : class
        {
            try
            {
                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(method);

                if (isToken)
                {
                    _ = client.AddDefaultHeader("token", token);
                }

                request.AddHeader("Content-Type", "application/json");
                switch (method)
                {
                    case Method.GET:
                        break;
                    case Method.POST:
                    case Method.PUT:
                        if (isJson)
                        {
                            request.AddJsonBody(pms);
                        }
                        else
                        {
                            request.AddParameter("application/x-www-form-urlencoded", pms, ParameterType.RequestBody);
                            request.AddParameter("Content-Type", "application/x-www-form-urlencoded", ParameterType.HttpHeader);
                        }
                        break;
                    case Method.DELETE:
                        break;
                    default:
                        break;
                }

                IRestResponse response = await client.ExecuteAsync(request);
                return response.StatusCode == System.Net.HttpStatusCode.OK
                    ? JsonConvert.DeserializeObject<Response>(response.Content)
                    : new BaseResponse()
                    {
                        Code = (int)response.StatusCode,
                        Msg = response.StatusDescription ?? response.ErrorMessage
                    } as Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
