using UnityEngine;

namespace LFramework.AI.Example.EventExample
{
    public class EventText : MonoBehaviour, IController
    {
        //发送事件
        public void SendEvent()
        {
            this.SendCommand<SendEventCommand>();
        }
        
        void Start()
        {
            //!注册事件!需传入Action<T>参数，即以事件类为唯一形数的函数
            //以下写法均为事件注册写法
            //!事件注册后应当处理好注销操作，事件系统中封装了用于快速实现注销的函数，如下方使用
            
            //直接注册，需手动注销，这里在OnDestroy里注销
            this.RegisterEvent<EventA>(OnEventA);

            //注册完调用API进行注销配置
            this.RegisterEvent<EventB>(e => { Debug.Log($"From EventB Get a = {e.a},b = {e.b}."); })
                .UnRegisterWhenGameObjectDisable(gameObject);   //在该组件Disable时注销事件

            this.RegisterEvent<AGroup>(e => { Debug.Log($"{e} Event Belong to AGroup."); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);     //在该组件Destroy时注销事件
        }
        
        public void OnEventA(EventA e)
        {
            Debug.Log("EventA");
        }

        void OnDestroy()
        {
            //注销EventA
            this.UnRegisterEvent<EventA>(OnEventA);
        }

        //注册该Controller模块
        public IArchitecture GetArchitecture()
        {
            return EventExample.Interface;
        }
    }
}