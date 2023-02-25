#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LFramework.Kit.DialogueSystem
{
    public class StartNodeView : NodeViewBase
    {
        public StartNodeView(DialogNodeDataBase dialogNodeData) : base(dialogNodeData)
        {
            title = "Start";
            
            Port output = GetPortForNode(this, Direction.Output, Port.Capacity.Single);
            output.portName = "output";
            output.name = "0";
            
            output.portColor = Color.green;
            outputContainer.Add(output);
            
            if (DialogNodeData.ChildNode.Count < 1)
            {
                DialogNodeData.ChildNode.Add(null);
            }
        }
    }
}
#endif