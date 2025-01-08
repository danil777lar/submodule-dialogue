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
        public string Choices;
        public override string DefaultName => "Dialogue Step";
        protected override string StyleSheetName => "DialogueGraphNode";

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

            if (!string.IsNullOrEmpty(Choices))
            {
                string[] choiceData = Choices.Split('/');
                for (int i = 0; i < choiceData.Length; i++)
                {
                    AddChoice(choiceData[i], false);
                }
            }

            return this;
        }
        
        private void DrawUI()
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portColor = Color.white;
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

            Button button = new Button(() => AddChoice());
            button.text = "Add Choice";
            titleButtonContainer.Add(button);
        }

        private void AddChoice(string choice = "", bool updateChoiceData = true)
        {
            Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            port.portColor = Color.white;
            
            Label portLabel = port.contentContainer.Q<Label>("type");
            port.contentContainer.Remove(portLabel);
            port.contentContainer.Add(new Label("  "));

            int outputPortCount = outputContainer.Query("connector").ToList().Count();
            string outputPortName = string.IsNullOrEmpty(choice) ? $"Option {outputPortCount + 1}" : choice;
            port.portName = outputPortName;
            
            TextField textField = new TextField();
            textField.name = string.Empty;
            textField.value = outputPortName;
            textField.RegisterValueChangedCallback(evt =>
            {
                port.portName = evt.newValue;
                UpdateChoiceData();
            });
            port.contentContainer.Add(textField);
            
            Button deleteButton = new Button(() =>
            {
                RemovePort(port);
                UpdateChoiceData();
            });
            deleteButton.text = "X";
            port.contentContainer.Add(deleteButton);
            
            outputContainer.Add(port);

            if (updateChoiceData)
            {
                UpdateChoiceData();
            }

            RefreshPorts();
            RefreshExpandedState();
        }

        private void UpdateChoiceData()
        {
            Choices = string.Empty;
            List<Port> ports = outputContainer.Query<Port>().ToList();
            foreach (Port port in ports)
            {
                if (port.direction == Direction.Output)
                {
                    Choices += $"{port.portName}";
                    if (port != ports.Last())
                    {
                        Choices += "/";
                    }
                }
            }
        }
    }
}