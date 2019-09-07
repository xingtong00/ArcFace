using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tong.ArcFace.ArcStruct;
using Tong.ArcFace.Common;

namespace Tong.ArcFace.Util
{
    public class RecognizeResult
    {
        /// <summary>
        /// 结果
        /// </summary>
        public List<Result> Results { get; set; } = new List<Result>();

        public AgeInfo AgeInfo { get; set; }

        public GenderInfo GenderInfo { get; set; }

        private MultiFaceInfo _multiFaceInfo;

        public MultiFaceInfo MultiFaceInfo
        {
            get
            {
                return _multiFaceInfo;
            }
            set
            {
                MaxFaceInfo = GetMaxFace(MultiFaceInfo);
                _multiFaceInfo = value;
            }
        }

        public SingleFaceInfo MaxFaceInfo { get; set; }

        public int MaxFaceIndex { get; set; }

        public LivenessInfo LivenessInfo { get; set; }


        /// <summary>
        /// 获取多个人脸检测结果中面积最大的人脸
        /// </summary>
        /// <param name="multiFaceInfo">人脸检测结果</param>
        /// <returns>面积最大的人脸信息</returns>
        private SingleFaceInfo GetMaxFace(MultiFaceInfo multiFaceInfo)
        {
            SingleFaceInfo singleFaceInfo = new SingleFaceInfo();
            singleFaceInfo.FaceRect = new FaceRect();
            singleFaceInfo.FaceOrient = 1;

            int maxArea = 0;
            int index = -1;
            for (int i = 0; i < multiFaceInfo.FaceNum; i++)
            {
                try
                {
                    FaceRect rect = Marshal.PtrToStructure<FaceRect>(multiFaceInfo.FaceRects + Marshal.SizeOf<FaceRect>() * i);
                    int area = (rect.Right - rect.Left) * (rect.Bottom - rect.Top);
                    if (maxArea <= area)
                    {
                        maxArea = area;
                        index = i;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            if (index != -1)
            {
                singleFaceInfo.FaceRect = Marshal.PtrToStructure<FaceRect>(multiFaceInfo.FaceRects + Marshal.SizeOf<FaceRect>() * index);
                singleFaceInfo.FaceOrient = Marshal.PtrToStructure<int>(multiFaceInfo.FaceOrients + Marshal.SizeOf<int>() * index);
                MaxFaceIndex = index;
            }
            return singleFaceInfo;
        }
    }

    public class Result
    {
        /// <summary>
        /// 人脸位置信息
        /// </summary>
        public FaceRect FaceRect { get; set; }

        /// <summary>
        /// 是否是真人 
        /// 0：非真人；1：真人；-1：不确定；-2:传入人脸数>1；
        /// </summary>
        public Liveness Live { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 0：男；1：女
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// 人脸特征
        /// </summary>
        public FaceFeature FaceFeature { get; set; }

        /// <summary>
        /// 特征码
        /// </summary>
        public byte[] Feature { get; set; }
    }
}
