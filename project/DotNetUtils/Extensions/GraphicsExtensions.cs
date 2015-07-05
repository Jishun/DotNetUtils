using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DotNetUtils
{
    public static class GraphicsExtensions
    {
        public static void SaveJpegWithQualitySetting(this Image image, Stream stream, long quality = 95)
        {
            if (quality <= 5 || quality > 100)
            {
                throw new ArgumentOutOfRangeException("quality");
            }
            ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(cd => cd.MimeType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase));
            if (codec == null)
            {
                image.Save(stream, ImageFormat.Jpeg);
            }
            else
            {
                EncoderParameters encParams = new EncoderParameters(1);
                encParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                image.Save(stream, codec, encParams);
            }
        }

        public static void DrawStringAlignRightCenter(this Graphics gfx, string str, Font font, Brush brush, float right, float top, float height)
        {
            CheckParameters(gfx, str, font, brush);
            var size = gfx.MeasureString(str, font);
            gfx.DrawString(str, font, brush, new PointF(right - size.Width, top + (height - size.Height) / 2));
        }

        public static void DrawStringAlignRightCenterAutoSpacing(this Graphics gfx, string str, Font font, Brush brush, float right, float top, float height, float width)
        {
            CheckParameters(gfx, str, font, brush);
            var size = gfx.MeasureString(str, font);
            if (size.Width <= width)
            {
                gfx.DrawString(str, font, brush, new PointF(right - size.Width, top + (height - size.Height) / 2));
                return;
            }
            DrawStringAutoSpacing(gfx, str, font, brush, new PointF(right - width, top), width);
        }

        public static void DrawStringAlignLeftCenter(this Graphics gfx, string str, Font font, Brush brush, float left, float top, float height)
        {
            CheckParameters(gfx, str, font, brush);
            var size = gfx.MeasureString(str, font);
            gfx.DrawString(str, font, brush, new PointF(left, top + (height - size.Height) / 2));
        }

        public static void DrawStringAlignCenter(this Graphics gfx, string str, Font font, Brush brush, float left, float top, float width, float height)
        {
            CheckParameters(gfx, str, font, brush);
            var size = gfx.MeasureString(str, font);
            gfx.DrawString(str, font, brush, new PointF(left + (width - size.Width) / 2, top + (height - size.Height) / 2));
        }

        public static void DrawStringAlignVerticalCenter(this Graphics gfx, string str, Font font, Brush brush, float left, float top, float height)
        {
            CheckParameters(gfx, str, font, brush);
            var size = gfx.MeasureString(str, font);
            gfx.DrawString(str, font, brush, new PointF(left, top + (height - size.Height) / 2));
        }

        private static void DrawStringAutoSpacing(this Graphics gfx, string text, Font font, Brush brush, PointF point, float widthLimit)
        {
            //Calculate spacing
            float widthNeeded = text.Sum(c => gfx.MeasureString(c.ToString(CultureInfo.InvariantCulture), font).Width);
            float spacing = (widthLimit - widthNeeded) / (text.Length - 1);
            //draw text
            float indent = 0;
            foreach (char c in text)
            {
                gfx.DrawString(c.ToString(CultureInfo.InvariantCulture), font, brush, new PointF(point.X + indent, point.Y));
                indent += gfx.MeasureString(c.ToString(CultureInfo.InvariantCulture), font).Width + spacing;
            }
        }

        private static void CheckParameters(Graphics gfx, string str, Font font, Brush brush)
        {
            if (gfx == null)
            {
                throw new ArgumentNullException("gfx");
            }
            if (str.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("str");
            }
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
        }
    }
}
