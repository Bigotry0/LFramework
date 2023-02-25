using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace LFramework.Kit.DialogueSystem
{
    public abstract class DialogNodeDataBase : ScriptableObject
    {
        /// <summary>
        /// 节点坐标
        /// </summary>
        [HideInInspector] public Vector2 Position = Vector2.zero;

        [HideInInspector] public string Path;

        /// <summary>
        /// 节点类型
        /// </summary>
        public abstract NodeType NodeType { get; }

        private void OnValidate()
        {
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
#endif
        }

        public List<string> OutputItems = new List<string>();

        public List<DialogNodeDataBase> ChildNode = new List<DialogNodeDataBase>();
    }
}