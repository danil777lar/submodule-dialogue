using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Larje.Dialogue.Editor
{
    public abstract class GraphNode : Node
    {
        public const int DEFAULT_NODE_WIDTH = 200;
        public const int DEFAULT_NODE_HEIGHT = 150;
        
        public string GUID;

        public event Action<Port> EventRemovePort;
        
        public abstract string DefaultName { get; }

        public virtual GraphNode Initialize(Vector2 position)
        {
            title = DefaultName;
            GUID = Guid.NewGuid().ToString();
            
            SetPosition(new Rect(position, new Vector2(DEFAULT_NODE_WIDTH, DEFAULT_NODE_HEIGHT)));
            
            return this;
        }
        
        public void AddChoicePort(string overriddenPortName = "")
        {
            Port generatedPort = InstantiatePort(Orientation.Horizontal, Direction.Output, 
                Port.Capacity.Multi, typeof(float));
            
            Label portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(portLabel);

            int outputPortCount = outputContainer.Query("connector").ToList().Count();
            string outputPortName = string.IsNullOrEmpty(overriddenPortName)
                ? $"Option {outputPortCount + 1}"
                : overriddenPortName;

            TextField textField = new TextField()
            {
                name = string.Empty,
                value = outputPortName
            };
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(new Label("  "));
            generatedPort.contentContainer.Add(textField);
            Button deleteButton = new Button(() => RemovePort(generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
            generatedPort.portName = outputPortName;
            outputContainer.Add(generatedPort);
            RefreshPorts();
            RefreshExpandedState();
        }
        
        private void RemovePort(Port port)
        {
            EventRemovePort?.Invoke(port);  
            outputContainer.Remove(port);
            RefreshPorts();
            RefreshExpandedState();
        }
    }
}