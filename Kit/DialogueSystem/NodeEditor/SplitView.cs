#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace LFramework.Kit.DialogueSystem
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits>
        {
        }
    }
}
#endif