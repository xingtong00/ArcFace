using System;

namespace Tong.ArcFace.ArcStruct
{
    /// <summary>
    /// 多人脸检测结构体
    /// </summary>
    public struct MultiFaceInfo
    {
        /// <summary>
        /// 人脸Rect结果集
        /// </summary>
        public IntPtr FaceRects { get; set; }

        /// <summary>
        /// 人脸角度结果集，与faceRects一一对应  对应ASF_OrientCode
        /// </summary>
        public IntPtr FaceOrients { get; set; }

        /// <summary>
        /// 结果集大小
        /// </summary>
        public int FaceNum { get; set; }

        /// <summary>
        /// face ID，IMAGE模式下不返回FaceID
        /// </summary>
        public IntPtr FaceId { get; set; }
    }
}
