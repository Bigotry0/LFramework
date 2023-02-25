namespace LFramework.AI.BehaviorTree
{
    public abstract class BTPreconditionUseDB : BTPrecondition
    {
        protected string _dataToCheck;
        protected int _dataIdToCheck;

        public BTPreconditionUseDB(string dataToCheck)
        {
            this._dataToCheck = dataToCheck;
        }

        public override void Activate(BTDatabase database)
        {
            base.Activate(database);

            _dataIdToCheck = database.GetDataId(_dataToCheck);
        }
    }

    public class BTPreconditionFloat : BTPreconditionUseDB
    {
        public BTPreconditionFloat(string dataToCheck) : base(dataToCheck)
        {
        }

        public override bool Check()
        {
            throw new System.NotImplementedException();
        }
    }
}