﻿using System;
using System.Collections.Generic;
using System.Linq;
using Larje.Dialogue.Runtime.Converted;
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
        private const string TYPE_DIALOGUE = "DialogueGraphNode";
        private const string TYPE_ENTER = "EnterGraphNode";
        private const string TYPE_EXIT = "ExitGraphNode";
        private const string TYPE_EVENT = "EventGraphNode";
        private const string TYPE_CONDITION = "ConditionGraphNode";
        
        public List<LinkData> Links = new List<LinkData>();
        public List<NodeData> Nodes = new List<NodeData>();
        
        public DialogueStep GetFirstStep(Action<string> sendEvent, Func<string, bool> checkCondition)
        {
            NodeData enter0 = Nodes.Find(x => 
                x.IsTypeOf(TYPE_ENTER) && x.GetField<int>("EnterIndex") == 0);
            
            LinkData link = Links.Find(x => x.FromGUID == enter0.GetField<string>("GUID"));
            
            string nextNodeGuid = FindNextStep(link.ToGUID, sendEvent, checkCondition);
            NodeData nextNode = GuidToNode(nextNodeGuid);
            
            return NodeToStep(nextNode);
        }
        
        public DialogueStep GetNextStep(string id, string choiceText, Action<string> sendEvent, Func<string, bool> checkCondition)
        {
            NodeData node = Nodes[int.Parse(id)];
            LinkData link = Links.Find(x => 
                x.FromGUID == node.GetField<string>("GUID") && x.FromPortName == choiceText);
            
            string nextNodeGuid = FindNextStep(link.ToGUID, sendEvent, checkCondition);
            NodeData nextNode = GuidToNode(nextNodeGuid);
            
            return NodeToStep(nextNode);
        }

        private string FindNextStep(string guid, Action<string> sendEvent, Func<string, bool> checkCondition)
        {
            NodeData node = GuidToNode(guid);

            if (string.IsNullOrEmpty(guid))
            {
                return "";
            }
            
            if (node.IsTypeOf(TYPE_DIALOGUE))
            {
                return guid;
            }

            string next = "";
            
            if (node.IsTypeOf(TYPE_EVENT))
            {
                sendEvent?.Invoke(node.GetField<string>("EventName"));
                
                LinkData link = Links.Find(x => x.FromGUID == guid);
                next = link != null ? link.ToGUID : "";
            }
            
            if (node.IsTypeOf(TYPE_CONDITION))
            {
                LinkData linkTrue = Links.Find(x => x.FromGUID == guid && x.FromPortName == "True");
                LinkData linkFalse = Links.Find(x => x.FromGUID == guid && x.FromPortName == "False");

                string conditionName = node.GetField<string>("ConditionName");
                next = checkCondition.Invoke(conditionName) ? linkTrue.ToGUID : linkFalse.ToGUID; 
            }
            
            if (node.IsTypeOf(TYPE_EXIT))
            {
                int exitIndex = node.GetField<int>("ExitIndex");
                NodeData enterNode = Nodes.Find(x => 
                    x.IsTypeOf(TYPE_ENTER) && x.GetField<int>("EnterIndex") == exitIndex);

                next = enterNode != null ? enterNode.GetField<string>("GUID") : ""; 
            }
            
            if (node.IsTypeOf(TYPE_ENTER))
            {
                LinkData link = Links.Find(x => x.FromGUID == guid);
                next = link != null ? link.ToGUID : ""; 
            }
            
            return FindNextStep(next, sendEvent, checkCondition);
        }
        
        private string GuidToId(string guid)
        {
            foreach (NodeData nodeData in Nodes)
            {
                if (nodeData.GetField<string>("GUID") == guid)
                {
                    return Nodes.IndexOf(nodeData).ToString();
                }
            }

            return null;
        }
        
        private NodeData GuidToNode(string guid)
        {
            return Nodes.Find(x => x.GetField<string>("GUID") == guid);
        }

        private DialogueStep NodeToStep(NodeData node)
        {
            if (node != null && node.IsTypeOf(TYPE_DIALOGUE))
            {
                DialogueStep step = new DialogueStep();
                step.Id = GuidToId(node.GetField<string>("GUID"));
                step.Text = node.GetField<string>("DialogueText");

                step.Choices = new List<DialogueStep.Choice>();
                foreach (LinkData nodeLink in Links)
                {
                    if (nodeLink.FromGUID == node.GetField<string>("GUID"))
                    {
                        DialogueStep.Choice choice = new DialogueStep.Choice
                        {
                            Text = nodeLink.FromPortName
                        };
                        step.Choices.Add(choice);
                    }
                }

                return step;
            }

            return null;
        }
    }
}