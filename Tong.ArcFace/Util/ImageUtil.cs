using System;
using System.Drawing;

namespace Tong.ArcFace.Util
{
    public class ImageUtil
    {
        /// <summary>
        /// 按指定宽高缩放图片
        /// </summary>
        /// <param name="image">原图片</param>
        /// <param name="dstWidth">目标图片宽</param>
        /// <param name="dstHeight">目标图片高</param>
        /// <returns></returns>
        public static Image ScaleImage(Image image, int dstWidth, int dstHeight)
        {
            Graphics g = null;
            try
            {
                //按比例缩放           
                float scaleRate = GetWidthAndHeight(image.Width, image.Height, dstWidth, dstHeight);
                int width = (int)(image.Width * scaleRate);
                int height = (int)(image.Height * scaleRate);

                //将宽度调整为4的整数倍
                if (width % 4 != 0)
                {
                    width = width - width % 4;
                }

                Bitmap destBitmap = new Bitmap(width, height);
                g = Graphics.FromImage(destBitmap);
                g.Clear(Color.Transparent);

                //设置画布的描绘质量         
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, new Rectangle((width - width) / 2, (height - height) / 2, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

                return destBitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }
            }

            return null;
        }

        /// <summary>
        /// 获取图片缩放比例
        /// </summary>
        /// <param name="oldWidth">原图片宽</param>
        /// <param name="oldHeigt">原图片高</param>
        /// <param name="newWidth">目标图片宽</param>
        /// <param name="newHeight">目标图片高</param>
        /// <returns></returns>
        public static float GetWidthAndHeight(int oldWidth, int oldHeigt, int newWidth, int newHeight)
        {
            //按比例缩放           
            float scaleRate = 0.0f;
            if (oldWidth >= newWidth && oldHeigt >= newHeight)
            {
                int widthDis = oldWidth - newWidth;
                int heightDis = oldHeigt - newHeight;
                if (widthDis > heightDis)
                {
                    scaleRate = newWidth * 1f / oldWidth;
                }
                else
                {
                    scaleRate = newHeight * 1f / oldHeigt;
                }
            }
            else if (oldWidth >= newWidth && oldHeigt < newHeight)
            {
                scaleRate = newWidth * 1f / oldWidth;
            }
            else if (oldWidth < newWidth && oldHeigt >= newHeight)
            {
                scaleRate = newHeight * 1f / oldHeigt;
            }
            else
            {
                int widthDis = newWidth - oldWidth;
                int heightDis = newHeight - oldHeigt;
                if (widthDis > heightDis)
                {
                    scaleRate = newHeight * 1f / oldHeigt;
                }
                else
                {
                    scaleRate = newWidth * 1f / oldWidth;
                }
            }
            return scaleRate;
        }

        /// <summary>
        /// 剪裁图片
        /// </summary>
        /// <param name="image">原图片</param
        /// <param name="rectangle">裁剪大小</param>
        /// <returns>剪裁后的图片</returns>
        public static Image CutImage(Image image, Rectangle rectangle)
        {
            try
            {
                Bitmap srcBitmap = new Bitmap(image);
                Bitmap destBitmap = new Bitmap(rectangle.Width, rectangle.Height);
                using (Graphics g = Graphics.FromImage(destBitmap))
                {
                    g.Clear(Color.Transparent);

                    //设置画布的描绘质量         
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(srcBitmap, new Rectangle(0, 0, rectangle.Width, rectangle.Height), rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height, GraphicsUnit.Pixel);
                }

                return destBitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
    }
}
