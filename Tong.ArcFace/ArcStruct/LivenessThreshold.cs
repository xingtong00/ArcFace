namespace Tong.ArcFace.ArcStruct
{
    /// <summary>
    /// 活体置信度
    /// </summary>
    public struct LivenessThreshold
    {
        /// <summary>
        /// RGB活体置信度，默认阈值0.5
        /// </summary>
        public float ThresholdmodelBGR { get; set; }

        /// <summary>
        /// IR活体置信度，默认阈值0.7
        /// </summary>
        public float ThresholdmodelIR { get; set; }
    }
}
