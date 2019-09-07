using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Tong.ArcFace.ArcEnum;
using Tong.ArcFace.ArcStruct;
using Tong.ArcFace.Common;
using Tong.ArcFace.Properties;
using Tong.ArcFace.Util;

namespace Tong.ArcFace
{
    public class Recognition : IDisposable
    {
        #region 单例

        private Recognition()
        {

        }

        private static readonly Lazy<Recognition> instance = new Lazy<Recognition>(() => new Recognition());

        public static Recognition Instance
        {
            get
            {
                return instance.Value;
            }
        }

        #endregion

        #region 引擎

        private IntPtr _engine = IntPtr.Zero;

        public IntPtr Engine
        {
            get
            {
                return _engine;
            }
        }

        #endregion

        private static object locks = new object();

        /// <summary>
        /// 激活引擎
        /// </summary>
        /// <param name="appId">官网获取的 APPID</param>
        /// <param name="sdkKey">官网获取的 SDKKEY</param>
        public void Activation(string appId, string sdkKey)
        {
            int result = ArcFaceApi.Activation(appId, sdkKey);
            if (result != 0 && result != 90114)
            {
                throw new Exception(Resources.ResourceManager.GetString(result.ToString()));
            }
        }

        /// <summary>
        /// 初始化引擎
        /// </summary>
        /// <param name="detectionMode">VIDEO 视频模式 | IMAGE 图片模式</param>
        /// <param name="orientPriority">检测脸部的角度优先值，推荐仅检测单一角度，效果更优</param>
        /// <param name="scale">
        /// 用于数值化表示的最小人脸尺寸，该尺寸代表人脸尺寸相对于图片长边的占比。
        /// <remarks>
        /// video 模式有效值范围[2,16], Image 模式有效值范围[2,32] 推荐值为 16
        /// </remarks>
        /// </param>
        /// <param name="maxFaceNumber">最大需要检测的人脸个数[1-50]</param>
        /// <param name="combinedMask">用户选择需要检测的功能组合，可单个或多个</param>
        /// <returns>结果</returns>
        public void InitEngine(uint detectionMode = DetectionMode.Image, OrientPriority orientPriority = OrientPriority.Orient0Only, int scale = 16,
                               int maxFaceNumber = 5, EngineMode combinedMask = EngineMode.RgbAll)
        {
            int result = ArcFaceApi.InitEngine(detectionMode, orientPriority, scale, maxFaceNumber, combinedMask, ref _engine);
            CheckResult(result);
        }

        /// <summary>
        /// 检测人脸
        /// </summary>
        /// <param name="imageInfo">图像</param>
        /// <returns>人脸检测的结果</returns>
        public RecognizeResult DetectFaces(ImageInfo imageInfo)
        {
            lock (locks)
            {
                NeedInit();
                int result = ArcFaceApi.DetectFaces(_engine, imageInfo.Width, imageInfo.Height,
                    imageInfo.Format, imageInfo.ImageData, out var faceInfo);
                CheckResult(result);
                RecognizeResult recognizeResult = new RecognizeResult();
                recognizeResult.MultiFaceInfo = faceInfo;
                for (int i = 0; i < faceInfo.FaceNum; i++)
                {
                    Result temp = new Result();
                    temp.FaceRect = Marshal.PtrToStructure<FaceRect>(faceInfo.FaceRects + Marshal.SizeOf<FaceRect>() * i);
                    recognizeResult.Results.Add(temp);
                }
                return recognizeResult;
            }
        }

