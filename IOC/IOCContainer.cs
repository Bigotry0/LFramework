using System;
using System.Collections.Generic;

namespace LFramework
{
    public class IOCContainer
    {
        /// <summary>
        /// 实例字典
        /// </summary>
        private Dictionary<Type, object> Instances = new Dictionary<Type, object>();

        /// <summary>
        /// 注册容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public void Register<T>(T instance)
        {
            var Key = typeof(T);

            if (Instances.ContainsKey(Key))
            {
                Instances[Key] = instance;
            }
            else
            {
                Instances.Add(Key, instance);
            }
        }

        /// <summary>
        /// 获取容器值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class
        {
            var Key = typeof(T);

            if (Instances.TryGetValue(Key, out var instance))
            {
                return instance as T;
            }

            return null;
        }
    }
}