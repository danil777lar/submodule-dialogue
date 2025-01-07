using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Larje.Dialogue.Runtime.Graph.Data;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Larje.Dialogue.Editor
{
    [Serializable]
    public class DialogueGraphNode : GraphNode
    {
        public string DialogueText;
        public override string DefaultName => "Dialogue Step";

        public override GraphNode Initialize(Vector2 position, List<Node> allNodes)
        {
            base.Initialize(position, allNodes);
            DialogueText = DefaultName;
            DrawUI();
            return this;
        }
        
        public override GraphNode Load(Vector2 position)
        {
            base.Load(position);
            DrawUI();
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

        private void DrawUI()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);
            
            RefreshExpandedState();
            RefreshPorts();

            title = DialogueText;
            TextField textField = new TextField("");
            textField.RegisterValueChangedCallback(evt =>
            {
                DialogueText = evt.newValue;
                title = evt.newValue;
            });
            textField.SetValueWithoutNotify(title);
            mainContainer.Add(textField);

            Button button = new Button(() => { AddChoicePort(); })
            {
                text = "Add Choice"
            };
            titleButtonContainer.Add(button);
        }
    }
}