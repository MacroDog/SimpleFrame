


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame
{
    public class BundleManager : Singleton<BundleManager>
    {

        /// <summary>
        /// 资源路径根节点
        /// </summary>
        public static string AssetsRoot = "Assets/ResourcesAssets";
        /// <summary>
        /// 版本信息文件 （加上资源号码）
        /// </summary>
        public static string VersionFileName = "version";
        /// <summary>
        /// 资源信息文件 (加上资源号）
        /// </summary>
        public static string ResourcesFileName = "resources";
    }
}

