namespace LFramework
{
    /// <summary>
    /// 命令模式命令接口
    /// </summary>
    public interface ICommand : IBelongToArchitecture, ICanSetArchitecture, ICanGetSystem, ICanGetModel, ICanGetUtility,
        ICanSendEvent, ICanSendCommand
    {
        void Execute();
    }

    /// <summary>
    /// Command抽象类
    /// </summary>
    public abstract class AbstractCommand : ICommand
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

        void ICommand.Execute()
        {
            OnExecute();
        }

        public abstract void OnExecute();
    }
}