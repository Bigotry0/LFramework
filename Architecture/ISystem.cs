namespace LFramework
{
    /// <summary>
    /// System模块接口，用于创建一个System模块
    /// </summary>
    public interface ISystem : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetSystem, ICanGetUtility,
        ICanSendEvent,
        ICanRegisterEvent
    {
        /// <summary>
        /// 模块初始化方法，会在模块注册时调用
        /// </summary>
        void Init();
    }

    /// <summary>
    /// System抽象类
    /// </summary>
    public abstract class AbstractSystem : ISystem
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

        void ISystem.Init()
        {
            OnInit();
        }

        protected abstract void OnInit();
    }
}