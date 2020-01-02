using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tong.ArcFace.ArcStruct
{
    /// <summary>
    /// 激活文件信息
    /// </summary>
    public struct ActiveFileInfo
    {
        /// <summary>
        /// SDK开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// SDK截止时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 激活码
        /// </summary>
        public string ActiveKey { get; set; }

        /// <summary>
        /// 平台版本
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// SDK类型
        /// </summary>
        public string SdkType { get; set; }

        /// <summary>
        /// APPID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// SDKKEY
        /// </summary>
        public string SdkKey { get; set; }

        /// <summary>
        /// SDK版本号
        /// </summary>
        public string SdkVersion { get; set; }

        /// <summary>
        /// 激活文件版本号
        /// </summary>
        public string FileVersion { get; set; }
    }
}
