using System;

namespace Tong.ArcFace.ArcStruct
{
    /// <summary>
    ///  3D人脸角度检测
    /// </summary>
    public struct Face3DAngle
    {
        /// <summary>
        /// 横滚角
        /// </summary>
        public IntPtr Roll { get; set; }

        /// <summary>
        /// 偏航角
        /// </summary>
        public IntPtr Yaw { get; set; }

        /// <summary>
        /// 俯仰角
        /// </summary>
        public IntPtr Pitch { get; set; }

        /// <summary>
        /// 0:正常; 非0:异常
        /// </summary>
        public IntPtr Status { get; set; }

        /// <summary>
        /// 结果集大小
        /// </summary>
        public int Num;
    }
}
