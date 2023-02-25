namespace LFramework.AI.FSM
{
    /// <summary>
    /// 状态所有者泛型类（模板类）
    /// </summary>
    /// <typeparam name="T">状态所有者</typeparam>
    public class StateTemplate<T> : StateBase
    {
        //状态所有者（泛型）
        protected readonly T owner;

        public StateTemplate(int id,T owner) : base(id)
        {
            this.owner = owner;
        }
    }
}