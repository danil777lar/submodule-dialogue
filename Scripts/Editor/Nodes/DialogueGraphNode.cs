using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Larje.Dialogue.Editor
{
    public class DialogueGraphNode : GraphNode
    {
        public string DialogueText;
        public override string DefaultName => "Dialogue Step";

        public override GraphNode Initialize(Vector2 position)
        {
            base.Initialize(position);
            
            DialogueText = DefaultName;
            
            styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);
            RefreshExpandedState();
            RefreshPorts();

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

            return this;
        }
    }
}