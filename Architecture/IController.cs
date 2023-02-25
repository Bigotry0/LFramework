using UnityEditor;
using UnityEngine;

namespace LFramework
{
    /// <summary>
    /// 表现层的IController接口，Controller模块应继承此接口,并自行阉割GetArchitecture()
    /// </summary>
    public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetSystem,
        ICanGetModel,
        ICanRegisterEvent
    {
        
    }
    
     // public class MonoBehaviourController : MonoBehaviour, IController
     // {
     //     private IArchitecture Architecture;
     //
     //     IArchitecture IBelongToArchitecture.GetArchitecture()
     //     {
     //         return Architecture;
     //     }
     //
     //     void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
     //     {
     //         Architecture = architecture;
     //     }
     // }
     //
     // public class EditorWindowController : EditorWindow, IController
     // {
     //     private IArchitecture Architecture;
     //
     //     IArchitecture IBelongToArchitecture.GetArchitecture()
     //     {
     //         return Architecture;
     //     }
     //
     //     void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
     //     {
     //         Architecture = architecture;
     //     }
     // }
}