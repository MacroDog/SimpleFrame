using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XLua;

namespace GameFrame
{
    [CSharpCallLua]
    public delegate void ReloadDelegate(string path);
    public class LuaFileWatcher
    {
        public static string LuaFilePath = "Sripts/LuaScripts";
        private static HashSet<string> allChangeFile = new HashSet<string>();
        private static ReloadDelegate ReloadFunction;

        private static DirectoryWatcher directoryWatcher;
        public static void CreateLuaFileWatcher(LuaEnv luaenv)
        {
            var scriptPath = Path.Combine(Application.dataPath + LuaFilePath);
            if (directoryWatcher == null)
            {
                directoryWatcher = new DirectoryWatcher(scriptPath, new FileSystemEventHandler(OnLuaFileChange), ".lua");
            }
            ReloadFunction = luaenv.Global.Get<ReloadDelegate>("hotfix");
            EditorApplication.update -= OnReload;
            EditorApplication.update += OnReload;

        }

        public static void OnLuaFileChange(object obj, FileSystemEventArgs args)
        {
            allChangeFile.Add(args.FullPath);
        }

        private static void OnReload()
        {
            if (EditorApplication.isPlaying == false)
            {
                return;
            }
            if (allChangeFile.Count == 0)
            {
                return;
            }
            foreach (var item in allChangeFile)
            {
                ReloadFunction.Invoke(item);
            }
            allChangeFile.Clear();
        }
    }
}