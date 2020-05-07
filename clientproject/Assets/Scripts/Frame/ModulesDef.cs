//
// ModulesDef.cs
//
// Author:
// [yfxie]
//


namespace GameFrame
{
    public partial class Client
    {
        /// <summary>
        /// Lua模块
        /// </summary>
        public static ModLua ModLua {
            get { return Ins.lua; }
        }

        private ModLua lua;


        void InitModules()
        {
        }
    }
}
