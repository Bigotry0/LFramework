using LFramework.AI.Utility;

namespace LFramework.AI.Example.CountAPP
{
    /// <summary>
    /// 框架类，创建时需继承Architecture类，并传入自身作为泛型参数
    /// </summary>
    public class CountAPP : Architecture<CountAPP>
    {
        /// <summary>
        /// 框架初始化方法，从抽象父类所继承，必须实现。在该方法内进行所需模块的注册操作
        /// </summary>
        protected override void Init()
        {
            //建议按System，Model，Utility顺序分层注册，代码可读性更高，如以下
            
            //注册System模块
            RegisterSystem<IAchievementSystem>(new AchievementSystem());
            
            //注册Model模块
            RegisterModel<ICountModel>(new CountModel());
            
            //注册Utility模块
            RegisterUtility<IStorage>(new PlayerPrefsStorage());
        }
    }
}