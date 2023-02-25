using UnityEngine;

namespace LFramework.AI.Example.CountAPP
{
    public interface IAchievementSystem : ISystem
    {
    }

    /// <summary>
    /// System模块，需继承AbstractSystem抽象类，实现其OnInit抽象方法，并在对应框架类中进行注册
    /// 可在System模块中进行一些具有状态的字段的监听处理等
    /// </summary>
    public class AchievementSystem : AbstractSystem, IAchievementSystem
    {
        protected override void OnInit()
        {
            //获取模块
            var countModel = this.GetModel<ICountModel>();
            
            //监听Count计数是否大于10
            countModel.Count.RegisterOnValueChange(value =>
            {
                if (value > 10)
                {
                    Debug.Log(">10");
                }
            });
        }
    }
}