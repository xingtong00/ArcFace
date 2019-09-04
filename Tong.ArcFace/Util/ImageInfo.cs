using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Tong.ArcFace.ArcEnum;
using Tong.ArcFace.Common;

namespace Tong.ArcFace.Util
{
    public class ImageInfo : IDisposable
    {
        #region 属性

        /// <summary>
        /// 图像数据
        /// </summary>
        public IntPtr ImageData { get; set; }

        /// <summary>
        /// 图像
        /// </summary>
        public Image<Bgr, byte> Image { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public CameraType CameraType { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public int Width
        {
            get { return Image.Width; }
        }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height
        {
            get { return Image.Height; }
        }

        /// <summary>
        /// 图片格式
        /// </summary>
        public ImagePixelFormat Format
        {
            get
            {
                if (CameraType == CameraType.Bgr)
                    return ImagePixelFormat.RGB24_B8G8R8;
                return ImagePixelFormat.PAF_GRAY;
            }
        }

        #endregion

        #region 构造函数

        public ImageInfo(Image image, CameraType cameraType = CameraType.Bgr) : this(new Bitmap(image), cameraType)
        {
        }

        public ImageInfo(Bitmap bitmap, CameraType cameraType = CameraType.Bgr)
        {
            bitmap = AdjustToRecognitionSize(bitmap);
            Image = new Image<Bgr, byte>(bitmap);
            CameraType = cameraType;
            ReadImage();
        }

        #endregion

        #region 接口实现
        public void Dispose()
        {
            Marshal.FreeHGlobal(ImageData);
            Image.Dispose();
        }

        #endregion

        #region 私有方法

        private Bitmap AdjustToRecognitionSize(Bitmap bitmap)
        {
            if (bitmap.Width > 1546 || bitmap.Height > 1536)
            {
                bitmap = ScaleImage(bitmap, 1536, 1536);
            }
            else
            {
                bitmap = ScaleImage(bitmap, bitmap.Width, bitmap.Height);
            }
            return bitmap;
        }

        private Bitmap ScaleImage(Bitmap bitmap, int dstWidth, int dstHeight)
        {
            Graphics g = null;
            try
            {
                //按比例缩放           
                float scaleRate = ImageUtil.GetWidthAndHeight(bitmap.Width, bitmap.Height, dstWidth, dstHeight);
                int width = (int)(bitmap.Width * scaleRate);
                int height = (int)(bitmap.Height * scaleRate);

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
                g.DrawImage(bitmap, new Rectangle((width - width) / 2, (height - height) / 2, width, height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
                return destBitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                g?.Dispose();
            }
        }

        public void ReadImage()
        {
            Image<Gray, byte> grayImage = null;
            try
            {
                if (CameraType == CameraType.Ir)
                {
                    //图像灰度转化
                    grayImage = Image.Convert<Gray, byte>(); //灰度化函数
                    ImageData = Marshal.AllocHGlobal(grayImage.Bytes.Length);
                    Marshal.Copy(grayImage.Bytes, 0, ImageData, grayImage.Bytes.Length);
                }
                else
                {
                    ImageData = Marshal.AllocHGlobal(Image.Bytes.Length);
                    Marshal.Copy(Image.Bytes, 0, ImageData, Image.Bytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                grayImage?.Dispose();
            }
        }

        #endregion
    }
}
