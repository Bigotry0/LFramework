using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace LFramework.Kit.DialogueSystem
{
    public class EventNodeView : NodeViewBase
    {
        public EventNodeView(DialogNodeDataBase dialogNodeData) : base(dialogNodeData)
        {
            title = "EventNode";

            Port input = GetPortForNode(this, Direction.Input, Port.Capacity.Multi);
            Port output = GetPortForNode(this, Direction.Output, Port.Capacity.Single);
            input.portName = "Input";
            input.portColor = Color.red;
            output.portName = "Output";
            output.portColor = Color.red;
            output.name = "0";

            inputContainer.Add(input);
            outputContainer.Add(output);

            extensionContainer.style.flexDirection = FlexDirection.Row;
            
            Label label = new Label("EventName:");
            label.style.alignSelf = Align.Center;
            extensionContainer.Add(label);
            
            TextField textField = new TextField();
            //textField.label = "EventName:";
            textField.style.minWidth = 50;
            //初始化
            if (DialogNodeData.OutputItems.Count < 1)
            {
                DialogNodeData.OutputItems.Add(null);
            }
            textField.SetValueWithoutNotify(DialogNodeData.OutputItems[0]);

            textField.RegisterValueChangedCallback(evt => { DialogNodeData.OutputItems[0] = evt.newValue; });
            
            extensionContainer.Add(textField);
            RefreshExpandedState();
            
            if (DialogNodeData.ChildNode.Count < 1)
            {
                DialogNodeData.ChildNode.Add(null);
            }
        }
    }
}