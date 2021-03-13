using System;
using System.Windows.Media.Imaging;

namespace Wby.Demo.PC.Common
{
    /// <summary>
    /// 图标操作类
    /// </summary>
    public class ImageHelper
    {
        public static BitmapImage ConvertToImage(string fileName)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(fileName);
            bmp.EndInit();
            bmp.Freeze();

            return bmp;
        }
    }
}
