using System.Collections.Generic;
using UnityEngine;

namespace LFramework.Kit.DialogueSystem
{
    public class DialogTreeRepository : ScriptableObject
    {
        [HideInInspector] public List<string> Keys = new List<string>();

        [HideInInspector] public List<DialogTree> Values = new List<DialogTree>();
    }
}