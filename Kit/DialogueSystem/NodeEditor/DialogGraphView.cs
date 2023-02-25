#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LFramework.Kit.DialogueSystem
{
    public class DialogGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<DialogGraphView, GraphView.UxmlTraits>
        {
        }

        public static DialogTree treeData = null;

        public static DialogGraphView Instance;

        /// <summary>
        /// 节点点击事件
        /// </summary>
        public Action<NodeViewBase> OnNodeSelected;

        public DialogGraphView()
        {
            //监听视图Transform变化事件
            viewTransformChanged += OnViewTransformChanged;
            //监听graphView变化事件
            graphViewChanged += OnGraphViewChanged;
            //增加格子背景
            Insert(0, new GridBackground());

            //增加内容缩放，拖动，选择，框选控制器
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            //框选bug
            //大坑！控制器之间存在优先级
            //这就是为什么框选控制器放在选择拖放节点控制器之前会导致节点无法移动
            //因为框选的优先级更高操
            this.AddManipulator(new RectangleSelector());

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/LFramework/Kit/DialogueSystem/NodeEditor/EditorWindow/DialogueTreeView.uss");
            styleSheets.Add(styleSheet);

            //初始化treedata布局
            if (treeData != null)
            {
                contentViewContainer.transform.position = treeData.GraphViewData.Position;
                contentViewContainer.transform.scale = treeData.GraphViewData.Scale;
            }

            //单例哈
            Instance = this;
        }

        /// <summary>
        /// graphView的Transform发生变化时触发
        /// </summary>
        /// <param name="graphView"></param>
        private void OnViewTransformChanged(GraphView graphView)
        {
            if (treeData != null)
            {
                //保存视图Transform信息
                treeData.GraphViewData.Position = contentViewContainer.transform.position;
                treeData.GraphViewData.Scale = contentViewContainer.transform.scale;
                // Debug.Log($"Position:{contentViewContainer.transform.position}");
                // Debug.Log($"Scale:{contentViewContainer.transform.scale}");
            }
        }

        /// <summary>
        /// 节点图变化事件
        /// </summary>
        /// <param name="graphviewchange"></param>
        /// <returns></returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            if (graphviewchange.elementsToRemove != null)
            {
                graphviewchange.elementsToRemove.ForEach(elem =>
                {
                    //节点删除
                    if (elem is NodeViewBase nodeView)
                    {
                        if (nodeView.DialogNodeData.NodeType == NodeType.Start)
                        {
                            treeData.StartNodeData = null;
                        }
                        else
                        {
                            treeData.ChildNodeDataList.Remove(nodeView.DialogNodeData);
                        }

                        AssetDatabase.DeleteAsset(nodeView.DialogNodeData.Path);
                        //TODO:做缓存，并在保存的时候应用操作。//Fuck！！！又不是Mono脚本，资源删除不搞这些有的没的
                    }
                    //连线删除
                    else if (elem is Edge edge)
                    {
                        NodeViewBase parentNodeView = edge.output.node as NodeViewBase;
                        NodeViewBase childNodeView = edge.input.node as NodeViewBase;
                        if (parentNodeView != null && childNodeView != null)
                        {
                            if (int.TryParse(edge.output.name, out int index))
                            {
                                parentNodeView.DialogNodeData.ChildNode[index] = null;
                            }
                            else
                            {
                                Debug.LogError("Node.name(string) to int fail");
                            }
                        }
                    }
                });
            }

            if (graphviewchange.edgesToCreate != null)
            {
                //创建连线
                graphviewchange.edgesToCreate.ForEach(edge =>
                {
                    NodeViewBase parentNodeView = edge.output.node as NodeViewBase;
                    NodeViewBase childNodeView = edge.input.node as NodeViewBase;

                    if (parentNodeView != null && childNodeView != null)
                    {
                        if (int.TryParse(edge.output.name, out int index))
                        {
                            parentNodeView.DialogNodeData.ChildNode[index] = childNodeView.DialogNodeData;
                        }
                        else
                        {
                            Debug.LogError("Node.name(string) to int fail");
                        }
                    }
                });
            }

            //遍历节点，记录节点位置信息
            nodes.ForEach(node =>
            {
                NodeViewBase nodeView = node as NodeViewBase;
                if (nodeView != null && nodeView.DialogNodeData != null)
                {
                    nodeView.DialogNodeData.Position = nodeView.GetPosition().position;
                }
            });

            return graphviewchange;
        }

        //节点链接规则
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            //自己不能连自己，接口方向不能相同，接口类型要相同，都是表示对话数据而已，本对话系统所有接口类型都被我统一成bool
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node &&
                endPort.portType == startPort.portType
            ).ToList();
        }

        /// <summary>
        /// 链接两个点
        /// </summary>
        /// <param name="_outputPort">outputPort</param>
        /// <param name="_inputPort">inputPort</param>
        private void AddEdgeByPorts(Port _outputPort, Port _inputPort)
        {
            //虽然是不可能发生，但咱还是保守一点
            if (_outputPort.node == _inputPort.node)
            {
                return;
            }

            //用上某大佬酷拽吊炸天的数据流动效果接线，拜托他真的超帅的
            Edge tempEdge = new FlowingEdge()
            {
                input = _inputPort,
                output = _outputPort
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            Add(tempEdge);
        }

        /// <summary>
        /// 菜单点击时鼠标位置
        /// </summary>
        private Vector2 clickPosition;

        /// <summary>
        /// 右键菜单
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            Debug.Log(evt.mousePosition);

            //将鼠标世界坐标转为视图本地坐标
            clickPosition = contentViewContainer.WorldToLocal(evt.mousePosition);

            if (treeData.StartNodeData == null)
            {
                evt.menu.AppendAction("Create StartNode", x => { CreateNode(NodeType.Start, clickPosition); });
            }
            
            evt.menu.AppendAction("Create RandomDialogNode",
                x => { CreateNode(NodeType.RandomDialogNode, clickPosition); });
            evt.menu.AppendAction("Create SequentialDialogNode",
                x => CreateNode(NodeType.SequentialDialogNode, clickPosition));
            evt.menu.AppendAction("Create SelectDialogNode",
                x => CreateNode(NodeType.SelectDialogNode, clickPosition));
            evt.menu.AppendAction("Create EventNode", x => { CreateNode(NodeType.EventNode, clickPosition); });
            evt.menu.AppendAction("Create EndNode", x => { CreateNode(NodeType.End, clickPosition); });
            evt.menu.AppendAction("Create CharacterSwitchNode", x => { CreateNode(NodeType.CharacterSwitchNode, clickPosition); });   
        }


        //确保目录存在
        private void MakeSureTheFolder()
        {
            //TODO：做成可自行设置的对话资源文件部署
            if (!AssetDatabase.IsValidFolder("Assets/DialogueData/NodeData"))
            {
                AssetDatabase.CreateFolder("Assets", "DialogueData");
                AssetDatabase.CreateFolder("Assets/DialogueData", "NodeData");
            }
        }

        /// <summary>
        /// 新建节点
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        private void CreateNode(NodeType type, Vector2 position = default)
        {
            if (treeData == null)
            {
                return;
            }

            MakeSureTheFolder();
            NodeViewBase nodeView = null;

            //创建节点的核心，新增的节点需要在这里进行创建方式的添加
            switch (type)
            {
                case NodeType.Start:
                {
                    var dialogNodeData = ScriptableObject.CreateInstance<StartNodeData>();
                    dialogNodeData.Path = $"Assets/DialogueData/NodeData/StartData[{dialogNodeData.GetInstanceID()}].asset";
                    EditorUtility.SetDirty(dialogNodeData);

                    AssetDatabase.CreateAsset(dialogNodeData,
                        $"Assets/DialogueData/NodeData/StartData[{dialogNodeData.GetInstanceID()}].asset");

                    nodeView = new StartNodeView(dialogNodeData);
                    break;
                }
                case NodeType.RandomDialogNode:
                {
                    var dialogNodeData = ScriptableObject.CreateInstance<RandomDialogNodeData>();
                    dialogNodeData.Path = $"Assets/DialogueData/NodeData/RandomDialogData[{dialogNodeData.GetInstanceID()}].asset";
                    EditorUtility.SetDirty(dialogNodeData);

                    AssetDatabase.CreateAsset(dialogNodeData,
                        $"Assets/DialogueData/NodeData/RandomDialogData[{dialogNodeData.GetInstanceID()}].asset");

                    nodeView = new RandomDialogNodeView(dialogNodeData);
                    break;
                }
                case NodeType.SequentialDialogNode:
                {
                    var dialogNodeData = ScriptableObject.CreateInstance<SequentialDialogNodeData>();
                    dialogNodeData.Path =
                        $"Assets/DialogueData/NodeData/SequentialDialogData[{dialogNodeData.GetInstanceID()}].asset";
                    EditorUtility.SetDirty(dialogNodeData);

                    AssetDatabase.CreateAsset(dialogNodeData,
                        $"Assets/DialogueData/NodeData/SequentialDialogData[{dialogNodeData.GetInstanceID()}].asset");

                    nodeView = new SequentialDialogNodeView(dialogNodeData);
                    break;
                }
                case NodeType.SelectDialogNode:
                {
                    var dialogNodeData = ScriptableObject.CreateInstance<SelectDialogNodeData>();
                    dialogNodeData.Path =
                        $"Assets/DialogueData/NodeData/SelectDialogData[{dialogNodeData.GetInstanceID()}].asset";
                    EditorUtility.SetDirty(dialogNodeData);

                    AssetDatabase.CreateAsset(dialogNodeData,
                        $"Assets/DialogueData/NodeData/SelectDialogData[{dialogNodeData.GetInstanceID()}].asset");

                    nodeView = new SelectDialogNodeView(dialogNodeData);
                    break;
                }
                case NodeType.EventNode:
                {
                    var dialogNodeData = ScriptableObject.CreateInstance<EventNodeData>();
                    dialogNodeData.Path =
                        $"Assets/DialogueData/NodeData/EventData[{dialogNodeData.GetInstanceID()}].asset";
                    EditorUtility.SetDirty(dialogNodeData);

                    AssetDatabase.CreateAsset(dialogNodeData,
                        $"Assets/DialogueData/NodeData/EventData[{dialogNodeData.GetInstanceID()}].asset");

                    nodeView = new EventNodeView(dialogNodeData);
                    break;
                }
                case NodeType.End:
                {
                    var dialogNodeData = ScriptableObject.CreateInstance<EndNodeData>();
                    dialogNodeData.Path = $"Assets/DialogueData/NodeData/EndData[{dialogNodeData.GetInstanceID()}].asset";
                    EditorUtility.SetDirty(dialogNodeData);

                    AssetDatabase.CreateAsset(dialogNodeData,
                        $"Assets/DialogueData/NodeData/EndData[{dialogNodeData.GetInstanceID()}].asset");

                    nodeView = new EndNodeView(dialogNodeData);
                    break;
                }
                case NodeType.CharacterSwitchNode:
                {
                    var dialogNodeData = ScriptableObject.CreateInstance<CharacterSwitchNodeData>();
                    dialogNodeData.Path = $"Assets/DialogueData/NodeData/CharacterSwitchData[{dialogNodeData.GetInstanceID()}].asset";
                    EditorUtility.SetDirty(dialogNodeData);

                    AssetDatabase.CreateAsset(dialogNodeData,
                        $"Assets/DialogueData/NodeData/CharacterSwitchData[{dialogNodeData.GetInstanceID()}].asset");

                    nodeView = new CharacterSwitchNodeView(dialogNodeData);
                    break;
                }
                default:
                {
                    Debug.LogError("未找到该类型的节点");
                    break;
                }
            }

            //添加节点被选择事件
            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.SetPosition(new Rect(position, nodeView.GetPosition().size));

            //对Start节点做个特判哈
            if (nodeView.DialogNodeData.NodeType == NodeType.Start)
            {
                treeData.StartNodeData = nodeView.DialogNodeData;
            }
            else
            {
                treeData.ChildNodeDataList.Add(nodeView.DialogNodeData);
            }

            this.AddElement(nodeView);
        }

        /// <summary>
        /// 临时字典，用于初始化节点图的，这种玩意用完最好记得把内存释放掉哈
        /// </summary>
        private Dictionary<DialogNodeDataBase, NodeViewBase> NodeDirt;

        /// <summary>
        /// 重置节点图
        /// </summary>
        public void ResetNodeView()
        {
            if (treeData != null)
            {
                //初始化字典
                NodeDirt = new Dictionary<DialogNodeDataBase, NodeViewBase>();
                var nodeData = treeData.ChildNodeDataList;

                //检查StartNode是否存在
                if (treeData.StartNodeData == null)
                {
                    CreateNode(NodeType.Start);
                }
                else
                {
                    RecoveryNode(treeData.StartNodeData);
                }

                //恢复节点
                foreach (var node in nodeData)
                {
                    RecoveryNode(node);
                }

                //恢复节点边
                RecoveryEdge(treeData.StartNodeData);
                foreach (var node in nodeData)
                {
                    RecoveryEdge(node);
                }

                //清除字典
                NodeDirt.Clear();
            }
        }

        /// <summary>
        /// 恢复节点
        /// </summary>
        /// <param name="DialogNodeData"></param>
        private void RecoveryNode(DialogNodeDataBase DialogNodeData)
        {
            if (DialogNodeData == null)
            {
                return;
            }

            NodeViewBase nodeView = null;
            //恢复节点的核心部分，新增的节点需要在这里进行恢复方式的添加
            switch (DialogNodeData.NodeType)
            {
                case NodeType.Start:
                {
                    nodeView = new StartNodeView(DialogNodeData);
                    break;
                }
                case NodeType.RandomDialogNode:
                {
                    nodeView = new RandomDialogNodeView(DialogNodeData);
                    break;
                }
                case NodeType.SequentialDialogNode:
                {
                    nodeView = new SequentialDialogNodeView(DialogNodeData);
                    break;
                }
                case NodeType.SelectDialogNode:
                {
                    nodeView = new SelectDialogNodeView(DialogNodeData);
                    break;
                }
                case NodeType.EventNode:
                {
                    nodeView = new EventNodeView(DialogNodeData);
                    break;
                }
                case NodeType.End:
                {
                    nodeView = new EndNodeView(DialogNodeData);
                    break;
                }
                case NodeType.CharacterSwitchNode:
                {
                    nodeView = new CharacterSwitchNodeView(DialogNodeData);
                    break;
                }
                default:
                {
                    Debug.LogError("未找到该类型的节点");
                    break;
                }
            }

            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.SetPosition(new Rect(DialogNodeData.Position, nodeView.GetPosition().size));
            NodeDirt.Add(DialogNodeData, nodeView);

            this.AddElement(nodeView);
        }

        /// <summary>
        /// 恢复节点连线
        /// </summary>
        private void RecoveryEdge(DialogNodeDataBase DialogNodeData)
        {
            if (DialogNodeData.ChildNode == null)
            {
                return;
            }

            for (int i = 0; i < DialogNodeData.ChildNode.Count; i++)
            {
                //没连就跳过，找屁呢
                if (DialogNodeData.ChildNode[i] == null)
                {
                    continue;
                }

                Port _output = NodeDirt[DialogNodeData].outputContainer[i].Q<Port>();
                Port _input = NodeDirt[DialogNodeData.ChildNode[i]].inputContainer[0].Q<Port>();

                AddEdgeByPorts(_output, _input);
            }
        }
    }
}
#endif