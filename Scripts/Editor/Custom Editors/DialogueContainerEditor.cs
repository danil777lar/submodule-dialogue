using System;
using System.Reflection;
using Larje.Dialogue.Runtime.Graph;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(DialogueGraphContainer))]
public class DialogueContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
    }
}
