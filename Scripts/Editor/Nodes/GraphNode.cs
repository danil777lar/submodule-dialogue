using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Larje.Dialogue.Editor
{
    public abstract class GraphNode : Node
    {
        public const int DEFAULT_NODE_WIDTH = 200;
        public const int DEFAULT_NODE_HEIGHT = 150;
        
        public string GUID;

        public event Action<Port> EventRemovePort;
        
        public abstract string DefaultName { get; }

        public virtual GraphNode Initialize(Vector2 position, List<Node> allNodes)
        {
            title = DefaultName;
            GUID = Guid.NewGuid().ToString();
            
            SetPosition(new Rect(position, new Vector2(DEFAULT_NODE_WIDTH, DEFAULT_NODE_HEIGHT)));
            
            return this;
        }
        
        protected void RemovePort(Port port)
        {
            EventRemovePort?.Invoke(port);  
            outputContainer.Remove(port);
            RefreshPorts();
            RefreshExpandedState();
        }
    }
}