

namespace GameFrame
{
    public interface IClientModule
    {
        void InitData();

        /// <summary>
        /// 加载信息
        /// </summary>
        /// <param name="json"></param>
        void InitInfo();

        /// <summary>
        /// 初始化结束
        /// </summary>
        void InitEnd();

        /// <summary>
        /// 重置数据
        /// </summary>
        void ResetData();

        /// <summary>
        /// 重置信息
        /// </summary>
        void ResetInfo();
    }
}
