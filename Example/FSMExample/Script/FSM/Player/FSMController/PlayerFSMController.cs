using LFramework.AI.FSM;

namespace LFramework.AI.Example.FSMExample
{
    public enum PlayerStates
    {
        None,
        Idle,
        Move,
        Attack
    }

    public class PlayerFSMController : FSMController<PlayerStates>, IController
    {
        void Start()
        {
            AddStateInStateMachine(PlayerStates.Idle, new IdleState(1, this));
            AddStateInStateMachine(PlayerStates.Move, new MoveState(2, this));
            AddStateInStateMachine(PlayerStates.Attack, new AttackState(3, this));
        }

        private void Update()
        {
            UpdateState();
        }

        public void ToAttackState()
        {
            TranslateState(PlayerStates.Attack);
        }
        
        public void ToMoveState()
        {
            TranslateState(2);
        }
        
        public void ToIdleState()
        {
            TranslateState(PlayerStates.Idle);
        }
        
        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return FSMExample.Interface;
        }
    }
}