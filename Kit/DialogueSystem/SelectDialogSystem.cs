using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LFramework.Kit.DialogueSystem
{
    public class SelectDialogSystem : MonoBehaviour
    {
        public DialogueSystem DialogueSystem;

        public GameObject ItemButton;

        private void OnEnable()
        {
            //清除旧选项
            DeleteOldItem();
        }

        private void Awake()
        {
            //监听选择对话信息
            DialogueSystem.SelectDialogInfo.AddListener(Select);
        }

        /// <summary>
        /// 显示选择菜单
        /// </summary>
        /// <param name="selectItem"></param>
        private void Select(List<string> selectItem)
        {
            int currentIndex = 0;
            foreach (var item in selectItem)
            {
                var itemButton = Instantiate(ItemButton);
                itemButton.GetComponentInChildren<Text>().text = item;
                itemButton.name = currentIndex.ToString();

                itemButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    DialogueSystem.SelectResult(int.Parse(itemButton.name));
                    //在这里选完顺便进入下一句
                    DialogueSystem.Next();
                });

                itemButton.transform.SetParent(transform, false);
                currentIndex++;
            }
        }

        /// <summary>
        /// 删除物体下所有子物体
        /// </summary>
        void DeleteOldItem()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                //性能更好
                transform.GetChild(i).gameObject.SetActive(false);
                //这种看着干净，爽
                //Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}