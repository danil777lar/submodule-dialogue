using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Larje.Dialogue.Editor
{
    public class GraphNode : Node
    {
        public const int DEFAULT_NODE_WIDTH = 200;
        public const int DEFAULT_NODE_HEIGHT = 150;
        
        public string GUID;

        public GraphNode Initialize(string nodeName, Vector2 position)
        {
            DialogueGraphNode nodeInstance = new DialogueGraphNode()
            {
                title = nodeName,
                DialogueText = nodeName,
                GUID = Guid.NewGuid().ToString()
            };
            
            nodeInstance.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            
            inputPort.portName = "Input";
            nodeInstance.inputContainer.Add(inputPort);
            nodeInstance.RefreshExpandedState();
            nodeInstance.RefreshPorts();
            nodeInstance.SetPosition(new Rect(position, new Vector2(DEFAULT_NODE_WIDTH, DEFAULT_NODE_HEIGHT)));

            TextField textField = new TextField("");
            textField.RegisterValueChangedCallback(evt =>
            {
                nodeInstance.DialogueText = evt.newValue;
                nodeInstance.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(nodeInstance.title);
            nodeInstance.mainContainer.Add(textField);

            Button button = new Button(() => { AddChoicePort(); })
            {
                text = "Add Choice"
            };
            nodeInstance.titleButtonContainer.Add(button);
            return nodeInstance;
        }
        
        public void AddChoicePort(string overriddenPortName = "")
        {
            var generatedPort = InstantiatePort(Orientation.Horizontal, Direction.Output, 
                Port.Capacity.Multi, typeof(float));
            
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(portLabel);

            var outputPortCount = outputContainer.Query("connector").ToList().Count();
            var outputPortName = string.IsNullOrEmpty(overriddenPortName)
                ? $"Option {outputPortCount + 1}"
                : overriddenPortName;


            var textField = new TextField()
            {
                name = string.Empty,
                value = outputPortName
            };
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(new Label("  "));
            generatedPort.contentContainer.Add(textField);
            var deleteButton = new Button(() => RemovePort(generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
            generatedPort.portName = outputPortName;
            outputContainer.Add(generatedPort);
            RefreshPorts();
            RefreshExpandedState();
        }
        
        private void RemovePort(Port socket)
        {
            var targetEdge = edges.ToList().Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            outputContainer.Remove(socket);
            RefreshPorts();
            RefreshExpandedState();
        }
    }
}