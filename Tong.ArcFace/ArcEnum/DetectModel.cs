﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tong.ArcFace.ArcEnum
{
    /// <summary>
    /// 根据图像颜色空间选择对应的算法模型进行检测。
    /// </summary>
    public enum DetectModel
    {
        /// <summary>
        /// RGB图像检测模型
        /// </summary>
        DetectModelRgb = 0x1
    }
}
