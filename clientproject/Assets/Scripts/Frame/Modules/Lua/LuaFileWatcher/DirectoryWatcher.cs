using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFrame
{
    public class DirectoryWatcher
    {
        public DirectoryWatcher(string path, FileSystemEventHandler handler, string filter)
        {

        }

        public void CreateWatcher(string path, FileSystemEventHandler handler, string filter)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            if (filter != string.Empty)
            {
                watcher.Filter = filter;
            }
            watcher.Changed += handler;
            watcher.IncludeSubdirectories = true; //includeSubdirectories;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            watcher.InternalBufferSize = 10240;
        }
    }
}
