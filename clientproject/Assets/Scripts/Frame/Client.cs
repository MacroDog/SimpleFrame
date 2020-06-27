//
// Client.cs
//
// Author:
// [yfxie]
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFrame
{
    public partial class Client : MonoBehaviour
    {
        public static Client Ins = null;

        public bool Inited { get; protected set; }
        private List<IClientModule> allModules = new List<IClientModule>();
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
                FindMdule();
            }
            else
            {
                FrameDebug.LogError("Client has been created mutiple times!");
            }
        }

        void Start()
        {
            StartCoroutine(InitModuleData());
        }

        void FindMdule()
        {
            allModules.Add(ModuleLua.Instance);
        }

        //��ʼ��ģ��
        IEnumerator InitModuleData()
        {
            for (int i = 0; i < allModules.Count; i++)
            {
                allModules[i].InitData();
                yield return null;
            }
        }


        IEnumerable InitModuleInfo()
        {
            for (int i = 0; i < allModules.Count; i++)
            {
                allModules[i].InitInfo();
                yield return null;
            }
        }

        IEnumerable InitEnd()
        {
            for (int i = 0; i < allModules.Count; i++)
            {
                allModules[i].InitEnd();
                yield return null;
            }
        }

    }
}
