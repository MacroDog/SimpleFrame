using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;
namespace GameFrame
{
    public class ModuleLua : Singleton<ModuleLua>, IClientModule
    {

        public string HotFix = "Hotfix";
        public const string luaScriptsFolder = @"/Scripts/Frame/LuaScripts/";
        private static string luaScriptsFullPath; 
        public TextAsset fixtext;

        private bool hotfixed = false;

        private LuaEnv luaEnv;

        private void DoHotFix()
        {
            if (!hotfixed)
            {
                if (luaEnv == null)
                {
                    luaEnv = new LuaEnv();
                }
                if (fixtext != null)
                {
                    luaEnv.DoString(fixtext.text);
                }
                hotfixed = true;
            }
        }
        void IClientModule.InitData()
        {
            FrameDebug.Log("Start Init ModuleLua");
            luaScriptsFullPath = Application.dataPath + luaScriptsFolder;
            DoHotFix();
            luaEnv.AddLoader(CustomLoader);
            LoadScript(HotFix);
            LuaFileWatcher.CreateLuaFileWatcher(luaEnv);
        }

        void IClientModule.InitEnd()
        {

        }

        void IClientModule.InitInfo()
        {

        }

        void IClientModule.ResetData()
        {

        }

        void IClientModule.ResetInfo()
        {

        }
        public bool DoFile(string path)
        {
            TextAsset file = ReadLuaFile(path);

            if (file != null)
            {
                luaEnv.DoString(file.bytes);
                return true;
            }
            else
            {
                Debug.LogError("Lua dofile: " + file + " can not be loaded!");
                return false;
            }
        }

        public void LoadScript(string name)
        {
            SafeDoString(string.Format("require('{0}')", name));
        }

        private static byte[] CustomLoader(ref string filepath)
        {
            string scriptPath = string.Empty;
            filepath = filepath.Replace(".", "/") + ".lua";

            scriptPath = Path.Combine(luaScriptsFullPath, filepath);
            if (!File.Exists(scriptPath))
            {
                FrameDebug.LogError("Error Lua Script Path" + scriptPath);
                return null;
            }
            File.SetAttributes(scriptPath, FileAttributes.Normal);
            return File.ReadAllBytes(scriptPath);
        }
        private static TextAsset ReadLuaFile(string path)
        {
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            if (path.EndsWith(".lua"))
            {
                path = path.Substring(0, path.Length - 4);
            }

            TextAsset text = null;
            if (Application.isPlaying)
            {
                text = ResManager.Instance.Load<TextAsset>(path);
            }
            else
            {
                text = Resources.Load<TextAsset>(path);
            }
            return text;
        }
        private static string ReadLuaFileContent(string path)
        {

            var temp = ReadLuaFile(path);
            if (temp == null)
            {
                return string.Empty;
            }
            string content = ReadLuaFile(path).text;
#if UNITY_EDITOR
            //强制读取最新内容
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                string p = UnityEditor.AssetDatabase.GetAssetPath(temp);
                if (!string.IsNullOrEmpty(p))
                {
                    content = File.ReadAllText(p);
                }
            }
#endif
            //是否需要解密？
            return content;
        }

        public void SafeDoString(string scriptContent)
        {
            if (luaEnv != null)
            {
                try
                {
                    luaEnv.DoString(scriptContent);
                }
                catch (System.Exception ex)
                {
                    string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                    Debug.LogError(msg, null);
                }
            }
        }
    }
}

