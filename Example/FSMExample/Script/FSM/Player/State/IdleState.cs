using LFramework.AI.FSM;
using UnityEngine;

namespace LFramework.AI.Example.FSMExample
{
    public class IdleState : StateTemplate<PlayerFSMController>
    {
        public IdleState(int id, PlayerFSMController owner) : base(id, owner)
        {
        }
        
        public override void OnEnter(params object[] args)
        {
            base.OnEnter(args);
            Debug.Log("OnIdleState");
        }

        public override void OnStay(params object[] args)
        {
            base.OnStay(args);
        }

        public override void OnExit(params object[] args)
        {
            base.OnExit(args);
        }

    }
}