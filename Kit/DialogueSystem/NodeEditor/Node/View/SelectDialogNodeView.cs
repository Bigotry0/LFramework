#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LFramework.Kit.DialogueSystem
{
    public class SelectDialogNodeView : NodeViewBase
    {
        private int nextIndex = 0;

        public SelectDialogNodeView(DialogNodeDataBase dialogNodeData) : base(dialogNodeData)
        {
            title = "SelectDialogNode";

            Port input = GetPortForNode(this, Direction.Input, Port.Capacity.Multi);
            input.portName = "Input";
            input.portColor = Color.cyan;
            inputContainer.Add(input);

            //工具条
            Toolbar toolbar = new Toolbar();
            ToolbarButton addButton = new ToolbarButton(AddOutputPort)
            {
                text = "Add"
            };
            ToolbarButton delButton = new ToolbarButton(DeleteOutputPort)
            {
                text = "Del"
            };
            toolbar.Add(addButton);
            toolbar.Add(delButton);
            
            toolbar.style.flexDirection = FlexDirection.RowReverse;

            extensionContainer.Add(toolbar);
            RefreshExpandedState();

            while (nextIndex < DialogNodeData.ChildNode.Count)
            {
                AddOutputPort();
            }
        }

        public void AddOutputPort()
        {
            if (DialogNodeData.OutputItems.Count < nextIndex + 1)
            {
                DialogNodeData.OutputItems.Add(default);
            }

            Button background = new Button();
            Port output = GetPortForNode(this, Direction.Output, Port.Capacity.Single);
            output.name = nextIndex.ToString();
            output.portName = "Output";
            output.portColor = Color.cyan;

            TextField textField = new TextField();
            textField.name = nextIndex.ToString();
            textField.style.minWidth = 100;
            //初始化
            textField.SetValueWithoutNotify(DialogNodeData.OutputItems[nextIndex]);

            textField.RegisterValueChangedCallback(evt =>
            {
                if (int.TryParse(textField.name, out int index))
                {
                    DialogNodeData.OutputItems[index] = evt.newValue;
                }
                else
                {
                    Debug.LogError("textField.name(string) to int fail");
                }
            });

            background.Add(textField);
            textField.Add(output);
            outputContainer.Add(background);
            RefreshPorts();

            if (DialogNodeData.ChildNode.Count < nextIndex + 1)
            {
                DialogNodeData.ChildNode.Add(null);
            }

            nextIndex++;
        }

        public void DeleteOutputPort()
        {
            if (nextIndex > 0)
            {
                nextIndex--;

                DialogNodeData.ChildNode.RemoveAt(DialogNodeData.ChildNode.Count - 1);
                DialogNodeData.OutputItems.RemoveAt(DialogNodeData.OutputItems.Count - 1);
                outputContainer.Remove(outputContainer[nextIndex]);
            }
        }
    }
}
#endif