using System;
using Tong.ArcFace.Util;

namespace Tong.ArcFaceSample.Model
{
    public class Face : Result
    {
        public int FaceId { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 是否展示
        /// </summary>
        public bool IsShow { get; set; } = false;

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public int AnalysisTime { get; set; } = 5;

        public bool IsAnalysis
        {
            get
            {
                var temp = DateTime.Now - CreateAt;
                if (temp.Seconds > AnalysisTime)
                    return false;
                return true;
            }
        }

        public Face(Result result)
        {
            FaceRect = result.FaceRect;
            Live = result.Live;
            Age = result.Age;
            Gender = result.Gender;
            FaceFeature = result.FaceFeature;
            Feature = result.Feature;
        }

        public void Update(Result result)
        {
            FaceRect = result.FaceRect;
            Live = result.Live;
            Age = result.Age;
            Gender = result.Gender;
            FaceFeature = result.FaceFeature;
            Feature = result.Feature;
        }
    }
}
