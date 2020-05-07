//
// ModulesDef.cs
//
// Author:
// [yfxie]
//

namespace GameFrame
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object _synclock = new object();

        public static T Instance {
            get {
                if (_instance == null)
                {
                    lock (_synclock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                            FrameDebug.Log("{0}：创建了单例对象" + typeof(T).Name);
                        }
                    }
                }
                return _instance;
            }
        }

    }
}

