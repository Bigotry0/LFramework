#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LFramework.Kit.DialogueSystem
{
    public class RandomDialogNodeView : NodeViewBase
    {
        private int nextIndex = 0;

        public RandomDialogNodeView(DialogNodeDataBase dialogNodeData) : base(dialogNodeData)
        {
            title = "RandomDialogNode";

            Port input = GetPortForNode(this, Direction.Input, Port.Capacity.Multi);
            Port output = GetPortForNode(this, Direction.Output, Port.Capacity.Single);
            input.portName = "Input";
            input.portColor = Color.magenta;
            output.portName = "Output";
            output.portColor = Color.magenta;
            output.name = "0";

            inputContainer.Add(input);
            outputContainer.Add(output);
            
            //工具条
            Toolbar toolbar = new Toolbar();
            ToolbarButton addButton = new ToolbarButton(AddTextField)
            {
                text = "Add"
            };
            ToolbarButton delButton = new ToolbarButton(DeleteTextField)
            {
                text = "Del"
            };
            toolbar.Add(addButton);
            toolbar.Add(delButton);
            
            toolbar.style.flexDirection = FlexDirection.RowReverse;
            contentContainer.Add(toolbar);

            while (nextIndex < DialogNodeData.OutputItems.Count)
            {
                AddTextField();
            }
            
            // 纪念一波旧方案
            // //创建序列化对象
            // SerializedObject serializedObject = new SerializedObject(DialogNodeData);
            // SerializedProperty serializedProperty = serializedObject.FindProperty("OutputItems");
            //
            // PropertyField propertyField = new PropertyField();
            // propertyField.style.maxHeight = 150;
            // propertyField.BindProperty(serializedProperty);
            //
            // this.extensionContainer.Add(propertyField);
            // RefreshExpandedState();
            
            
            //加个判断，不然每开一次创一个
            if (DialogNodeData.ChildNode.Count < 1)
            {
                DialogNodeData.ChildNode.Add(null);
            }
            //this.RefreshPorts();
        }
        
        public void AddTextField()
        {
            if (DialogNodeData.OutputItems.Count < nextIndex + 1)
            {
                DialogNodeData.OutputItems.Add(default);
            }

            Button background = new Button();

            TextField textField = new TextField();
            textField.name = nextIndex.ToString();
            textField.style.minWidth = 160;
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
            extensionContainer.Add(background);
            RefreshExpandedState();

            nextIndex++;
        }
        
        public void DeleteTextField()
        {
            if (nextIndex > 0)
            {
                nextIndex--;
                
                DialogNodeData.OutputItems.RemoveAt(DialogNodeData.OutputItems.Count - 1);
                extensionContainer.Remove(extensionContainer[nextIndex]);
            }
        }
    }
}
#endif