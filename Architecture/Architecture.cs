using System;
using System.Collections.Generic;

namespace LFramework
{
    /// <summary>
    /// 架构接口
    /// </summary>
    public interface IArchitecture
    {
        //注册System模块
        void RegisterSystem<T>(T system) where T : ISystem;

        //注册Model模块
        void RegisterModel<T>(T model) where T : IModel;

        //注册Utility模块
        void RegisterUtility<T>(T utility) where T : IUtility;

        //获取System
        T GetSystem<T>() where T : class, ISystem;

        //获取Model
        T GetModel<T>() where T : class, IModel;

        //获取Utility
        T GetUtility<T>() where T : class, IUtility;

        //发送命令
        void SendCommand<T>() where T : ICommand, new();
        void SendCommand<T>(T command) where T : ICommand;

        //发送事件
        void SendEvent<T>() where T : new();
        void SendEvent<T>(T e);

        //注册事件
        IUnRegister RegisterEvent<T>(Action<T> onEvent);

        //注销事件
        void UnRegisterEvent<T>(Action<T> onEvent);
    }

    /// <summary>
    /// 框架
    /// </summary>
    public abstract class Architecture<T> : IArchitecture where T : Architecture<T>, new()
    {
        /// <summary>
        /// 框架是否初始化完成 
        /// </summary>
        private bool IsInited = false;

        /// <summary>
        /// 用于缓存要初始化的Model
        /// </summary>
        private List<IModel> Models = new List<IModel>();

        /// <summary>
        /// 用于缓存要初始化的System
        /// </summary>
        private List<ISystem> Systems = new List<ISystem>();

        /// <summary>
        /// 类似单例模式，但该实例仅可在内部使用
        /// </summary>
        private static T ArchitectureInstance = null;

        /// <summary>
        /// 框架对象（单例）
        /// </summary>
        public static IArchitecture Interface
        {
            get
            {
                if (ArchitectureInstance == null)
                {
                    MakeSureArchitecture();
                }

                return ArchitectureInstance;
            }
        }

        /// <summary>
        /// 用于增加注册的委托
        /// </summary>
        public static Action<T> OnRegisterPatch;

        /// <summary>
        /// 确保框架和IOC容器是有实例的
        /// </summary>
        private static void MakeSureArchitecture()
        {
            if (ArchitectureInstance == null)
            {
                ArchitectureInstance = new T();
                //注册模块
                ArchitectureInstance.Init();

                //发出模块注册补丁委托，用于在Model和System模块初始化前进行模块变更等配置
                OnRegisterPatch?.Invoke(ArchitectureInstance);

                //Model层位于System层底层，所以必须在System层之前完成初始化，因为System层可在初始化时便访问Model层
                //初始化Model
                foreach (var architectureModel in ArchitectureInstance.Models)
                {
                    //调用Model模块初始化方法
                    architectureModel.Init();
                }

                //清空待初始化Model模块缓存
                ArchitectureInstance.Models.Clear();

                //初始化System
                foreach (var architectureSystem in ArchitectureInstance.Systems)
                {
                    //调用System模块初始化方法
                    architectureSystem.Init();
                }

                //清空待初始化System模块缓存
                ArchitectureInstance.Systems.Clear();

                //框架初始化完成
                ArchitectureInstance.IsInited = true;
            }
        }

        /// <summary>
        /// 初始化框架
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// 创建IOC容器
        /// </summary>
        private IOCContainer Container = new IOCContainer();
        
        /// <summary>
        /// 注册System模块API
        /// </summary>
        /// <param name="system"></param>
        /// <typeparam name="T"></typeparam>
        public void RegisterSystem<T>(T system) where T : ISystem
        {
            system.SetArchitecture(this);
            Container.Register<T>(system);

            if (!ArchitectureInstance.IsInited)
            {
                //若框架未初始化，则使用Systems统一初始化模块
                Systems.Add(system);
            }
            else
            {
                system.Init();
            }
        }

        /// <summary>
        /// 注册Model模块API
        /// </summary>
        /// <param name="model"></param>
        /// <typeparam name="T"></typeparam>
        public void RegisterModel<T>(T model) where T : IModel
        {
            model.SetArchitecture(this);
            Container.Register<T>(model);

            if (!ArchitectureInstance.IsInited)
            {
                //若框架未初始化，则使用Models统一初始化模块
                Models.Add(model);
            }
            else
            {
                model.Init();
            }
        }

        /// <summary>
        /// 注册Utility模块
        /// </summary>
        /// <param name="utility"></param>
        /// <typeparam name="T"></typeparam>
        public void RegisterUtility<T>(T utility) where T : IUtility
        {
            ArchitectureInstance.Container.Register<T>(utility);
        }

        /// <summary>
        /// 获取System模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSystem<T>() where T : class, ISystem
        {
            return Container.Get<T>();
        }

        /// <summary>
        /// 获取Model模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetModel<T>() where T : class, IModel
        {
            return Container.Get<T>();
        }

        /// <summary>
        /// 获取Utility模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUtility<T>() where T : class, IUtility
        {
            return Container.Get<T>();
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SendCommand<T>() where T : ICommand, new()
        {
            var command = new T();
            command.SetArchitecture(this);
            command.Execute();
            //去除双向引用
            command.SetArchitecture(null);
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="command"></param>
        /// <typeparam name="T"></typeparam>
        public void SendCommand<T>(T command) where T : ICommand
        {
            command.SetArchitecture(this);
            command.Execute();
            //去除双向引用
            command.SetArchitecture(null);
        }

        //创建事件系统
        private ITypeEventSystem TypeEventSystem = new TypeEventSystem();

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SendEvent<T>() where T : new()
        {
            TypeEventSystem.Send<T>();
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="e"></param>
        /// <typeparam name="T"></typeparam>
        public void SendEvent<T>(T e)
        {
            TypeEventSystem.Send<T>(e);
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="onEvent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IUnRegister RegisterEvent<T>(Action<T> onEvent)
        {
            return TypeEventSystem.Register<T>(onEvent);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="onEvent"></param>
        /// <typeparam name="T"></typeparam>
        public void UnRegisterEvent<T>(Action<T> onEvent)
        {
            TypeEventSystem.UnRegister<T>(onEvent);
        }
    }
}