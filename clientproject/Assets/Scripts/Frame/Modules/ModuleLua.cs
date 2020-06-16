using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;
namespace GameFrame
{
    public class ModuleLua : Singleton<ModuleLua>, IClientModule
    {

        public TextAsset HotFix;

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
                if (HotFix != null)
                {
                    luaEnv.DoString(HotFix.text);
                }
                hotfixed = true;
            }
        }
        void IClientModule.InitData()
        {
            DoHotFix();
        }

        void IClientModule.InitEnd()
        {
            throw new System.NotImplementedException();
        }

        void IClientModule.InitInfo()
        {
            throw new System.NotImplementedException();
        }

        void IClientModule.ResetData()
        {
            throw new System.NotImplementedException();
        }

        void IClientModule.ResetInfo()
        {
            throw new System.NotImplementedException();
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
    }
}

