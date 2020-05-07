//
// Client.cs
//
// Author:
// [yfxie]
//

using UnityEngine;
namespace GameFrame
{
    public partial class Client : MonoBehaviour
    {
        public static Client Ins = null;
        public string DbFileName;
        public string ConfigFileName;
        public bool IgnoreUpdate = false;
        public bool IgnoreScript = false;

        public bool Inited { get; protected set; }

        public static bool IsInited {
            get { return Client.Ins != null && Client.Ins.Inited; }
        }


        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (Ins == null)
            {
                Ins = this;
                Inited = false;
            }
            else
            {
                FrameDebug.LogError("Client has been created mutiple times!");
            }
        }

        void Start()
        {

        }


    }
}
