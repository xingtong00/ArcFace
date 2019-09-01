using System;

namespace Tong.ArcFace.ArcStruct
{
    /// <summary>
    /// 性别
    /// </summary>
    public struct GenderInfo
    {
        /// <summary>
        /// 性别检测结果集合
        /// </summary>
        public IntPtr GenderArray { get; set; }

        /// <summary>
        /// 结果集大小
        /// </summary>
        public int Num { get; set; }
    }
}
