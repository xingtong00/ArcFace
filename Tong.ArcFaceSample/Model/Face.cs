using System;
using Tong.ArcFace.Util;

namespace Tong.ArcFaceSample.Model
{
    public class Face : Result
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 是否展示
        /// </summary>
        public bool IsShow { get; set; } = false;

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public bool IsAnalysis
        {
            get
            {
                var temp = DateTime.Now - CreateAt;
                if (temp.Seconds > 5)
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
    }
}
