
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFrame
{
    public class AssetBundleBuilder : MonoBehaviour
    {
        private static string versionName = "version";
        public static string GetOutPutDir(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                    return "AssetBundles/Windows/";
                default:
                    return "AssetBundles/default/";
            }
        }

        public static string GetFullOutPutDir(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                    return Application.dataPath.Remove(Application.dataPath.Length - 6) + GetOutPutDir(target);
                default:
                    FrameDebug.LogError("Dont Contain target" + target.ToString());
                    return "AssetBundles";
            }
        }

        public static string GetManifestPath(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                    return GetFullOutPutDir(target) + "Windows";
                default:
                    FrameDebug.LogError("Dont Contain target" + target.ToString());
                    return GetFullOutPutDir(target) + "default";
            }
        }
        public static string GetAssetBuildPath(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                    return GetFullOutPutDir(target) + "Windows";
                default:
                    FrameDebug.LogError("Dont Contain target" + target.ToString());
                    return GetFullOutPutDir(target) + "default";
            }
        }


        public static void BuildBundleBuild(BuildTarget target)
        {
            var path = GetOutPutDir(target);
            CheckFile(path);
            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, target);
            WriteVersion(target);
        }

        public static void CheckFile(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        [MenuItem("ResourceManager/发布工具(pc)/发布AssetBuild", false, 105)]
        public static void publishPC()
        {
            BuildBundleBuild(BuildTarget.StandaloneWindows64);

        }

        public void BuildVersion(string outPath, BuildTarget target)
        {
        }

        static string GetTargetName(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneWindows:
                    return "Windows";
                default:
                    FrameDebug.LogError("Null Target Name");
                    return "";
            }
        }

        static void WriteVersion(BuildTarget target)
        {
            FrameDebug.Log("Start Write Verison");
            var oripath = GetFullOutPutDir(target);
            var path = GetAssetBuildPath(target);
            var manifestPath = GetManifestPath(target);
            var temps = new List<string>(Directory.GetFiles(oripath));
            temps.Remove(path);
            var versionpath = oripath + versionName;
            if (Directory.Exists(versionpath))
            {
                Directory.Delete(versionpath);
            }
            var viersionfile = File.CreateText(versionpath);
            var allfiles = new List<string>(Directory.GetFiles(oripath));

            allfiles.Remove(versionpath);
            allfiles.Remove(manifestPath);
            allfiles.Remove(manifestPath + ".manifest");
            var allmanifest = AssetBundle.LoadFromFile(manifestPath).LoadAsset<AssetBundleManifest>("AssetBundleManifest").GetAllAssetBundles();
            for (int i = 0; i < allmanifest.Length; i++)
            {
                if (!allmanifest[i].EndsWith(".manifest"))
                {
                    var fullitemPath = oripath + allmanifest[i];
                    var manifest = AssetBundle.LoadFromFile(fullitemPath);
                    if (manifest != null)
                    {
                        var code = manifest.GetHashCode();
                        allfiles.Remove(fullitemPath);
                        if (i == allmanifest.Length - 1)
                        {
                            viersionfile.Write(string.Format("{0}\t{1}\t{2}", allmanifest[i], AssetBundleUtils.GetMD5(fullitemPath), AssetBundleUtils.GetSize(fullitemPath)));
                        }
                        else
                        {
                            viersionfile.WriteLine(string.Format("{0}\t{1}\t{2}", allmanifest[i], AssetBundleUtils.GetMD5(fullitemPath), AssetBundleUtils.GetSize(fullitemPath)));
                        }
                    }
                }
            }
            viersionfile.Close();
            //删除不属于的ab
            foreach (var item in allfiles)
            {
                File.Delete(item);
            }
            FrameDebug.Log("Finish");
        }
    }
}