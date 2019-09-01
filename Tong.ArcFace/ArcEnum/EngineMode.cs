namespace Tong.ArcFace.ArcEnum
{
    /// <summary>
    /// 引擎的工作模式
    /// </summary>
    public enum EngineMode
    {
        /// <summary>
        /// 不做方法初始化方法类型
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// 人脸检测方法类型常量
        /// </summary>
        FaceDetect = 0x00000001,
        /// <summary>
        /// 人脸识别方法类型常量，包含图片feature提取和feature比对
        /// </summary>
        FaceRecognition = 0x00000004,
        /// <summary>
        /// 年龄检测方法类型常量
        /// </summary>
        Age = 0x00000008,
        /// <summary>
        /// 性别检测方法类型常量
        /// </summary>
        Gender = 0x00000010,
        /// <summary>
        /// 3D角度检测方法类型常量
        /// </summary>
        Face3DAngle = 0x00000020,
        /// <summary>
        /// RGB活体
        /// </summary>
        Liveness = 0x00000080,
        /// <summary>
        /// 红外活体
        /// </summary>
        IrLiveness = 0x00000400,
        /// <summary>
        /// Rgb全部
        /// </summary>
        RgbAll = FaceDetect | FaceRecognition | Age | Gender | Face3DAngle | Liveness,
        /// <summary>
        /// 全部
        /// </summary>
        IrAll = FaceDetect | FaceRecognition | Age | Gender | Face3DAngle | IrLiveness
    }
}
