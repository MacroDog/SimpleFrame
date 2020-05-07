//
// ClientModule.cs
//
// Author:
// [yfxie]
//

namespace GameFrame
{
    public abstract class ClientModule
    {
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="reader"></param>
        public virtual void InitData() { }

        /// <summary>
        /// 加载信息
        /// </summary>
        /// <param name="json"></param>
        public virtual void InitInfo() { }

        /// <summary>
        /// 初始化结束
        /// </summary>
        public virtual void InitEnd() { }

        /// <summary>
        /// 重置数据
        /// </summary>
        public virtual void ResetData() { }

        /// <summary>
        /// 重置信息
        /// </summary>
        public virtual void ResetInfo() { }
    }
}

