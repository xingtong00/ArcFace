﻿using System;
using System.Runtime.InteropServices;

namespace Tong.ArcFace.ArcStruct
{
    /// <summary>
    /// 人脸特征
    /// </summary>
    public struct FaceFeature : IDisposable
    {
        /// <summary>
        /// 特征值 byte[]
        /// </summary>
        public IntPtr Feature { get; set; }

        /// <summary>
        /// 结果集大小
        /// </summary>
        public int FeatureSize { get; set; }

        public void Dispose()
        {
            Marshal.FreeHGlobal(Feature);
        }
    }
}
