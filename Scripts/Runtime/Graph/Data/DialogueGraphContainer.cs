using System;
using System.Collections.Generic;
using System.Linq;
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

            NodeData firstStepNode = GetFirstStepNode();
            foreach (NodeData nodeData in NodeData)
            {
                if (nodeData.Type.Split(".").Last() == "DialogueGraphNode")
                {
                    Runtime.Converted.Dialogue.Step step = new Runtime.Converted.Dialogue.Step();
                    step.Id = GuidToId(nodeData.GetField<string>("GUID"));
                    step.Text = nodeData.GetField<string>("DialogueText");

                    if (nodeData == firstStepNode)
                    {
                        dialogue.StartStep = step;
                    }

                    step.Choices = new List<Runtime.Converted.Dialogue.Choice>();
                    foreach (LinkData nodeLink in NodeLinks)
                    {
                        if (nodeLink.FromGUID == nodeData.GetField<string>("GUID"))
                        {
                            Runtime.Converted.Dialogue.Choice choice = new Runtime.Converted.Dialogue.Choice();
                            choice.Text = nodeLink.FromPortName;
                            choice.NextStepId = GuidToId(CheckRedirection(nodeLink.ToGUID));
                            step.Choices.Add(choice);
                        }
                    }

                    dialogue.Steps.Add(step);
                }
            }

            return dialogue;
        }
        
        private NodeData GetFirstStepNode()
        {
            NodeData enter0 = NodeData.Find(x => 
                x.Type.Split(".").Last() == "EnterGraphNode" && 
                x.GetField<int>("EnterIndex") == 0);
            
            LinkData link = NodeLinks.Find(x => x.FromGUID == enter0.GetField<string>("GUID"));
            
            return NodeData.Find(x => x.GetField<string>("GUID") == link.ToGUID);
        }

        private string CheckRedirection(string guid)
        {
            NodeData node = NodeData.Find(x => x.GetField<string>("GUID") == guid);
            if (node.Type.Split(".").Last() == "ExitGraphNode")
            {
                int index = node.GetField<int>("Index");
                NodeData enterNode = NodeData.Find(x => 
                    x.Type.Split(".").Last() == "EnterGraphNode" && 
                    x.GetField<int>("ExitIndex") == index);
                
                LinkData link = NodeLinks.Find(x => x.FromGUID == enterNode.GetField<string>("GUID"));
                return link.ToGUID;
            }

            return guid;
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