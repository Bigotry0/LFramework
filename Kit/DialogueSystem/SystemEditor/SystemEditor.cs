#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace LFramework.Kit.DialogueSystem
{
    public class SystemEditor : EditorWindow
    {
        private ScrollView DataView;
        private DialogTreeRepository DialogTreeRepository;

        [MenuItem("Window/DialogueSystem/DialogTreeRepository")]
        public static void OpenManager()
        {
            SystemEditor wnd = GetWindow<SystemEditor>();
            wnd.titleContent = new GUIContent("DialogTreeRepository");
        }

        //确保目录存在
        private void MakeSureTheFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/DialogueData"))
            {
                AssetDatabase.CreateFolder("Assets", "DialogueData");
            }
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/LFramework/Kit/DialogueSystem/SystemEditor/EditorWindow/SystemEditor.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            DataView = rootVisualElement.Q<ScrollView>("DataView");

            InitUI();
        }

        private void InitUI()
        {
            if (this.DialogTreeRepository == null)
            {
                MakeSureTheFolder();

                DialogTreeRepository =
                    AssetDatabase.LoadAssetAtPath<DialogTreeRepository>(
                        "Assets/DialogueData/DialogTreeRepository.asset");
                if (DialogTreeRepository == null)
                {
                    AssetDatabase.CreateAsset(
                        ScriptableObject.CreateInstance(typeof(DialogTreeRepository)),
                        "Assets/DialogueData/DialogTreeRepository.asset");
                    DialogTreeRepository =
                        AssetDatabase.LoadAssetAtPath<DialogTreeRepository>(
                            "Assets/DialogueData/DialogTreeRepository.asset");
                }

                EditorUtility.SetDirty(DialogTreeRepository);
            }

            if (DialogTreeRepository.Keys.Count != DialogTreeRepository.Values.Count)
            {
                return;
            }

            for (var index = 0; index < DialogTreeRepository.Keys.Count; index++)
            {
                AddItem(index);
            }

            Button AddButton = rootVisualElement.Q<Button>("Add");
            AddButton.clicked += () =>
            {
                DialogTreeRepository.Keys.Add(null);
                DialogTreeRepository.Values.Add(null);
                AddItem(DialogTreeRepository.Keys.Count - 1);
            };
        }

        private void AddItem(int index)
        {
            TextField keyField = new TextField
            {
                name = index.ToString(),
                style = { minWidth = 120, maxWidth = 120, alignSelf = Align.Center }
            };
            keyField.SetValueWithoutNotify(DialogTreeRepository.Keys[index]);
            keyField.RegisterValueChangedCallback(evt =>
            {
                if (int.TryParse(keyField.name, out int i))
                {
                    DialogTreeRepository.Keys[int.Parse(keyField.name)] = evt.newValue;
                }
                else
                {
                    Debug.LogError("textField.name(string) to int fail");
                }
            });

            ObjectField valueField = new ObjectField
            {
                name = index.ToString(),
                objectType = typeof(DialogTree),
                style = { minWidth = 160, maxWidth = 160, alignSelf = Align.Center }
            };
            valueField.SetValueWithoutNotify(DialogTreeRepository.Values[index]);
            valueField.RegisterValueChangedCallback(evt =>
            {
                if (int.TryParse(valueField.name, out int i))
                {
                    DialogTreeRepository.Values[int.Parse(valueField.name)] = evt.newValue as DialogTree;
                }
                else
                {
                    Debug.LogError("ObjectField.name(string) to int fail");
                }
            });

            GroupBox FieldContainer = new GroupBox
            {
                name = "FieldContainer",
                style = { flexDirection = FlexDirection.Row, alignSelf = Align.Center },
            };
            FieldContainer.Add(keyField);
            FieldContainer.Add(valueField);

            VisualElement Container = new VisualElement()
            {
                name = index.ToString(),
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignSelf = Align.Center
                }
            };

            Button DelButton = new Button()
            {
                name = index.ToString(),
                style = { flexWrap = Wrap.Wrap, minWidth = 60, maxHeight = 25, alignSelf = Align.Center },
                text = "Del",
            };
            DelButton.clicked += () => { DelButtonOnClick(DelButton, Container); };

            Label keyLabel = new Label("Key: ")
            {
                style = { alignSelf = Align.Center }
            };
            Container.Add(keyLabel);
            Container.Add(FieldContainer);
            Container.Add(DelButton);
            // Label label = new Label(index.ToString());
            // Container.Add(label);

            DataView.Add(Container);
        }

        private void DelButtonOnClick(Button delButton, VisualElement container)
        {
            DialogTreeRepository.Keys.Remove(DialogTreeRepository.Keys[int.Parse(delButton.name)]);
            DialogTreeRepository.Values.Remove(DialogTreeRepository.Values[int.Parse(delButton.name)]);

            DataView.contentContainer.Remove(container);
            for (int index = int.Parse(delButton.name); index < DataView.contentContainer.childCount; index++)
            {
                DataView.contentContainer[index].Q<GroupBox>("FieldContainer").Q<TextField>($"{index + 1}").name =
                    index.ToString();
                DataView.contentContainer[index].Q<GroupBox>("FieldContainer").Q<ObjectField>($"{index + 1}").name =
                    index.ToString();
                DataView.contentContainer[index].Q<Button>($"{index + 1}").name = index.ToString();
            }
        }
    }
}
#endif