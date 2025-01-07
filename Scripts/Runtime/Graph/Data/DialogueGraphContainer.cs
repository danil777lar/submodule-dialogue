using System;
using System.Collections.Generic;
using Larje.Dialogue.Runtime.Graph.Data;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Larje.Dialogue.Runtime.Graph
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Dialogue Graph", menuName = "Larje/Dialogue/Dialogue Graph")]
    public class DialogueGraphContainer : ScriptableObject
    {
        public List<LinkData> NodeLinks = new List<LinkData>();
        public List<NodeData> NodeData = new List<NodeData>();

        public Runtime.Converted.Dialogue GetDialogue()
        {
            Runtime.Converted.Dialogue dialogue = new Runtime.Converted.Dialogue();
            dialogue.Steps = new List<Runtime.Converted.Dialogue.Step>();

            foreach (NodeData nodeData in NodeData)
            {
                Runtime.Converted.Dialogue.Step step = new Runtime.Converted.Dialogue.Step();
                step.Id = GuidToId(nodeData.GetField<string>("GUID"));

                step.Choices = new List<Runtime.Converted.Dialogue.Choice>();
                foreach (LinkData nodeLink in NodeLinks)
                {
                    if (nodeLink.FromGUID == nodeData.GetField<string>("GUID"))
                    {
                        Runtime.Converted.Dialogue.Choice choice = new Runtime.Converted.Dialogue.Choice();
                        choice.Text = nodeLink.FromPortName;
                        choice.NextStepId = GuidToId(nodeLink.ToGUID);
                        step.Choices.Add(choice);
                    }
                }

                dialogue.Steps.Add(step);
            }

            return dialogue;
        }
        
        private string GuidToId(string guid)
        {
            foreach (NodeData nodeData in NodeData)
            {
                if (nodeData.GetField<string>("GUID") == guid)
                {
                    return NodeData.IndexOf(nodeData).ToString();
                }
            }

            return null;
        }
    }
}