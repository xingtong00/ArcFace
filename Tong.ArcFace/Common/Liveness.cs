namespace Tong.ArcFace.Common
{
    public enum Liveness
    {
        /// <summary>
        /// 非真人
        /// </summary>
        Sham = 0,

        /// <summary>
        /// 真人
        /// </summary>
        Reality = 1,

        /// <summary>
        /// 不确定
        /// </summary>
        Indetermination = -1,

        /// <summary>
        /// 传入人脸数 > 1
        /// </summary>
        Error = -2
    }
}
