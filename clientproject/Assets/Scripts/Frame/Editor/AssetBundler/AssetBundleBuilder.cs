
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
                    return GetFullOutPutDir(target) + "Windows.manifest";
                default:
                    FrameDebug.LogError("Dont Contain target" + target.ToString());
                    return GetFullOutPutDir(target) + "default.manifest";
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

        string GetTargetName(BuildTarget target)
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
            var oripath = GetFullOutPutDir(target);
            var path = GetAssetBuildPath(target);

            var manifest = GetManifestPath(target);
            var temp = AssetBundle.LoadFromFile(path);
            if (temp == null)
            {
                FrameDebug.LogError("Error Path" + path);
                return;
            }
            var allasset = temp.LoadAsset<AssetBundleManifest>("AssetBundleManifest").GetAllAssetBundles();
            var versionpath = oripath + versionName;
            if (File.Exists(versionpath))
            {
                File.Delete(versionpath);
            }
            var fileInfo = File.CreateText(versionpath);
            foreach (var item in allasset)
            {
                fileInfo.WriteLine(item);
            }
            fileInfo.Close();
            FrameDebug.Log("Finish");
        }
    }
}