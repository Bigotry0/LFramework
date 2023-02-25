namespace LFramework
{
    /// <summary>
    /// Model模块接口，用于创建一个Model模块
    /// </summary>
    public interface IModel : IBelongToArchitecture, ICanSetArchitecture, ICanGetUtility, ICanSendEvent
    {
        /// <summary>
        /// 模块初始化方法，会在模块注册时调用
        /// </summary>
        void Init();
    }

    /// <summary>
    /// Model抽象类
    /// </summary>
    public abstract class AbstractModel : IModel
    {
        private IArchitecture Architecture;

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return Architecture;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            Architecture = architecture;
        }

        void IModel.Init()
        {
            OnInit();
        }

        protected abstract void OnInit();
    }
}