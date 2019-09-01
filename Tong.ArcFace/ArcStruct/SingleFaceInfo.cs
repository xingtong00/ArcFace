namespace Tong.ArcFace.ArcStruct
{
    /// <summary>
    /// 单人脸信息
    /// </summary>
    public struct SingleFaceInfo
    {
        /// <summary>
        /// 人脸框
        /// </summary>
        public FaceRect FaceRect { get; set; }

        /// <summary>
        /// 人脸角度
        /// </summary>
        public int FaceOrient { get; set; }
    }
}
