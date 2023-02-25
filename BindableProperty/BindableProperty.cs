using System;
using System.Collections.Generic;

namespace LFramework
{
    /// <summary>
    /// 可绑定属性工具类，用于定义一个带值监听事件的属性。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindableProperty<T> where T : IEquatable<T>
    {
        private T _Value = default(T);

        public T Value
        {
            get => _Value;
            set
            {
                if (!value.Equals(_Value))
                {
                    _Value = value;
                    //Value值改变时发出委托，同时传出改变后的值
                    OnValueChanged?.Invoke(_Value);
                }
            }
        }

        private Action<T> OnValueChanged = v => { };

        /// <summary>
        /// 注册BindableProperty值变化事件
        /// </summary>
        /// <param name="onValueChange"></param>
        /// <returns></returns>
        public IUnRegister RegisterOnValueChange(Action<T> onValueChange)
        {
            OnValueChanged += onValueChange;
            return new BindablePropertyUnRegister<T>()
            {
                BindableProperty = this,
                OnValueChanged = onValueChange
            };
        }

        /// <summary>
        /// 注销BindableProperty值变化事件
        /// </summary>
        /// <param name="onValueChanged"></param>
        public void UnRegisterOnValueChanged(Action<T> onValueChanged)
        {
            OnValueChanged -= onValueChanged;
        }

        /// <summary>
        /// 注销所有BindableProperty值变化事件
        /// </summary>
        public void UnRegisterAllOnValueChangedEvent()
        {
            if (OnValueChanged != null)
            {
                OnValueChanged -= this.OnValueChanged;
            }

            OnValueChanged += v => { };
        }
    }

    public class BindablePropertyUnRegister<T> : IUnRegister where T : IEquatable<T>
    {
        public BindableProperty<T> BindableProperty { get; set; }
        public Action<T> OnValueChanged { get; set; }
        
        public void UnRegister()
        {
            BindableProperty.UnRegisterOnValueChanged(OnValueChanged);

            BindableProperty = null;
            OnValueChanged = null;
        }
    }
}