#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace LFramework.Kit.DialogueSystem
{
    public class DialogTreeManager
    {
        private static DialogTreeManager _Instance;

        public static DialogTreeManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DialogTreeManager();
                }

                return _Instance;
            }
        }

        private Dictionary<string, DialogTree> AllDialogTree = new Dictionary<string, DialogTree>();

        private DialogTreeManager()
        {
            var dialogTreeRepository =
                AssetDatabase.LoadAssetAtPath<DialogTreeRepository>("Assets/DialogueData/DialogTreeRepository.asset");
            if (dialogTreeRepository == null)
            {
                return;
            }

            if (dialogTreeRepository.Keys.Count != dialogTreeRepository.Values.Count)
            {
                Debug.LogError(
                    "The number of keys in the DialogTreeRepository is not equal to the number of values");
                return;
            }

            for (var i = 0; i < dialogTreeRepository.Keys.Count; i++)
            {
                if (AllDialogTree.ContainsKey(dialogTreeRepository.Keys[i]))
                {
                    Debug.LogError(
                        $"The key:{dialogTreeRepository.Keys[i]} with the same name already exists in the DialogTreeRepository");
                    return;
                }

                if (AllDialogTree.ContainsValue(dialogTreeRepository.Values[i]))
                {
                    Debug.LogError(
                        $"The DialogTree:{dialogTreeRepository.Values[i]} with the same name already exists in the DialogTreeRepository");
                    return;
                }

                AllDialogTree.Add(dialogTreeRepository.Keys[i], dialogTreeRepository.Values[i]);
            }
        }

        /// <summary>
        /// 获取DialogTree方法
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public DialogTree GetDialogTree(string key)
        {
            if (AllDialogTree.TryGetValue(key, out DialogTree dialogTree))
            {
                return dialogTree;
            }

            Debug.LogError($"Key value:{key} has no corresponding value in the repository");
            return null;
        }
    }
}