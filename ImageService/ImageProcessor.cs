using System;
using System.Drawing;

namespace ImageService
{
    public enum HorizonOptions
    {
        Left,
        Center,
        Right,
    }

    public enum VerticalOptions
    {
        Top,
        Middle,
        Bottom,
    }

    public enum ResizeOptions
    {
        Fit,
        Fill,
    }

    public class ImageProcessor
    {
        public static double CalculateResizeRate(Image image, Size size, ResizeOptions option)
        {
            double widthRate = (double)size.Width / image.Size.Width;
            double heightRate = (double)size.Height / image.Size.Height;
            double result = 1.0;
            switch (option)
            {
                case ResizeOptions.Fit:
                    result = Math.Min(heightRate, widthRate);
                    break;
                case ResizeOptions.Fill:
                    result = Math.Max(heightRate, widthRate);
                    break;
            }
            return Math.Min(result, 1.0);
        }

        public static Image ResizeImage(Image source, Size size, ResizeOptions options)
        {
            double rate = CalculateResizeRate(source, size, options);
            Size newSize = new Size((int)(source.Width * rate), (int)(source.Height * rate));
            Bitmap bitmap = new Bitmap(source, newSize);
            return (Image)bitmap;
        }

        public static Image CutImage(Image source, Point point, Size destSize)
        {
            Bitmap bitmap= new Bitmap(destSize.Width, destSize.Height);
            using (Graphics graphic = Graphics.FromImage(bitmap))
            {
                graphic.DrawImage(source, new Rectangle(new Point(0, 0), destSize), new Rectangle(point, destSize), GraphicsUnit.Pixel);
            }
            return (Image)bitmap;
        }

        public static Image CutImage(Image source, Size destSize, HorizonOptions horizonOption, VerticalOptions verticalOption)
        {
            int x = 0, y = 0;

            switch (horizonOption)
            {
                case HorizonOptions.Left:
                    x = 0;
                    break;
                case HorizonOptions.Center:
                    x = (source.Width - destSize.Width) / 2;
                    break;
                case HorizonOptions.Right:
                    x = source.Width - destSize.Width;
                    break;
            }

            switch (verticalOption)
            {
                case VerticalOptions.Top:
                    y = 0;
                    break;
                case VerticalOptions.Middle:
                    y = (source.Height - destSize.Height) / 2;
                    break;
                case VerticalOptions.Bottom:
                    y = source.Height - destSize.Height;
                    break;
            }

            return CutImage(source, new Point(x, y), destSize);
        }

        public static Image CutImage(Image source, Rectangle selectedRegion, double previewRate)
        {
            Point point = new Point((int)(selectedRegion.X / previewRate), (int)(selectedRegion.Y / previewRate));
            Size size = new Size((int)(selectedRegion.Width / previewRate), (int)(selectedRegion.Height / previewRate));
            return CutImage(source, point, size);
        }

        public static Image GenerateTextImage(string drawingString, Font stringFont, Brush stringColor, bool isVertical)
        {
            Size stringSize = new Size();
            Bitmap stringImage = new Bitmap(1, 1);

            StringFormat stringFormat = new StringFormat();
            if (isVertical) stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;

            using (Graphics g = Graphics.FromImage(stringImage))
            {
                stringSize = g.MeasureString(drawingString, stringFont, new PointF(0, 0), stringFormat).ToSize();
            }

            stringImage = new Bitmap(stringSize.Width, stringSize.Height);
            using (Graphics g = Graphics.FromImage(stringImage))
            {
                g.DrawString(drawingString, stringFont, stringColor, new Point(0, 0), stringFormat);
            }

            return stringImage;
        }
    }
}