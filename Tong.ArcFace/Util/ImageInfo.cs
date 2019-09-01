using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Tong.ArcFace.ArcEnum;

namespace Tong.ArcFace.Util
{
    public class ImageInfo : IDisposable
    {
        /// <summary>
        /// 图像数据
        /// </summary>
        public IntPtr ImageData { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 图片格式
        /// </summary>
        public ImagePixelFormat Format { get; set; }

        public void Dispose()
        {
            Marshal.FreeHGlobal(ImageData);
        }
    }
}
