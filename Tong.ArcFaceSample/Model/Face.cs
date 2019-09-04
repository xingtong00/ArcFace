using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tong.ArcFace.ArcStruct;
using Tong.ArcFace.Util;

namespace Tong.ArcFaceSample.Model
{
    public class Face : Result
    {
        /// <summary>
        /// 评价
        /// </summary>
        public string Evaluation { get; set; }

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
