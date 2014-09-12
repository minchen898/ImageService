using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web.Services;

namespace ImageService
{
    public enum PositionOptions
    {
        LeftTop,
        LeftMiddle,
        LeftBottom,
        CenterTop,
        CenterMiddle,
        CenterBottom,
        RightTop,
        RightMiddle,
        RightBottom,
        Custom,
    }

    public enum ProcessOptions
    {
        CutFirst,
        ResizeFirst,
    }

    public class CutSetting
    {
        public Size RegionSize;
        public PositionOptions Position;
        public Point LeftTopPosition;
        public double? PreviewRate;
    }

    public class SizeSetting
    {
        public Size FrameSize;
        public ResizeOptions Option;
    }

    /// <summary>
    ///UploadImage 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class UploadImage : System.Web.Services.WebService
    {
        [WebMethod]
        public string UploadFile(byte[] image, CutSetting cutSetting, SizeSetting sizeSetting, ProcessOptions processOption)
        {
            return saveImage(processImage(image, cutSetting, sizeSetting, processOption)).ToString();
        }

        [WebMethod]
        public string UploadImageByUrl(string url, CutSetting cutSetting, SizeSetting sizeSetting, ProcessOptions processOption)
        {
            byte[] image;
            using (WebClient client = new WebClient())
            {
                image = client.DownloadData(Server.UrlDecode(url));
            }
            return saveImage(processImage(image, cutSetting, sizeSetting, processOption)).ToString();
        }

        public byte[] processImage(byte[] byteArray, CutSetting cutSetting, SizeSetting sizeSetting, ProcessOptions processOption)
        {
            byte[] result = null;

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                Image image = Image.FromStream(ms);

                if (processOption == ProcessOptions.ResizeFirst)
                {
                    if (sizeSetting != null && sizeSetting.FrameSize.Width > 0 && sizeSetting.FrameSize.Height > 0)
                    {
                        image = ImageProcessor.ResizeImage(image, sizeSetting.FrameSize, sizeSetting.Option);
                    }
                }

                if (cutSetting != null && cutSetting.RegionSize.Width > 0 && cutSetting.RegionSize.Height > 0)
                {
                    if (cutSetting.Position == PositionOptions.Custom)
                    {
                        Point point = cutSetting.LeftTopPosition;
                        double rate = 1.0;
                        if (point == null) point = new Point(0, 0);
                        if (cutSetting.PreviewRate != null) rate = cutSetting.PreviewRate.Value;
                        image = ImageProcessor.CutImage(image, new Rectangle(point, cutSetting.RegionSize), rate);
                    }
                    else
                    {
                        string positionOption = cutSetting.Position.ToString();
                        HorizonOptions horizonOption = HorizonOptions.Left;
                        VerticalOptions verticalOption = VerticalOptions.Top;

                        if (positionOption.Contains("Left")) horizonOption = HorizonOptions.Left;
                        else if (positionOption.Contains("Center")) horizonOption = HorizonOptions.Center;
                        else if (positionOption.Contains("Right")) horizonOption = HorizonOptions.Right;

                        if (positionOption.Contains("Top")) verticalOption = VerticalOptions.Top;
                        else if (positionOption.Contains("Middle")) verticalOption = VerticalOptions.Middle;
                        else if (positionOption.Contains("Bottom")) verticalOption = VerticalOptions.Bottom;

                        image = ImageProcessor.CutImage(image, cutSetting.RegionSize, horizonOption, verticalOption);
                    }
                }

                if (processOption == ProcessOptions.CutFirst)
                {
                    if (sizeSetting != null && sizeSetting.FrameSize.Width > 0 && sizeSetting.FrameSize.Height > 0)
                    {
                        image = ImageProcessor.ResizeImage(image, sizeSetting.FrameSize, sizeSetting.Option);
                    }
                }

                ImageConverter converter = new ImageConverter();
                result = (byte[])converter.ConvertTo(image, typeof(byte[]));
            }

            return result;
        }

        private int saveImage(byte[] image)
        {
            int imageId = 0;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ImageDB"].ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("sp_SaveImage", connection) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddWithValue("@ImageContent", image);
                SqlParameter returnValue = command.Parameters.Add("Return", SqlDbType.Int);
                returnValue.Direction = ParameterDirection.ReturnValue; 
                command.ExecuteNonQuery();

                imageId = (int)returnValue.Value;
                connection.Close();
            }

            return imageId;
        }
    }
}
