namespace Tong.ArcFace.ArcStruct
{

    /// <summary>
    /// 图片检测模式
    /// </summary>
    public struct DetectionMode
    {
        /// <summary>
        /// Video模式，一般用于多帧连续检测
        /// </summary>
        public const uint Video = 0x00000000;

        /// <summary>
        /// Image模式，一般用于静态图的单次检测
        /// </summary>
        public const uint Image = 0xFFFFFFFF;
    }
}
