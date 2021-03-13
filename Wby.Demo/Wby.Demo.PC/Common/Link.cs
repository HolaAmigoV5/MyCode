using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Wby.Demo.PC.Common
{
    /// <summary>
    /// 配置启动超链接
    /// </summary>
    public static class Link
    {
        public static void OpenInBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
        }
    }
}
