#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LFramework.Kit.DialogueSystem
{
    /// <summary>
    /// 对话系统抽象类
    /// </summary>
    public abstract class DialogueSystem : MonoBehaviour
    {
        /// <summary>
        /// 对话系统状态
        /// </summary>
        public enum DialogueStates
        {
            /// <summary>
            /// 未开始
            /// </summary>
            NotStart,

            /// <summary>
            /// 对话中
            /// </summary>
            Started,

            /// <summary>
            /// 已完成播放
            /// </summary>
            Finished
        }

        /// <summary>
        /// 语句播放状态
        /// </summary>
        public enum PlayTextStates
        {
            /// <summary>
            /// 播放中
            /// </summary>
            IsPlayed,

            /// <summary>
            /// 播放完成
            /// </summary>
            Finished
        }

        /// <summary>
        /// 对话数据
        /// </summary>
        public DialogTree DialogTree;

        /// <summary>
        /// 当前对话节点
        /// </summary>
        private DialogNodeDataBase CurrentDialogNode;

        /// <summary>
        /// TreeData是否读取结束
        /// </summary>
        private bool IsLoadDialogTreeDataEnd = false;

        /// <summary>
        /// NotStart状态回调
        /// </summary>
        public UnityEvent OnNotStart = new UnityEvent();

        /// <summary>
        /// Started状态回调
        /// </summary>
        public UnityEvent OnDialogueStart = new UnityEvent();

        /// <summary>
        /// Finished状态回调
        /// </summary>
        public UnityEvent OnDialogueFinish = new UnityEvent();

        /// <summary>
        /// 用于设置文本输出目标
        /// </summary>
        public UnityEvent<string> OnPlayText = new UnityEvent<string>();

        /// <summary>
        /// SelectDialog事件开始时回调
        /// </summary>
        [HideInInspector] public UnityEvent<List<string>> SelectDialogInfo = new UnityEvent<List<string>>();

        /// <summary>
        /// 选择对话开始事件
        /// </summary>
        public UnityEvent OnSelectDialogStart = new UnityEvent();

        /// <summary>
        /// 角色切换事件
        /// </summary>
        public UnityEvent<string> OnCharacterSwitchEvent = new UnityEvent<string>();

        /// <summary>
        /// 选择对话结束事件
        /// </summary>
        public UnityEvent OnSelectDialogFinish = new UnityEvent();

        /// <summary>
        /// 对话事件传出口
        /// </summary>
        public UnityEvent<string> OnDialogEvent = new UnityEvent<string>();

        /// <summary>
        /// 对话数据队列
        /// </summary>
        public Queue<string> SentenceQueue;

        /// <summary>
        /// 对话系统当前状态
        /// </summary>
        private DialogueStates DialogueState = DialogueStates.NotStart;

        /// <summary>
        /// 当前语句播放状态
        /// </summary>
        protected PlayTextStates PlayTextState = PlayTextStates.Finished;

        /// <summary>
        /// 是否处于进行选择对话的状态
        /// </summary>
        private bool IsSelecting = false;

        /// <summary>
        /// 通过路径设置
        /// </summary>
        /// <param name="path">DialogTree对象相对路径</param>
        public void SetDialogTree(string path)
        {
            var dialogTree = AssetDatabase.LoadAssetAtPath<DialogTree>(path);
            //var dialogTree = Resources.Load(path);
            if (dialogTree == null)
            {
                Debug.LogError("Load DialogTree in path:" + path + " failed!");
                return;
            }

            if (typeof(DialogTree) == DialogTree.GetType())
            {
                DialogTree = dialogTree;
            }

            if (DialogueState != DialogueStates.Started)
            {
                InitDialogTreeData();
            }
        }

        /// <summary>
        /// 通过对象设置
        /// </summary>
        /// <param name="dialogTree">DialogTree对象</param>
        public void SetDialogTree(DialogTree dialogTree)
        {
            if (dialogTree == null)
            {
                Debug.LogError($"The DialogTree: {dialogTree} object is Null");
                return;
            }

            DialogTree = dialogTree;
            
            if (DialogueState != DialogueStates.Started)
            {
                InitDialogTreeData();
            }
        }

        /// <summary>
        /// 通过存储库Key设置
        /// </summary>
        /// <param name="key">键</param>
        public void SetDialogTreeByKey(string key)
        {
            var dialogTree = DialogTreeManager.Instance.GetDialogTree(key);
            
            if (dialogTree == null)
            {
                Debug.LogError($"The DialogTree: {dialogTree} object is Null");
                return;
            }

            DialogTree = dialogTree;
            
            if (DialogueState != DialogueStates.Started)
            {
                InitDialogTreeData();
            }
        }

        /// <summary>
        /// 对外输出语句
        /// </summary>
        /// <param name="text">文本</param>
        protected void OutputText(string text)
        {
            if (text == null)
            {
                return;
            }
            
            OnPlayText?.Invoke(text);
        }
        
        /// <summary>
        /// 初始化对话树数据
        /// </summary>
        private void InitDialogTreeData()
        {
            if (DialogTree.StartNodeData == null)
            {
                Debug.LogError("The Start node does not exist in the DialogTree file");
                return;
            }

            //StartNode只有一个接口
            CurrentDialogNode = DialogTree.StartNodeData.ChildNode[0];
            IsLoadDialogTreeDataEnd = false;
        }

        /// <summary>
        /// 加载当前对话节点
        /// </summary>
        private void LoadCurrentDialogNode()
        {
            if (IsLoadDialogTreeDataEnd)
            {
                return;
            }

            if (CurrentDialogNode == null)
            {
                Debug.LogError("The branch has ended but the EndNode is not connected");
                //保护措施
                IsLoadDialogTreeDataEnd = true;
                return;
            }

            switch (CurrentDialogNode.NodeType)
            {
                case NodeType.SequentialDialogNode:
                {
                    foreach (var output in CurrentDialogNode.OutputItems)
                    {
                        SentenceQueue.Enqueue(output);
                    }

                    CurrentDialogNode = CurrentDialogNode.ChildNode[0];
                    break;
                }
                case NodeType.RandomDialogNode:
                {
                    if (CurrentDialogNode.OutputItems.Count > 0)
                    {
                        SentenceQueue.Enqueue(
                            CurrentDialogNode.OutputItems[Random.Range(0, CurrentDialogNode.OutputItems.Count)]);
                    }

                    CurrentDialogNode = CurrentDialogNode.ChildNode[0];
                    break;
                }
                case NodeType.SelectDialogNode:
                {
                    IsSelecting = true;
                    StartSelectDialog();
                    break;
                }
                case NodeType.EventNode:
                {
                    OnDialogEvent?.Invoke(CurrentDialogNode.OutputItems[0]);
                    CurrentDialogNode = CurrentDialogNode.ChildNode[0];
                    break;
                }
                case NodeType.End:
                {
                    IsLoadDialogTreeDataEnd = true;
                    break;
                }
                case NodeType.CharacterSwitchNode:
                {
                    OnCharacterSwitchEvent?.Invoke(CurrentDialogNode.OutputItems[0]);
                    CurrentDialogNode = CurrentDialogNode.ChildNode[0];
                    break;
                }
                case NodeType.SwitchDialogTreeNode:
                {
                    SetDialogTreeByKey(CurrentDialogNode.OutputItems[0]);
                    InitDialogTreeData();
                    break;
                }
            }
        }


        /// <summary>
        /// 开启对话系统方法
        /// </summary>
        public void StartDialogue()
        {
            if (DialogueState == DialogueStates.NotStart)
            {
                if (DialogTree == null)
                {
                    Debug.LogError("The DialogTree file is missing");
                    return;
                }

                DialogueState = DialogueStates.Started;
                OnDialogueStart?.Invoke();

                //先播放第一句
                Next();
            }
        }

        /// <summary>
        /// 继续对话方法
        /// </summary>
        public void Next()
        {
            if (DialogueState != DialogueStates.Started)
            {
                return;
            }

            if (IsSelecting)
            {
                return;
            }

            if (SentenceQueue.Count > 0)
            {
                switch (PlayTextState)
                {
                    case PlayTextStates.Finished:
                    {
                        PlayText(SentenceQueue.Dequeue());
                        break;
                    }

                    case PlayTextStates.IsPlayed:
                    {
                        NextOnTextIsPlayed();
                        break;
                    }
                }
            }
            else
            {
                LoadCurrentDialogNode();
                
                if (IsLoadDialogTreeDataEnd)
                {
                    DialogueState = DialogueStates.Finished;
                    OnDialogueFinish?.Invoke();
                }
                else
                {
                    //递归
                    Next();
                }
            }
        }

        /// <summary>
        /// 重置对话方法
        /// </summary>
        public void Reset()
        {
            SentenceQueue = new Queue<string>();
            DialogueState = DialogueStates.NotStart;
            IsSelecting = false;
            IsLoadDialogTreeDataEnd = false;

            OnNotStart?.Invoke();

            InitDialogTreeData();
        }

        /// <summary>
        /// 开始选择对话
        /// </summary>
        private void StartSelectDialog()
        {
            if (!IsSelecting)
            {
                return;
            }

            if (CurrentDialogNode.NodeType != NodeType.SelectDialogNode)
            {
                IsSelecting = false;
                return;
            }

            if (CurrentDialogNode.ChildNode.Count < 1)
            {
                IsSelecting = false;
                Debug.LogError("The SelectDialogNode is not connected to any child nodes");
                return;
            }

            OnSelectDialogStart?.Invoke();
            //发送事件，传出选项数据
            SelectDialogInfo?.Invoke(CurrentDialogNode.OutputItems);
        }

        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="resultIndex">选项下标（下标从0开始）</param>
        public void SelectResult(int resultIndex)
        {
            if (!IsSelecting)
            {
                return;
            }

            if (resultIndex >= CurrentDialogNode.OutputItems.Count)
            {
                Debug.LogError("The index is out of the range of the child node list");
                return;
            }

            //选择答案所选节点
            CurrentDialogNode = CurrentDialogNode.ChildNode[resultIndex];

            IsSelecting = false;
            OnSelectDialogFinish?.Invoke();
        }

        /// <summary>
        /// 播放对话语句方法，该方法在子类中实现，可自定义语句打印效果
        /// 子类必须在打印语句时将PlayTextState设置为IsPlayed状态，打印完成时必须将其设置为Finished状态
        /// </summary>
        /// <param name="sentence">当前对话语句</param>
        protected abstract void PlayText(string sentence);

        /// <summary>
        /// 状态监听方法，该方法在子类中实现，该方法在PlayTextState为IsPlayed时尝试继续对话时被调用，可在子类中监听并进行处理
        /// </summary>
        protected abstract void NextOnTextIsPlayed();

        void Awake()
        {
            //初始化
            Reset();
        }
    }
}