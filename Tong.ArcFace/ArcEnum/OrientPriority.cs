namespace Tong.ArcFace.ArcEnum
{
    /// <summary>
    /// 检测方向的优先级
    /// </summary>
    public enum OrientPriority
    {
        /// <summary>
        /// 仅检测 0 度
        /// </summary>
        Orient0Only = 0x1,

        /// <summary>
        /// 仅检测 90 度
        /// </summary>
        Orient90Only = 0x2,

        /// <summary>
        /// 仅检测 270 度
        /// </summary>
        Orient270Only = 0x3,

        /// <summary>
        /// 仅检测 180 度
        /// </summary>
        Orient180Only = 0x4,

        /// <summary>
        /// 检测 0,90,270,180 全角度
        /// </summary>
        OrientHigherExt = 0x5
    }
}
