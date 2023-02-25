using System;
using System.Reflection;

namespace LFramework.AI
{
    /// <summary>
    /// 单例泛型类，用于创建一个单例类，该类必须有一个私有的无参构造函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : class
    {
        private static T _Instance;

        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    var Type = typeof(T);
                    var Constructors = Type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    var Constructor = Array.Find(Constructors, C => C.GetParameters().Length == 0);

                    if (Constructor == null)
                    {
                        //若继承类中无私有无参构造函数，抛出异常
                        throw new Exception("Non-Public Constructor() not found in" + typeof(T));
                    }

                    _Instance = Constructor.Invoke(null) as T;
                }

                return _Instance;
            }
        }
    }
}
