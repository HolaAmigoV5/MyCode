using QRCoder;
using System.Drawing;

namespace My.Util.Helper
{
    /// <summary>
    /// 描述：二维码生成帮助类
    /// 作者：wby 2019/10/25 13:29:11
    /// </summary>
    public class QRCodeHelper
    {
        /// <summary>
        /// 生成二维码，默认边长为250px
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <returns></returns>
        public static Image BuildQRCode(string content)
        {
            return BuildQRCode(content, 250, Color.White, Color.Black);
        }

        /// <summary>
        /// 生成二维码,自定义边长
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <param name="imgSize">二维码边长px</param>
        /// <returns></returns>
        public static Image BuildQRCode(string content,int imgSize)
        {
            return BuildQRCode(content, imgSize, Color.White, Color.Black);
        }

        /// <summary>
        /// 生成二维码
        /// 注：自定义边长以及颜色
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <param name="imgSize">二维码边长px</param>
        /// <param name="background">二维码底色</param>
        /// <param name="foreground">二维码前景色</param>
        /// <returns></returns>
        public static Image BuildQRCode(string content,int imgSize,Color background,Color foreground)
        {
            return BuildQRCode_Logo(content, imgSize, background, foreground, null);
        }

        /// <summary>
        /// 生成二维码并添加Logo
        /// 注：默认生成边长为250px的二维码
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <param name="logo">logo图片</param>
        /// <returns></returns>
        public static Image BuildQRCode_Logo(string content,Bitmap logo)
        {
            return BuildQRCode_Logo(content, 250, Color.White, Color.Black, logo);
        }

        /// <summary>
        /// 生成二维码并添加LOGO
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <param name="imgSize">二维码边长</param>
        /// <param name="background">二维码底色</param>
        /// <param name="foreground">二维码前景色</param>
        /// <param name="logo">logo图片</param>
        /// <returns></returns>
        public static Image BuildQRCode_Logo(string content,int imgSize,Color background,Color foreground,Bitmap logo)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            var ppm = imgSize / qrCodeData.ModuleMatrix.Count;
            Bitmap qrCodeImage = qrCode.GetGraphic(ppm, background, foreground, logo);

            return qrCodeImage;
        }
    }
}
