﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Larje.Dialogue.Runtime.Graph.Data;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Larje.Dialogue.Editor
{
    [Serializable]
    public class DialogueGraphNode : GraphNode
    {
        [JsonRequired] public DialogueContent Content; 
            
        public override string DefaultName => "Dialogue Step";
        protected override string StyleSheetName => "DialogueGraphNode";

        public override GraphNode Initialize(Vector2 position, List<Node> allNodes)
        {
            base.Initialize(position, allNodes);

            Content = new DialogueContent();
            Content.Speeches = new List<Speech>();
            Content.Speeches.Add(new Speech()
            {
                LanguageCode = "en",
                Title = "Title",
                Text = "Text"
            });
            
            DrawUI();
            return this;
        }

        public override GraphNode Load(Vector2 position)
        {
            base.Load(position);
            DrawUI();

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

            Button button = new Button(() => DialogueGraphNodeWindow.OpenWindow(this));
            button.text = "Edit";
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
            
            Label label = new Label(outputPortName);
            port.contentContainer.Add(label);
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
            /*Choices = string.Empty;
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
            }*/
        }

        [Serializable]
        public class DialogueContent
        {
            public List<Speech> Speeches;
        }
        
        [Serializable]
        public class Speech
        {
            public string LanguageCode;
            public string Title;
            public string Text;
        }
    }
}