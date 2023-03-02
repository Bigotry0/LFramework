#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace LFramework.Kit.DialogueSystem
{
    public class DialogGraphWindow : EditorWindow
    {
        // [MenuItem("Window/UI Toolkit/DialogueView")]
        // public static void ShowExample()
        // {
        //
        //     DialogueView wnd = GetWindow<DialogueView>();
        //     wnd.titleContent = new GUIContent("DialogueView");
        // }

        //public static GameObject userSelectionGo;

        private DialogGraphView _graphView = null;
        private InspectorView _inspectorView = null;

        [OnOpenAsset(1)]
        public static bool OnOpenAsssets(int id, int line)
        {
            if (EditorUtility.InstanceIDToObject(id) is DialogTree tree)
            {
                //打开不同文件
                if (DialogGraphView.treeData != tree)
                {
                    DialogGraphView.treeData = tree;

                    //判断窗口是否打开
                    if (HasOpenInstances<DialogGraphWindow>())
                    {
                        //Debug.Log(true);
                        CloseEditorWindow();
                    }

                    //大大大大大坑！新版本unity不自动在磁盘上应用资源更新，必须先给目标物体打上Dirty标记
                    EditorUtility.SetDirty(tree);
                }

                DialogGraphWindow wnd = GetWindow<DialogGraphWindow>();
                wnd.titleContent = new GUIContent("DialogueView");
                
                //Debug.Log("Open");
                return true;
            }
            return false;
        }

        public static void CloseEditorWindow()
        {
            DialogGraphWindow wnd = GetWindow<DialogGraphWindow>();
            wnd.Close();
        }

        public void CreateGUI()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/LFramework/Kit/DialogueSystem/NodeEditor/EditorWindow/DialogGraphWindow.uxml");
            visualTree.CloneTree(rootVisualElement);

            _graphView = rootVisualElement.Q<DialogGraphView>("DialogGraphView");

            _inspectorView = rootVisualElement.Q<InspectorView>("InspectorView");

            var saveButton = rootVisualElement.Q<ToolbarButton>("SaveButton");
            saveButton.clicked += OnSaveButtonClicked;

            //初始化节点图
            DialogGraphView.Instance.ResetNodeView();
        }

        //保存资源文件
        private void OnSaveButtonClicked()
        {
            //AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // private void OnDestroy()
        // {
        //     //死之前记得保存一下，保险哈
        //     OnSaveButtonClicked();
        //     
        // }
    }
}
#endif