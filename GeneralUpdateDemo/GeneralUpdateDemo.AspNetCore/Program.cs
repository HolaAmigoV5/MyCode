
using GeneralUpdate.AspNetCore.DTO;
using GeneralUpdate.AspNetCore.Hubs;
using GeneralUpdate.AspNetCore.Services;
using GeneralUpdate.Core.Domain.DTO;
using GeneralUpdate.Core.Domain.Enum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace GeneralUpdateDemo.AspNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<IUpdateService, GeneralUpdateService>();
            builder.Services.AddSignalR();
            var app = builder.Build();

            // Push the latest version information in real time. 推送功能
            app.MapHub<VersionHub>("/versionhub");

            app.MapPost("/push", async Task<string> (HttpContext context) =>
            {
                try
                {
                    var hubContext = context.RequestServices.GetRequiredService<IHubContext<VersionHub>>();
                    await hubContext.SendMessage("TESTNAME", "123");
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                return "OK";
            });

            // Check if an update is required. 更新功能
            app.MapGet("/versions/{clientType}/{clientVersion}/{clientAppKey}", (int clientType, string clientVersion, string clientAppKey, IUpdateService updateService) =>
            {
                var versions = new List<VersionDTO>();
                var md5 = "b03d52c279faf003965c46041f2037f9";//生成好的更新包文件的MD5码，因为返回给客户端的时候需要同这个来验证是否可用
                var pubTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                string? version = null;
                if (clientType == AppType.ClientApp)
                {
                    //client
                    //version = "0.0.0.0";
                    version = "9.9.9.9";//这里设置为9是让程序认为需要更新
                }
                else if (clientType == AppType.UpgradeApp)
                {
                    //upgrad
                    //version = "0.0.0.0";
                    version = "9.9.9.9"; //这里设置为9是让程序认为需要更新
                }
                var url = $"http://192.168.50.203/testpacket.zip";//更新包的下载地址
                var name = "testpacket";
                versions.Add(new VersionDTO(md5, pubTime, version, url, name));
                return updateService.Update(clientType, clientVersion, version, clientAppKey, GetAppSecretKey(), false, versions);
            });

            // Upload update package. 上传更新包
            app.MapPost("/upload", async Task<string> (HttpContext context, HttpRequest request) =>
            {
                var uploadReapDTO = new UploadReapDTO();
                try
                {
                    var contextReq = context.Request;
                    int.TryParse(contextReq.Form["clientType"], out int clientType);
                    var version = contextReq.Form["clientType"].ToString();
                    var clientAppKey = contextReq.Form["clientAppKey"].ToString();
                    var md5 = contextReq.Form["md5"].ToString();

                    if (!request.HasFormContentType) throw new Exception("ContentType was not included in the request !");
                    var form = await request.ReadFormAsync();

                    var formFile = form.Files["file"];
                    if (formFile is null || formFile.Length == 0) throw new ArgumentNullException("Uploaded update package file not found !");
                    await using var stream = formFile.OpenReadStream();
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);

                    //TODO: save to file server
                    string localPath = $"D:\\{formFile.FileName}";
                    await using var fileStream = new FileStream(localPath, FileMode.CreateNew, FileAccess.Write);
                    fileStream.Write(buffer, 0, buffer.Length);

                    //TODO: data persistence to mysql, sqlserver ...


                    uploadReapDTO.Code = HttpStatus.OK;
                    uploadReapDTO.Body = "Published successfully.";
                    uploadReapDTO.Message = RespMessage.RequestSucceeded;

                    return JsonConvert.SerializeObject(uploadReapDTO);
                }
                catch (Exception ex)
                {
                    uploadReapDTO.Code = HttpStatus.BAD_REQUEST;
                    uploadReapDTO.Body = $"Failed to publish ! Because : {ex.Message}";
                    uploadReapDTO.Message = RespMessage.RequestFailed;
                    return JsonConvert.SerializeObject(uploadReapDTO);
                }
            });

            app.Run();

            string GetAppSecretKey()
            {
                return "41A54379-C7D6-4920-8768-21A3468572E5";
            }
        }
    }
}