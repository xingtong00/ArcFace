namespace Tong.ArcFace.ArcEnum
{
    /// <summary>
    /// 检测方向的优先级
    /// </summary>
    public enum OrientPriority
    {
        /// <summary>
        /// 常规预览下正方向
        /// </summary>
        Orient0Only = 0x1,

        /// <summary>
        /// 基于0°逆时针旋转90°的方向
        /// </summary>
        Orient90Only = 0x2,

        /// <summary>
        /// 基于0°逆时针旋转270°的方向
        /// </summary>
        Orient270Only = 0x3,

        /// <summary>
        /// 基于0°旋转180°的方向（逆时针、顺时针效果一样）
        /// </summary>
        Orient180Only = 0x4,

        /// <summary>
        /// 全角度
        /// </summary>
        OrientHigherExt = 0x5
    }
}
