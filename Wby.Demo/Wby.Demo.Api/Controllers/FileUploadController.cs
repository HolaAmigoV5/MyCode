using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wby.Demo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public FileUploadController(IWebHostEnvironment webHost)
        {
            hostEnvironment = webHost;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles()
        {
            try
            {
                List<IFormFile> formFiles = new List<IFormFile>();
                if (Request != null && Request.Form != null && Request.Form.Files != null && Request.Form.Files.Count > 0)
                    formFiles.AddRange(Request.Form.Files);

                long size = formFiles.Sum(f => f.Length);

                DateTime dateTime = DateTime.Now;
                string basePath = $"\\Files\\{dateTime:yyyy:MM:dd}\\";
                string filePath = hostEnvironment.WebRootPath + basePath;

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                var filePathResultList = new List<string>();
                foreach (var formFile in formFiles)
                {
                    if (formFile.Length > 0)
                    {
                        string fileExt = Path.GetExtension(formFile.FileName);
                        string fileName = Guid.NewGuid().ToString() + fileExt;
                        string fileFullName = filePath + fileName;

                        using (var stream = new FileStream(fileFullName, FileMode.Create))
                            await formFile.CopyToAsync(stream);

                        filePathResultList.Add($"/Files/{dateTime:yyyy:MM:dd}/{fileName}");
                    }
                }

                string message = $"{formFiles.Count}个文件上传成功！";
                return Ok((Success: true, Message: message, FilePathList: filePathResultList));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
