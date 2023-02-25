using LFramework.AI.Utility;
using UnityEditor;
using UnityEngine;

namespace LFramework.AI.Example.CountAPP
{
    /// <summary>
    /// 编辑器组件测试，请暂且忽略
    /// </summary>
    public class EditorCountApp : EditorWindow, IController
    {
        [MenuItem("EditorCountApp/Open")]
        static void Open()
        {
            CountAPP.OnRegisterPatch += app =>
            {
                app.RegisterUtility<IStorage>(new EditorPrefsStorage());
            };
            
            var window = GetWindow<EditorCountApp>();
            window.position = new Rect(100, 100, 400, 600);
            window.titleContent = new GUIContent(nameof(EditorCountApp));
            window.Show();
        }

        private void OnGUI()
        {
            var countModel = this.GetModel<ICountModel>();
            
            if (GUILayout.Button("+"))
            {
                this.SendCommand<AddButtonCommand>();
            }

            GUILayout.Label(countModel.Count.Value.ToString());
            
            if (GUILayout.Button("-"))
            {
                this.SendCommand<DesButtonCommand>();
            }
        }

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return CountAPP.Interface;
        }
    }
}