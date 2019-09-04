using System;
using System.Runtime.InteropServices;

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

        /// <summary>
        /// 按照索引获取单个人脸信息
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>人脸信息</returns>
        public SingleFaceInfo GetSingleFaceInfo(int index)
        {
            SingleFaceInfo singleFaceInfo = new SingleFaceInfo();
            singleFaceInfo.FaceRect = new FaceRect();
            singleFaceInfo.FaceOrient = 1;
            if (FaceNum == 0)
                return singleFaceInfo;
            if (0 > index && index > FaceNum)
                return singleFaceInfo;
            singleFaceInfo.FaceRect = Marshal.PtrToStructure<FaceRect>(FaceRects + Marshal.SizeOf<FaceRect>() * index);
            singleFaceInfo.FaceOrient = Marshal.PtrToStructure<int>(FaceOrients + Marshal.SizeOf<int>() * index);
            return singleFaceInfo;
        }
    }
}
