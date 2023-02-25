using System.Collections.Generic;
using UnityEngine;

namespace LFramework.AI.FSM
{
    /// <summary>
    /// 状态机类：完成状态的存储，切换和状态的保持
    /// 该类由
    /// </summary>
    public class StateMachine
    {
        //储存当前状态机所控制的所有状态
        private readonly Dictionary<int, StateBase> StateCache;

        //上一个状态
        private StateBase PreviousState;

        //当前状态
        private StateBase CurrentState;

        /// <summary>
        /// 初始化状态机
        /// </summary>
        /// <param name="beginState">传入状态以设置状态机初始状态</param>
        public StateMachine(StateBase beginState)
        {
            PreviousState = null;
            CurrentState = beginState;

            StateCache = new Dictionary<int, StateBase>();
            //将状态添加进字典中
            AddState(beginState);
        }

        /// <summary>
        /// 启动状态机
        /// </summary>
        public void StartStateMachine()
        {
            CurrentState.OnEnter();
        }
        
        /// <summary>
        /// 增添状态进该状态机中
        /// </summary>
        /// <param name="state">状态对象</param>
        public void AddState(StateBase state)
        {
            if (StateCache.ContainsKey(state.ID))
            {
                return;
            }

            StateCache.Add(state.ID, state);
            state.Machine = this;
        }

        /// <summary>
        /// 通过状态ID切换状态
        /// </summary>
        /// <param name="id">状态ID</param>
        public void TranslateState(int id)
        {
            //检查状态是否属于该状态机所控制
            if (!StateCache.ContainsKey(id))
            {
                Debug.LogError(
                    $"The state represented by the id: {id} is not part of the state machine: {this} control");
                return;
            }

            //执行当前状态退出方法
            CurrentState.OnExit();

            PreviousState = CurrentState;
            CurrentState = StateCache[id];

            //进入新状态
            CurrentState.OnEnter();
        }

        /// <summary>
        /// 状态保持
        /// </summary>
        public void Update()
        {
            CurrentState?.OnStay();
        }
    }
}