using System;
using System.Reflection;
using Larje.Dialogue.DataContainers;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(DialogueContainer))]
public class DialogueContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
    }
}
