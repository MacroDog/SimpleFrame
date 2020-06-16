using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace GameFrame
{
    public enum ResItmeState
    {
        NONE,
        DOWNLOADING,
        FINISH
    }

    public class ResItemData
    {
        public string resName;
        public string resHash;
        public int resSize;
        public string resFullPath;

        public ResItemData()
        {
            resName = string.Empty;
            resHash = string.Empty;
            resSize = 0;
            resFullPath = string.Empty;

        }

        public override string ToString()
        {
            return resFullPath;
        }
    }
    public class ResItem
    {
        public string fullpath = "";
        private AssetBundle bundle;
        public ResItmeState state {
            get;
            private set;
        } = ResItmeState.NONE;
        public Dictionary<object, List<Action>> onfinishs;

        public void AddOnFinish(object obj, List<Action> onfinish)
        {
            if (onfinishs == null)
            {
                onfinishs = new Dictionary<object, List<Action>>();
            }
            onfinishs.Add(obj, onfinish);
        }

        public void StartDownLoading()
        {
            state = ResItmeState.DOWNLOADING;
        }

        public void FinishDownLoading()
        {
            state = ResItmeState.FINISH;
            if (onfinishs != null)
            {
                IEnumerator e = onfinishs.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current != null)
                    {
                        foreach (var item in onfinishs[e.Current])
                        {
                            item.Invoke();
                        }
                    }
                }
            }
        }
    }

    public class ResManager : MonoBehaviour
    {
        public static ResManager Instance {
            get {
                if (instance == null)
                {
                    instance = GameObject.Find("GameStart").GetComponent<ResManager>();
                }
                if (instance == null)
                {
                    FrameDebug.LogError("缺少ReManager");
                }
                return instance;
            }
        }

        private bool verReadFinished = false;
        bool resReadFinished = false;
        /// <summary>
        /// 资源下载保存路径(这个走配置表）
        /// </summary>
        public static string AssetBundlePath = "http://192.168.199.180:9527/res_pc/";
        public static string VersionFileName = "version";

        private static ResManager instance = null;
        internal Dictionary<string, Object> CachedObjects = new Dictionary<string, Object>();
        internal Dictionary<string, ResItem> DownloadItem = new Dictionary<string, ResItem>();
        internal Dictionary<string, ResItem> DownLoadingItem = new Dictionary<string, ResItem>();
        protected Dictionary<string, ResItem> m_resMap = new Dictionary<string, ResItem>();

        internal AssetBundleManifest _manifest;

        private Dictionary<string, ResItemData> m_dataMap = new Dictionary<string, ResItemData>();
        private string downLordPath = "";//暂时使用之后通过配置文件获取
        public const string DIR_SPLIT = "_!_";

        /// <summary>
		/// 已下载的文件
		/// key:文件名
		/// value:文件FileInfo
		/// </summary>
		internal Dictionary<string, FileInfo> DownloadedFiles = new Dictionary<string, FileInfo>();
        /// <summary>
        /// 下载目录子目录
        /// </summary>
        internal const string DOWNLOAD_DIR_NAME = "/Update/";
        /// <summary>
        /// 完整的下载目录
        /// </summary>
        public static string FullDownloadDir {
            get { return Application.persistentDataPath + DOWNLOAD_DIR_NAME; }
        }

        /// <summary>
        /// 缓存的unity objects
        /// 请求过的物体会在这里缓存起来
        /// </summary>
        public T Load<T>(string path) where T : class, new()
        {
            T obj = null;
            Object temp = null;
            if (CachedObjects.TryGetValue(path, out temp))
            {// 尝试在缓存中查找
                obj = temp as T;
                return obj;
            }
            var fileName = path.Replace("/", DIR_SPLIT);//替换目录分隔符
            if (DownloadedFiles.ContainsKey(fileName))
            {
                FileInfo file = DownloadedFiles[fileName];
                var bytes = File.ReadAllBytes(file.FullName);
                var ab = AssetBundle.LoadFromMemory(bytes);
                string objectName = Path.GetFileName(path);
                temp = ab.LoadAsset(objectName, typeof(T));
                obj = temp as T;
                ab.Unload(false);//一旦加载完毕，立即释放assetbundle，但不释放bundle中的物体。
                if (obj == null)
                {
                    FrameDebug.LogError(string.Format("the resource {0} load from ab is null", path));
                    return null;
                }
                CachedObjects.Add(path, temp);
            }
            else
            {
                temp = Resources.Load(path, typeof(T));
                obj = temp as T;
            }
            return obj;
        }

        private IEnumerator DownLordAssetBundle(ResItem item)
        {
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(item.fullpath);
            www.SendWebRequest();
            item.StartDownLoading();
            while (!www.isDone)
            {
                yield return 1;
            }
            if (www.isNetworkError || www.isHttpError)
            {
                FrameDebug.Log("DownLoad Err: " + www.error);
            }
            else
            {
                var ab = DownloadHandlerAssetBundle.GetContent(www);
            }
        }

        private void Start()
        {
            StartCoroutine(GetManifast(() => { initManifast(); }));
        }
        public void AddDownload(string path, Action ondone)
        {
            if (DownloadItem.ContainsKey(path) || DownLoadingItem.ContainsKey(path))
            {
                return;
            }
            ResItem item = new ResItem();
            item.fullpath = path;
            this.StartCoroutine(DownLordAssetBundle(item));
        }

        public void AddDownload(ResItem item)
        {
            this.StartCoroutine(DownLordAssetBundle(item));
        }

        void initManifast()
        {

        }

        //初始ab包manif
        IEnumerator GetManifast(Action onfinish)
        {
            this.StartCoroutine(TryGetVersion());
            bool isFinish = false;
            while (!isFinish)
            {
                yield return new WaitForEndOfFrame();
                while (verReadFinished && resReadFinished)
                {
                    isFinish = true;
                    if (onfinish != null)
                    {
                        onfinish.Invoke();
                    }
                    yield break;
                }
            }
        }

        IEnumerator TryGetVersion()
        {
            var ManifestPath = string.Format("{0}{1}", AssetBundlePath, VersionFileName);
            using (UnityWebRequest uwr = UnityWebRequest.Get(ManifestPath))
            {
                yield return uwr.SendWebRequest();
                if (uwr.isNetworkError || uwr.error != null)
                {
                    FrameDebug.LogError(String.Format("File -{0}- Download failed{1}", ManifestPath, uwr.error));
                    yield break;
                }
                m_dataMap.Clear();
                var data = Encoding.UTF8.GetString(uwr.downloadHandler.data);
                var infos = data.Split('\n');
                foreach (var item in infos)
                {
                    var texts = item.Split('\t');
                    var itemdata = new ResItemData();

                    itemdata.resName = texts[0];
                    itemdata.resHash = texts[1];
                    itemdata.resSize = int.Parse(texts[2]);
                    m_dataMap[itemdata.resName] = itemdata;
                }
                FrameDebug.Log(string.Format("Finish Download Version"));
                verReadFinished = true;
            }
        }
    }
}
