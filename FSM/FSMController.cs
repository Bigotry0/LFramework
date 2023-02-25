using System;
using System.Collections.Generic;
using UnityEngine;

namespace LFramework.AI.FSM
{
    /// <summary>
    /// 用于快速创建一个FSM控制器
    /// </summary>
    /// <typeparam name="T">状态枚举类</typeparam>
    public class FSMController<T> : MonoBehaviour where T : Enum
    {
        /// <summary>
        /// 当前状态ID
        /// </summary>
        public int CurrentStateID;

        /// <summary>
        /// 状态机
        /// </summary>
        private StateMachine Machine;

        /// <summary>
        /// 状态枚举-状态ID字典
        /// </summary>
        private Dictionary<T, int> StateID;

        /// <summary>
        /// 状态保持方法
        /// </summary>
        protected void UpdateState()
        {
            if (Machine != null)
            {
                Machine.Update();
            }
        }

        /// <summary>
        /// 添加状态方法，第一次调用时所传状态为FSM默认状态
        /// </summary>
        /// <param name="stateEnum">状态枚举</param>
        /// <param name="state">状态对象</param>
        protected void AddStateInStateMachine(T stateEnum, StateBase state)
        {
            if (StateID == null)
            {
                StateID = new Dictionary<T, int>();
            }

            //当状态id跟状态枚举都未在字典中注册过时，允许注册状态//，否则发送错误警告
            if (!StateID.ContainsKey(stateEnum) && !StateID.ContainsValue(state.ID))
            {
                StateID.Add(stateEnum, state.ID);
            }
            // else
            // {
            //     Debug.LogError(
            //         $"This state cannot be added to the States dictionary of the FSMController, and id: {state.ID} or state enum: {stateEnum} is duplicated");
            // }

            if (Machine != null)
            {
                Machine.AddState(state);
            }
            else
            {
                //创建状态机
                Machine = new StateMachine(state);
                CurrentStateID = state.ID;
            }
        }

        /// <summary>
        /// 转换状态方法
        /// </summary>
        /// <param name="id">状态id</param>
        public void TranslateState(int id)
        {
            Machine.TranslateState(id);
            CurrentStateID = id;
        }

        /// <summary>
        /// 转换状态方法
        /// </summary>
        /// <param name="stateEnum">状态枚举</param>
        public void TranslateState(T stateEnum)
        {
            Machine.TranslateState(StateID[stateEnum]);
            CurrentStateID = StateID[stateEnum];
        }

        /// <summary>
        /// 获取当前状态ID
        /// </summary>
        /// <returns></returns>
        public int GetCurrentStateID()
        {
            return CurrentStateID;
        }
    }
}