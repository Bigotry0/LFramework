using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LFramework.Kit.DialogueSystem
{
    public class CharacterSwitchNodeView : NodeViewBase
    {
        public CharacterSwitchNodeView(DialogNodeDataBase dialogNodeData) : base(dialogNodeData)
        {
            title = "CharacterSwitchNode";

            Port input = GetPortForNode(this, Direction.Input, Port.Capacity.Multi);
            Port output = GetPortForNode(this, Direction.Output, Port.Capacity.Single);
            input.portName = "Input";
            input.portColor = Color.white;
            output.portName = "Output";
            output.portColor = Color.white;
            output.name = "0";

            inputContainer.Add(input);
            outputContainer.Add(output);

            extensionContainer.style.flexDirection = FlexDirection.Row;
            
            Label label = new Label("Name:")
            {
                style =
                {
                    alignSelf = Align.Center
                }
            };
            extensionContainer.Add(label);
            
            TextField textField = new TextField();
            //textField.label = "EventName:";
            textField.style.minWidth = 120;
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