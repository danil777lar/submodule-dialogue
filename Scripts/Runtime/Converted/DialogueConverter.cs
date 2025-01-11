using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Larje.Dialogue.Runtime.Graph.Data;
using UnityEngine;

namespace Larje.Dialogue.Runtime.Converted
{
    public static class DialogueConverter
    {
        public const string TYPE_DIALOGUE = "DialogueGraphNode";
        public const string TYPE_ENTER = "EnterGraphNode";
        public const string TYPE_EXIT = "ExitGraphNode";
        public const string TYPE_EVENT = "EventGraphNode";
        public const string TYPE_CONDITION = "ConditionGraphNode";
        
        public static Dialogue GetDialogue(List<NodeData> nodes, List<LinkData> links)
        {
            Dialogue dialogue = new Dialogue();
            dialogue.Steps = new List<Dialogue.Step>();

            NodeData firstStepNode = GetFirstStepNode(nodes, links);
            foreach (NodeData nodeData in nodes)
            {
                if (nodeData.IsTypeOf(TYPE_DIALOGUE))
                {
                    Dialogue.Step step = new Dialogue.Step();
                    step.Id = GuidToId(nodeData.GetField<string>("GUID"), nodes);
                    step.Text = nodeData.GetField<string>("DialogueText");

                    if (nodeData == firstStepNode)
                    {
                        dialogue.StartStep = step;
                    }

                    step.Choices = new List<Dialogue.Choice>();
                    foreach (LinkData nodeLink in links)
                    {
                        if (nodeLink.FromGUID == nodeData.GetField<string>("GUID"))
                        {
                            Dialogue.Choice choice = new Dialogue.Choice();
                            choice.Text = nodeLink.FromPortName;
                            choice.NextStepId = GuidToId(CheckRedirection(nodeLink.ToGUID, nodes, links), nodes);
                            step.Choices.Add(choice);
                        }
                    }

                    dialogue.Steps.Add(step);
                }
            }

            return dialogue;
        }
        
        private static NodeData GetFirstStepNode(List<NodeData> nodes, List<LinkData> links)
        {
            NodeData enter0 = nodes.Find(x => 
                x.IsTypeOf(TYPE_ENTER) && x.GetField<int>("EnterIndex") == 0);
            
            LinkData link = links.Find(x => x.FromGUID == enter0.GetField<string>("GUID"));
            
            return nodes.Find(x => x.GetField<string>("GUID") == link.ToGUID);
        }

        private static string CheckRedirection(string guid, List<NodeData> nodes, List<LinkData> links)
        {
            NodeData node = nodes.Find(x => x.GetField<string>("GUID") == guid);
            
            if (node.IsTypeOf(TYPE_EXIT))
            {
                int index = node.GetField<int>("Index");
                NodeData enterNode = nodes.Find(x => 
                    x.IsTypeOf(TYPE_ENTER) && x.GetField<int>("ExitIndex") == index);
                
                LinkData link = links.Find(x => x.FromGUID == enterNode.GetField<string>("GUID"));
                return link.ToGUID;
            }

            return guid;
        }
        
        private static string GuidToId(string guid, List<NodeData> nodes)
        {
            foreach (NodeData nodeData in nodes)
            {
                if (nodeData.GetField<string>("GUID") == guid)
                {
                    return nodes.IndexOf(nodeData).ToString();
                }
            }

            return null;
        }
    }
}