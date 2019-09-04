using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tong.ArcFace.ArcStruct;

namespace Tong.ArcFace.Util
{
    public class FaceFeatureFactory
    {
        public static FaceFeature CreateFaceFeature(byte[] feature)
        {
            FaceFeature localFeature = new FaceFeature();
            localFeature.Feature = Marshal.AllocHGlobal(feature.Length);
            Marshal.Copy(feature, 0, localFeature.Feature, feature.Length);
            localFeature.FeatureSize = feature.Length;
            return localFeature;
        }
    }
}
