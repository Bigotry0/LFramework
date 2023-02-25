#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LFramework.Kit.DialogueSystem
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits>
        {
        }

        Editor _editor;

        public InspectorView()
        {
            
        }

        internal void UpdateSelection(NodeViewBase nodeView)
        {
            Clear();
            Debug.Log("显示节点的Inspector面板");
            UnityEngine.Object.DestroyImmediate(_editor);
            _editor = Editor.CreateEditor(nodeView.DialogNodeData);

            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (nodeView.DialogNodeData != null)
                {
                    _editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}
#endif