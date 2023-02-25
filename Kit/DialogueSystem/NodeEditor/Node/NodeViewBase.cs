#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LFramework.Kit.DialogueSystem
{
    public abstract class NodeViewBase : Node
    {
        public Action<NodeViewBase> OnNodeSelected;
        
        public string GUID;

        //对话数据
        public DialogNodeDataBase DialogNodeData = null;

        public NodeViewBase(DialogNodeDataBase dialogNodeData) : base()
        {
            GUID = Guid.NewGuid().ToString();
            DialogNodeData = dialogNodeData;
            EditorUtility.SetDirty(DialogNodeData);
        }

        public Port GetPortForNode(NodeViewBase node, Direction portDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(bool));
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public override Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            //return base.InstantiatePort(orientation, direction, capacity, type);
            return Port.Create<FlowingEdge>(orientation, direction, capacity, type);
        }
    }
}
#endif