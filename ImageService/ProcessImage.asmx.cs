using System.Drawing;
using System.IO;
using System.Net;
using System.Web.Services;

namespace ImageService
{
    /// <summary>
    ///ProcessImage 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class ProcessImage : System.Web.Services.WebService
    {
        [WebMethod]
        public double CalculateResizeRate(string imageUrl, Size size, ResizeOptions resizeOption)
        {
            double rate = 1.0;
            byte[] imageContent;
            using (WebClient client = new WebClient())
            {
                imageContent = client.DownloadData(Server.UrlDecode(imageUrl));
            }
            using (MemoryStream ms = new MemoryStream(imageContent))
            {
                Image image = Image.FromStream(ms);
                rate = ImageProcessor.CalculateResizeRate(image, size, resizeOption);
            }
            return rate;
        }

        [WebMethod]
        public byte[] GenerateTextImage(string text, string stringFont, int fontSize, FontStyle[] fontStyles, string stringColor, Size imageSize, bool isVertical)
        {
            fontSize = (fontSize == 0 ? 1024 : fontSize);

            Color color = new Color();
            try
            {
                color = ColorTranslator.FromHtml(stringColor);
            }
            catch { }

            FontStyle styles = FontStyle.Regular;
            if (fontStyles != null)
            {
                foreach (FontStyle style in fontStyles)
                {
                    styles |= style;
                }
            }

            Image image = ImageProcessor.GenerateTextImage(text, new Font(stringFont, fontSize, styles), new SolidBrush(color), isVertical);

            if (imageSize != null && imageSize.Width > 0 && imageSize.Height > 0)
            {
                image = new Bitmap(image, imageSize);
            }

            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }
    }
}
