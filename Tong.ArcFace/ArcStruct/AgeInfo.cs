using System;

namespace Tong.ArcFace.ArcStruct
{
    /// <summary>
    /// 年龄结果
    /// </summary>
    public struct AgeInfo
    {
        /// <summary>
        /// 年龄检测结果集合
        /// </summary>
        public IntPtr AgeArray { get; set; }

        /// <summary>
        /// 结果集大小
        /// </summary>
        public int Num { get; set; }
    }
}
