namespace LFramework.AI.BehaviorTree
{
    public abstract class BTPrecondition : BTNode
    {
        public BTPrecondition() : base(null)
        {
            
        }

        public abstract bool Check();

        public override BTResult Tick()
        {
            bool success = Check();
            if (success)
            {
                return BTResult.Ended;
            }
            else
            {
                return BTResult.Running;
            }
        }
        
    }
}