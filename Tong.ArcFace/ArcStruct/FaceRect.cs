using System.Drawing;

namespace Tong.ArcFace.ArcStruct
{
    /// <summary>
    /// 人脸框信息
    /// </summary>
    public struct FaceRect
    {
        public int Left { get; set; }

        public int Top { get; set; }

        public int Right { get; set; }

        public int Bottom { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(Left, Top, Right - Left, Bottom - Top);
            }
        }
    }
}
