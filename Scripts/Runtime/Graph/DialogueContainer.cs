using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Larje.Dialogue.Runtime.Graph
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Dialogue Graph", menuName = "Larje/Dialogue/Dialogue Graph")]
    public class DialogueContainer : ScriptableObject
    {
        public List<DialogueNodeLinkData> NodeLinks = new List<DialogueNodeLinkData>();
        public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
        public List<CommentBlockData> CommentBlockData = new List<CommentBlockData>();

        public Runtime.Converted.Dialogue GetDialogue()
        {
            Runtime.Converted.Dialogue dialogue = new Runtime.Converted.Dialogue();
            dialogue.Steps = new List<Runtime.Converted.Dialogue.Step>();

            foreach (DialogueNodeData nodeData in DialogueNodeData)
            {
                Runtime.Converted.Dialogue.Step step = new Runtime.Converted.Dialogue.Step();
                step.Id = GuidToId(nodeData.GUID);
                step.Text = nodeData.Text;

                step.Choices = new List<Runtime.Converted.Dialogue.Choice>();
                foreach (DialogueNodeLinkData nodeLink in NodeLinks)
                {
                    if (nodeLink.FromGUID == nodeData.GUID)
                    {
                        Runtime.Converted.Dialogue.Choice choice = new Runtime.Converted.Dialogue.Choice();
                        choice.Text = nodeLink.PortName;
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
            foreach (DialogueNodeData nodeData in DialogueNodeData)
            {
                if (nodeData.GUID == guid)
                {
                    return DialogueNodeData.IndexOf(nodeData).ToString();
                }
            }

            return null;
        }
    }
}