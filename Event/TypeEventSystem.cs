using System;
using System.Collections.Generic;
using UnityEngine;

namespace LFramework
{
    /// <summary>
    /// 事件系统接口
    /// </summary>
    public interface ITypeEventSystem
    {
        void Send<T>() where T : new();
        void Send<T>(T e);
        IUnRegister Register<T>(Action<T> onEvent);
        void UnRegister<T>(Action<T> onEvent);
    }

    /// <summary>
    /// 注销接口
    /// </summary>
    public interface IUnRegister
    {
        void UnRegister();
    }

    /// <summary>
    /// 注销信息泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct TypeEventSystemUnRegister<T> : IUnRegister
    {
        public ITypeEventSystem TypeEventSystem;
        public Action<T> OnEvent;

        public void UnRegister()
        {
            TypeEventSystem.UnRegister<T>(OnEvent);

            TypeEventSystem = null;

            OnEvent = null;
        }
    }

    /// <summary>
    /// Go对象Destroy时注销事件触发器，该类继承MonoBehaviour
    /// </summary>
    public class UnRegisterOnDestroyTrigger : MonoBehaviour
    {
        private HashSet<IUnRegister> UnRegisters = new HashSet<IUnRegister>();

        public void AddUnRegister(IUnRegister unRegister)
        {
            UnRegisters.Add(unRegister);
        }

        private void OnDestroy()
        {
            foreach (var unRegister in UnRegisters)
            {
                unRegister.UnRegister();
            }

            UnRegisters.Clear();
        }
    }

    /// <summary>
    /// Go对象Disable时注销事件触发器，该类继承MonoBehaviour
    /// </summary>
    public class UnRegisterOnDisableTrigger : MonoBehaviour
    {
        private HashSet<IUnRegister> UnRegisters = new HashSet<IUnRegister>();

        public void AddUnRegister(IUnRegister unRegister)
        {
            UnRegisters.Add(unRegister);
        }

        private void OnDisable()
        {
            foreach (var unRegister in UnRegisters)
            {
                unRegister.UnRegister();
            }

            UnRegisters.Clear();
        }
    }

    public static class UnRegisterExtension
    {
        /// <summary>
        /// 在MonoBehaviour脚本Destory时注销事件
        /// </summary>
        /// <param name="unRegister"></param>
        /// <param name="gameObject"></param>
        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister, GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
            }

            trigger.AddUnRegister(unRegister);
        }

        /// <summary>
        /// 在MonoBehaviour脚本Disable时注销事件
        /// </summary>
        /// <param name="unRegister"></param>
        /// <param name="gameObject"></param>
        public static void UnRegisterWhenGameObjectDisable(this IUnRegister unRegister, GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnRegisterOnDisableTrigger>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<UnRegisterOnDisableTrigger>();
            }

            trigger.AddUnRegister(unRegister);
        }
    }

    /// <summary>
    /// 基于类型的事件系统
    /// </summary>
    public class TypeEventSystem : ITypeEventSystem
    {
        public interface IRegistrations
        {
        }

        /// <summary>
        /// 事件储存泛型类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Registrations<T> : IRegistrations
        {
            public Action<T> OnEvent = e => { };
        }

        //储存不同类型的事件
        private Dictionary<Type, IRegistrations> EventRegistration = new Dictionary<Type, IRegistrations>();

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Send<T>() where T : new()
        {
            var e = new T();
            Send<T>(e);
        }

        public void Send<T>(T e)
        {
            var type = typeof(T);
            IRegistrations registrations;

            if (EventRegistration.TryGetValue(type, out registrations))
            {
                (registrations as Registrations<T>).OnEvent(e);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="onEvent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IUnRegister Register<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegistrations registrations;

            if (EventRegistration.TryGetValue(type, out registrations))
            {
            }
            else
            {
                registrations = new Registrations<T>();
                EventRegistration.Add(type, registrations);
            }

            (registrations as Registrations<T>).OnEvent += onEvent;

            return new TypeEventSystemUnRegister<T>()
            {
                OnEvent = onEvent,
                TypeEventSystem = this
            };
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="onEvent"></param>
        /// <typeparam name="T"></typeparam>
        public void UnRegister<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegistrations registrations;

            if (EventRegistration.TryGetValue(type, out registrations))
            {
                (registrations as Registrations<T>).OnEvent -= onEvent;
            }
        }
    }
}