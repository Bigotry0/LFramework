using System;
using System.Collections.Generic;
using LFramework.Kit.DialogueSystem;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LFramework.Kit.DialogueSystem
{
    [CreateAssetMenu(menuName = "Create new DialogTreeData",fileName = "DialogTreeData")]
    public class DialogTree : ScriptableObject
    {
        public DialogNodeDataBase StartNodeData = null;

        public List<DialogNodeDataBase> ChildNodeDataList = new List<DialogNodeDataBase>();

        [Serializable]
        public class ViewData
        {
            public Vector3 Position;
            public Vector3 Scale = new Vector3(1, 1, 1);
        }

        public ViewData GraphViewData = new ViewData();
    }
}

#if UNITY_EDITOR
/// <summary>
/// 处理资源文件的删除
/// </summary>
public class DeleteNodeDataBeforeAssetRemoved : UnityEditor.AssetModificationProcessor
{
    private static void DeleteNodeData(List<DialogNodeDataBase> nodeDataList)
    {
        foreach (var childNode in nodeDataList)
        {
            AssetDatabase.DeleteAsset(childNode.Path);
        }
    }

    public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
    {
        if (AssetDatabase.LoadAssetAtPath<DialogTree>(assetPath) is DialogTree tree)
        {
            //要是编辑的时候哪个傻逼把资源文件给删了咱就直接把编辑器给关了
            DialogGraphWindow.CloseEditorWindow();

            if (tree.StartNodeData != null)
            {
                //删除各节点文件
                tree.ChildNodeDataList.Add(tree.StartNodeData);
                //不能直接在OnWillDeleteAsset里面调用AssetDatabase的任何API，否则会出错
                DeleteNodeData(tree.ChildNodeDataList);
            }
        }

        //告诉unity咱没对文件做删除处理，让他自己去删
        return AssetDeleteResult.DidNotDelete;
    }
}
#endif