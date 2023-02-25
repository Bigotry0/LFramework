using LFramework.AI.FSM;
using UnityEngine;

namespace LFramework.AI.Example.FSMExample
{
    public class AttackState : StateTemplate<PlayerFSMController>
    {
        private float AttackingTime = 1f;

        private float Timer = 0;
        
        public AttackState(int id, PlayerFSMController owner) : base(id, owner)
        {
        }

        public override void OnEnter(params object[] args)
        {
            base.OnEnter(args);
            
            Debug.Log("OnAttackState");
        }

        public override void OnStay(params object[] args)
        {
            base.OnStay(args);
            Debug.Log("Attacking");
            Timer += Time.deltaTime;

            //满足条件时转换状态
            if (Timer > AttackingTime)
            {
                Timer = 0;
                owner.TranslateState(PlayerStates.Idle);
            }
        }

        public override void OnExit(params object[] args)
        {
            base.OnExit(args);
            Debug.Log("AttackSuccess");
        }
    }
}