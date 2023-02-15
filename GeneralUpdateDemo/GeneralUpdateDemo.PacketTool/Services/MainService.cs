using GeneralUpdateDemo.Infrastructure.DataServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeneralUpdateDemo.PacketTool.Services
{
    public class MainService
    {
        public async Task PostUpgradPakcet<T>(string remoteUrl, string filePath, int clientType, string version, string clientAppKey,string md5,Action<T?> reponseCallback) where T : class 
        {
            if(string.IsNullOrEmpty(remoteUrl)) remoteUrl = "http://127.0.0.1:5001/upload";
            var parameters = new Dictionary<string, string>
            {
                { "clientType", clientType.ToString() },
                { "version", version },
                { "clientAppKey", clientAppKey },
                { "md5", md5 }
            };
            await HttpService.Instance.PostFileRequest(remoteUrl, parameters, filePath, reponseCallback);
        }
    }
}
