using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tong.ArcFace.ArcStruct;

namespace Tong.ArcFace.Util
{
    public class RecognizeResult
    {
        /// <summary>
        /// 结果
        /// </summary>
        public List<Result> Results { get; set; } = new List<Result>();

        public AgeInfo AgeInfo { get; set; }

        public MultiFaceInfo MultiFaceInfo { get; set; }

        public SingleFaceInfo MaxFaceInfo { get; set; }

        public LivenessInfo LivenessInfo { get; set; }
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
        public int Live { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
    }
}
