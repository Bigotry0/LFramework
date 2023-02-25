#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LFramework.Kit.DialogueSystem
{
    public class EndNodeView : NodeViewBase
    {
        public EndNodeView(DialogNodeDataBase dialogNodeData) : base(dialogNodeData)
        {
            title = "End";
            
            Port input = GetPortForNode(this, Direction.Input, Port.Capacity.Multi);
            input.portName = "input";
            input.portColor = Color.gray;
            
            inputContainer.Add(input);
        }
    }
}
#endif