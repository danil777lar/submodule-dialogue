using System;
using System.Linq;
using UnityEngine.Serialization;

namespace Larje.Dialogue.Runtime.Graph.Data
{
    [Serializable]
    public class LinkData
    {
        public string PortName;
        public string FromGUID;
        public string ToGUID;
    }
}