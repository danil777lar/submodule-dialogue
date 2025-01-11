using System;
using System.Collections.Generic;
using System.Linq;
using Larje.Dialogue.Runtime.Graph.Data;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Larje.Dialogue.Runtime.Graph
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Dialogue Graph", menuName = "Larje/Dialogue/Dialogue Graph")]
    public class DialogueGraphContainer : ScriptableObject
    {
        public List<LinkData> NodeLinks = new List<LinkData>();
        public List<NodeData> NodeData = new List<NodeData>();
    }
}