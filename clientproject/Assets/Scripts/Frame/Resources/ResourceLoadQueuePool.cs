
using System.Collections.Generic;

namespace GameFrame
{
    //加载队列用于解决res依赖问题
    public class ResourceLoadQueue
    {

        private bool m_IsLoading = false;
        private List<ResItem> jobs = new List<ResItem>(8);

        private int m_index = 0;

        public void AddJob(ResItem item)
        {
            if (m_IsLoading)
            {
                return;
            }
            foreach (var res in jobs)
            {
                if (res.fullpath == item.fullpath)
                {
                    return;
                }
            }
            jobs.Add(item);
        }

        public void AddJobs(List<ResItem> resitems)
        {
            if (m_IsLoading)
            {
                return;
            }
            foreach (var item in resitems)
            {
                AddJob(item);
            }
        }

        public void LoadAync()
        {
            if (m_IsLoading)
            {
                return;
            }
            m_IsLoading = true;
            LoadAync(m_index);
        }

        private void LoadAync(int index)
        {
            if (index < jobs.Count)
            {
                ResManager.Instance.AddDownload(jobs[index]);
            }
            else
            {
                FrameDebug.Log("Index out range");
            }
        }

        public void Reset()
        {
            m_IsLoading = false;
            jobs.Clear();
            m_index = 0;
        }
    }
    public class ResourceLoadQueuePool
    {
        public static int AllBusyQueueNum = 0;
        private const int DEF_SIZE = 5;
        private static List<ResourceLoadQueue> queues = new List<ResourceLoadQueue>();
        private Dictionary <string,ResourceLoadQueue> OnUse = new Dictionary<string,ResourceLoadQueue>();

        public ResourceLoadQueue GetQueue(string assetBundle){
            ResourceLoadQueue loadQueue;
            if (OnUse.TryGetValue(assetBundle, out loadQueue))
            {
                return loadQueue;
            }
            loadQueue = new ResourceLoadQueue();
            queues.Add(loadQueue);
            OnUse.Add(assetBundle, loadQueue);
            return loadQueue;

        }
    
    }
}