using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Larje.Dialogue.Runtime.Graph
{
    [Serializable]
    public class DialogueNodeData
    {
        public string GUID;
        public string Text;
        public Vector2 Position;
    }
}