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
        public static ModuleLua ModLua {
            get { return Ins.lua; }
        }

        private ModuleLua lua;


        void InitModules()
        {
        }
    }
}
