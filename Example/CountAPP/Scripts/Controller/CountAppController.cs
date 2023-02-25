using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LFramework.AI.Example.CountAPP
{
    /// <summary>
    /// Controller模块，需继承IController接口，且必须显式实现接口内的GetArchitecture()方法，方法内返回所需框架类的Interface属性
    /// 如以下实例：
    /// 
    /// IArchitecture IBelongToArchitecture.GetArchitecture()
    /// {
    ///     return CountAPP.Interface;
    /// }
    /// 
    /// 此处用到的CountAPP为实例所创建的框架类，返回的框架类决定了该Controller所属框架，并可访问在权限允许内的框架内模块。
    /// </summary>
    public class CountAppController : MonoBehaviour, IController
    {
        private ICountModel CountModel;

        public Button AddButton;
        public Button DesButton;
        public TMP_Text CountUI;

        void Awake()
        {
            //获取模块
            CountModel = this.GetModel<ICountModel>();
            
            CountUI.text = CountModel.Count.Value.ToString();

            CountModel.Count.RegisterOnValueChange(i => { CountUI.text = CountModel.Count.Value.ToString(); });
            AddButton.onClick.AddListener(() => { this.SendCommand<AddButtonCommand>(); });
            DesButton.onClick.AddListener((() => { this.SendCommand(new DesButtonCommand()); }));
        }

        private void OnDestroy()
        {
            AddButton.onClick.RemoveAllListeners();
        }

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return CountAPP.Interface;
        }
    }
}