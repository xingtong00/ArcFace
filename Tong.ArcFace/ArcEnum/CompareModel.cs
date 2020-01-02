using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tong.ArcFace.ArcEnum
{
    public enum CompareModel
    {
        /// <summary>
        /// 用于生活照之间的特征比对，推荐阈值0.80
        /// </summary>
        LifePhoto = 0x1,

        /// <summary>
        /// 用于证件照或生活照与证件照之间的特征比对，推荐阈值0.82
        /// </summary>
        IdPhoto = 0x2
    }
}
