using System;
using System.Linq;
using UnityEngine.Serialization;

namespace Larje.Dialogue.Runtime.Graph
{
    [Serializable]
    public class DialogueNodeLinkData
    {
        public string PortName;
        public string FromGUID;
        public string ToGUID;
    }
}