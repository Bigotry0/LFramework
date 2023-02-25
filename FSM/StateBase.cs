namespace LFramework.AI.FSM
{
    /// <summary>
    /// 状态基类：为子类提供方法
    /// </summary>
    public class StateBase
    {
        /// <summary>
        /// 状态ID
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// 所属状态机
        /// </summary>
        public StateMachine Machine;

        /// <summary>
        /// 设置状态ID
        /// </summary>
        /// <param name="id">状态ID</param>
        public StateBase(int id)
        {
            ID = id;
        }

        /// <summary>
        /// 开始状态
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnEnter(params object[] args)
        {
        }

        /// <summary>
        /// 保持状态
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnStay(params object[] args)
        {
        }

        /// <summary>
        /// 退出状态
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnExit(params object[] args)
        {
        }
    }
}