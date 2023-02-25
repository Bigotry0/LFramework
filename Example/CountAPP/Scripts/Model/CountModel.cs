using LFramework.AI.Utility;

namespace LFramework.AI.Example.CountAPP
{
    public interface ICountModel : IModel
    {
        BindableProperty<int> Count { get; }
    }
    
    /// <summary>
    /// System模块，需继承AbstractModel抽象类，实现其OnInit抽象方法，并在对应框架类中进行注册
    /// 用于储存数据
    /// </summary>
    public class CountModel : AbstractModel,ICountModel
    {
        protected override void OnInit()
        {
            //初始化该Model，如读取存档等。
            //这里进行IStorage工具模块的获取
            var storage = this.GetUtility<IStorage>();
            
            //对可绑定属性赋值
            Count.Value = storage.LoadInt("COUNT", 0);

            //监听Count值变化
            Count.RegisterOnValueChange(v => { storage.SaveInt("COUNT", v); });
        }

        /// <summary>
        /// 可绑定属性，内封装了一个用于储存数据的属性，以及用于监听数据的值是否变化的委托，当值改变时，将触发该委托
        /// 该委托的监听已封装成RegisterOnValueChange(Action<T> onValueChange)方法
        /// 在其泛型参数里传入所需的属性类型，并可为该属性赋予初值，如下代码
        /// </summary>
        public BindableProperty<int> Count { get; } = new BindableProperty<int>()
        {
            Value = 0
        };
    }
}