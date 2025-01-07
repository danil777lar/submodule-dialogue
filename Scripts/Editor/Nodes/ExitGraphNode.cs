using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Larje.Dialogue.Editor
{
    public class ExitGraphNode : GraphNode
    {
        public int ExitIndex = 0;
        public override string DefaultName => "Exit";
        
        public override GraphNode Initialize(Vector2 position, List<Node> allNodes)
        {
            base.Initialize(position, allNodes);
            
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "In";
            inputContainer.Add(inputPort);
            
            title = $"Exit {ExitIndex}";
            IntegerField indexField = new IntegerField(10);
            indexField.RegisterValueChangedCallback(evt =>
            {
                ExitIndex = evt.newValue;
                title = $"Exit {ExitIndex}";
            });
            indexField.SetValueWithoutNotify(ExitIndex);
            mainContainer.Add(indexField);

            return this;
        }
    }
}