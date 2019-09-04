using System;
using System.Runtime.InteropServices;
using Tong.ArcFace.ArcEnum;
using Tong.ArcFace.ArcStruct;

namespace Tong.ArcFace
{
    public class ArcFaceApi
    {
        /// <summary>
        /// SDK动态链接库路径
        /// </summary>
        public const string DllPath = "Libs\\x64\\libarcsoft_face_engine.dll";


        /// <summary>
        /// 激活人脸识别SDK引擎函数
        /// </summary>
        /// <param name="appId">SDK对应的AppID</param>
        /// <param name="sdkKey">SDK对应的SDKKey</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "ASFActivation", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Activation(string appId, string sdkKey);

        /// <summary>
        /// 初始化引擎
        /// </summary>
        /// <param name="detectMode">AF_DETECT_MODE_VIDEO 视频模式 | AF_DETECT_MODE_IMAGE 图片模式</param>
        /// <param name="detectFaceOrientPriority">检测脸部的角度优先值，推荐：ASF_OrientPriority.ASF_OP_0_HIGHER_EXT</param>
        /// <param name="detectFaceScaleVal">用于数值化表示的最小人脸尺寸</param>
        /// <param name="detectFaceMaxNum">最大需要检测的人脸个数</param>
        /// <param name="combinedMask">用户选择需要检测的功能组合，可单个或多个</param>
        /// <param name="engine">初始化返回的引擎handle</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "ASFInitEngine", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitEngine(uint detectMode, OrientPriority detectFaceOrientPriority, int detectFaceScaleVal, int detectFaceMaxNum, EngineMode combinedMask, ref IntPtr engine);

        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="engine">引擎handle</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="format">图像颜色空间</param>
        /// <param name="imgData">图像数据</param>
        /// <param name="detectedFaces">人脸检测结果</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "ASFDetectFaces", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DetectFaces(IntPtr engine, int width, int height, ImagePixelFormat format, IntPtr imgData, out MultiFaceInfo detectedFaces);

        /// <summary>
        /// 人脸信息检测（年龄/性别/人脸3D角度）
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="format">图像颜色空间</param>
        /// <param name="imgData">图像数据</param>
        /// <param name="detectedFaces">人脸信息，用户根据待检测的功能裁减选择需要使用的人脸</param>
        /// <param name="combinedMask">只支持初始化时候指定需要检测的功能，在process时进一步在这个已经指定的功能集中继续筛选例如初始化的时候指定检测年龄和性别， 在process的时候可以只检测年龄， 但是不能检测除年龄和性别之外的功能</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "ASFProcess", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Process(IntPtr pEngine, int width, int height, ImagePixelFormat format, IntPtr imgData, MultiFaceInfo detectedFaces, EngineMode combinedMask);


        /// <summary>
        /// 单人脸特征提取
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="format">图像颜色空间</param>
        /// <param name="imgData">图像数据</param>
        /// <param name="faceInfo">单张人脸位置和角度信息</param>
        /// <param name="faceFeature">人脸特征</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "ASFFaceFeatureExtract", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FaceFeatureExtract(IntPtr pEngine, int width, int height, ImagePixelFormat format, IntPtr imgData, SingleFaceInfo faceInfo, out FaceFeature faceFeature);

        /// <summary>
        /// 人脸特征比对
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="faceFeature1">待比较人脸特征1</param>
        /// <param name="faceFeature2"> 待比较人脸特征2</param>
        /// <param name="similarity">相似度(0~1)</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "ASFFaceFeatureCompare", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FaceFeatureCompare(IntPtr pEngine, FaceFeature faceFeature1, FaceFeature faceFeature2, ref float similarity);

        /// <summary>
        /// 获取年龄信息
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="ageInfo">检测到的年龄信息</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "ASFGetAge", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAge(IntPtr pEngine, out AgeInfo ageInfo);

        /// <summary>
        /// 获取性别信息
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="genderInfo">检测到的性别信息</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "ASFGetGender", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetGender(IntPtr pEngine, out GenderInfo genderInfo);

        /// <summary>
        /// 获取3D角度信息
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="p3DAngleInfo">检测到脸部3D角度信息</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "ASFGetFace3DAngle", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFace3DAngle(IntPtr pEngine, IntPtr p3DAngleInfo);

        /// <summary>
        /// 获取RGB活体结果 
        /// </summary>
        /// <param name="hEngine">引擎handle</param>
        /// <param name="livenessInfo">活体检测信息</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "ASFGetLivenessScore", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetLivenessScore(IntPtr hEngine, out LivenessInfo livenessInfo);

        /// <summary>
        /// 该接口目前仅支持单人脸IR活体检测（不支持年龄、性别、3D角度的检测），默认取第一张人脸
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <param name="format">颜色空间格式</param>
        /// <param name="imgData">图片数据</param>
        /// <param name="faceInfo">人脸信息，用户根据待检测的功能选择需要使用的人脸。</param>
        /// <param name="combinedMask">目前只支持传入ASF_IR_LIVENESS属性的传入，且初始化接口需要传入 </param>
        /// <returns></returns>
        [DllImport(DllPath, EntryPoint = "ASFProcess_IR", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ProcessIr(IntPtr pEngine, int width, int height, ImagePixelFormat format, IntPtr imgData, MultiFaceInfo faceInfo, EngineMode combinedMask);

        /// <summary>
        /// 获取IR活体结果
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="livenessInfo">检测到IR活体结果</param>
        /// <returns></returns>
        [DllImport(DllPath, EntryPoint = "ASFGetLivenessScore_IR", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetLivenessScoreIr(IntPtr pEngine, out LivenessInfo livenessInfo);

        /// <summary>
        /// 销毁引擎
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "ASFUninitEngine", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UninitEngine(IntPtr pEngine);

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <returns>调用结果</returns>
        [DllImport(DllPath, EntryPoint = "CallingConvention", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetVersion(IntPtr pEngine);
    }
}