        /// <summary>
        /// 检测活体
        /// <para>
        /// 需要先执行人脸检测
        /// </para>
        /// </summary>
        /// <param name="imageInfo">图像</param>
        /// <param name="recognizeResult">人脸检测的结果</param>
        /// <param name="cameraType">相机类型</param>
        public void DetectLiveness(ImageInfo imageInfo, RecognizeResult recognizeResult, CameraType cameraType = CameraType.Bgr)
        {
            if (recognizeResult == null)
                throw new ArgumentNullException(nameof(recognizeResult));
            if (recognizeResult.MultiFaceInfo.FaceNum == 0)
                return;
            try
            {
                var result = cameraType == CameraType.Bgr
                    ? ArcFaceApi.Process(_engine, imageInfo.Width, imageInfo.Height, imageInfo.Format,
                    imageInfo.ImageData, recognizeResult.MultiFaceInfo, EngineMode.Liveness)
                    : ArcFaceApi.ProcessIr(_engine, imageInfo.Width, imageInfo.Height, imageInfo.Format,
                    imageInfo.ImageData, recognizeResult.MultiFaceInfo, EngineMode.IrLiveness);
                CheckResult(result);
                LivenessInfo livenessInfo;
                result = cameraType == CameraType.Bgr
                    ? ArcFaceApi.GetLivenessScore(_engine, out livenessInfo)
                    : ArcFaceApi.GetLivenessScoreIr(_engine, out livenessInfo);
                CheckResult(result);
                recognizeResult.LivenessInfo = livenessInfo;
                if (recognizeResult.LivenessInfo.Num != recognizeResult.Results.Count)
                    return;
                foreach (var r in recognizeResult.Results)
                {
                    r.Live = (Liveness)Marshal.PtrToStructure<int>(recognizeResult.LivenessInfo.IsLive);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 检测多种属性
        /// </summary>
        /// <param name="imageInfo">图像</param>
        /// <param name="combinedMask">需要检测的组合</param>
        /// <returns>检测的结果</returns>
        public RecognizeResult DetectFaceInfo(ImageInfo imageInfo, EngineMode combinedMask)
        {
            try
            {
                RecognizeResult recognizeResult = DetectFaces(imageInfo);
                if (recognizeResult.MultiFaceInfo.FaceNum == 0)
                    return recognizeResult;
                var result = ArcFaceApi.Process(_engine, imageInfo.Width, imageInfo.Height, imageInfo.Format,
                    imageInfo.ImageData, recognizeResult.MultiFaceInfo, combinedMask);
                CheckResult(result);
                if ((combinedMask & EngineMode.Liveness) > 0)
                {
                    result = ArcFaceApi.GetLivenessScore(_engine, out var livenessInfo);
                    CheckResult(result);
                    recognizeResult.LivenessInfo = livenessInfo;
                    if (recognizeResult.LivenessInfo.Num < 1)
                        return recognizeResult;
                    recognizeResult.Results[recognizeResult.MaxFaceIndex].Live = (Liveness)Marshal.PtrToStructure<int>(recognizeResult.LivenessInfo.IsLive);
                }
                if ((combinedMask & EngineMode.Age) > 0)
                {
                    result = ArcFaceApi.GetAge(_engine, out var ageInfo);
                    CheckResult(result);
                    recognizeResult.AgeInfo = ageInfo;
                    if (recognizeResult.AgeInfo.Num < 1)
                        return recognizeResult;
                    if (recognizeResult.AgeInfo.Num != recognizeResult.Results.Count)
                        throw new Exception("检测年龄结果集与人脸数量不符");
                    for (int i = 0; i < recognizeResult.AgeInfo.Num; i++)
                    {
                        recognizeResult.Results[i].Age =
                            Marshal.PtrToStructure<int>(recognizeResult.AgeInfo.AgeArray + Marshal.SizeOf<int>() * i);
                    }
                }
                if ((combinedMask & EngineMode.Gender) > 0)
                {
                    result = ArcFaceApi.GetGender(_engine, out var genderInfo);
                    CheckResult(result);
                    recognizeResult.GenderInfo = genderInfo;
                    if (recognizeResult.GenderInfo.Num < 1)
                        return recognizeResult;
                    if (recognizeResult.GenderInfo.Num != recognizeResult.Results.Count)
                        throw new Exception("检测性别结果集与人脸数量不符");
                    for (int i = 0; i < recognizeResult.GenderInfo.Num; i++)
                    {
                        recognizeResult.Results[i].Gender =
                            (Gender)Marshal.PtrToStructure<int>(recognizeResult.GenderInfo.GenderArray + Marshal.SizeOf<int>() * i);
                    }
                }
                return recognizeResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 提取特征码
        /// </summary>
        /// <param name="imageInfo">图像</param>
        /// <param name="recognizeResult">人脸检测的结果</param>
        /// <param name="index">需提取人脸的索引</param>
        /// <param name="cameraType">相机类型</param>
        /// <returns>特征码</returns>
        public void ExtractFeature(ImageInfo imageInfo, RecognizeResult recognizeResult, int index, CameraType cameraType = CameraType.Bgr)
        {
            if (recognizeResult.MultiFaceInfo.FaceNum == 0 || (index < 0 && recognizeResult.MultiFaceInfo.FaceNum < index))
                throw new Exception("索引错误");
            SingleFaceInfo singleFaceInfo = recognizeResult.MultiFaceInfo.GetSingleFaceInfo(index);
            try
            {
                var result = ArcFaceApi.FaceFeatureExtract(_engine, imageInfo.Width, imageInfo.Height, imageInfo.Format,
                    imageInfo.ImageData, singleFaceInfo, out var feature);
                CheckResult(result);
                recognizeResult.Results[index].Feature = new byte[feature.FeatureSize];
                Marshal.Copy(feature.Feature, recognizeResult.Results[index].Feature, 0, feature.FeatureSize);

                FaceFeature localFeature = new FaceFeature();
                localFeature.Feature = Marshal.AllocHGlobal(recognizeResult.Results[index].Feature.Length);
                Marshal.Copy(recognizeResult.Results[index].Feature, 0, localFeature.Feature, recognizeResult.Results[index].Feature.Length);
                localFeature.FeatureSize = recognizeResult.Results[index].Feature.Length;

                recognizeResult.Results[index].FaceFeature = localFeature;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 检查结果
        /// </summary>
        /// <param name="result">结果</param>
        public static void CheckResult(int result)
        {
            if (result != 0)
            {
                throw new Exception(Resources.ResourceManager.GetString(result.ToString()));
            }
        }

        /// <summary>
        /// 是否需要初始化
        /// </summary>
        private void NeedInit()
        {
            if (_engine == IntPtr.Zero)
                InitEngine();
        }

        public void Dispose()
        {
            lock (locks)
            {
                if (_engine != IntPtr.Zero)
                    ArcFaceApi.UninitEngine(_engine);
            }
        }
    }
}
