using System;
using UnityEngine;

namespace Larje.Dialogue.DataContainers
{
    [Serializable]
    public class DialogueNodeData
    {
        public string NodeGUID;
        public string DialogueText;
        public Vector2 Position;
    }
}