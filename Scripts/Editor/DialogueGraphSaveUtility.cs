using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Larje.Dialogue.Runtime.Graph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Larje.Dialogue.Editor
{
    public class DialogueGraphSaveUtility
    {
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<DialogueNode> Nodes => _graphView.nodes.ToList().Cast<DialogueNode>().ToList();
        private List<Group> CommentBlocks => _graphView.graphElements.ToList()
            .Where(x => x is Group).Cast<Group>().ToList();

        private DialogueContainer _dialogueContainer;
        private DialogueGraphView _graphView;

        public static DialogueGraphSaveUtility GetInstance(DialogueGraphView graphView)
        {
            return new DialogueGraphSaveUtility
            {
                _graphView = graphView
            };
        }

        public void SaveGraph(string assetPath)
        {
            DialogueContainer dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();
            if (!SaveNodes(dialogueContainerObject))
            {
                return;
            }
            
            SaveExposedProperties(dialogueContainerObject);
            SaveCommentBlocks(dialogueContainerObject);
            
            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DialogueContainer));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset)) 
			{
                AssetDatabase.CreateAsset(dialogueContainerObject, assetPath);
            }
            else 
			{
                DialogueContainer container = loadedAsset as DialogueContainer;
                container.NodeLinks = dialogueContainerObject.NodeLinks;
                container.DialogueNodeData = dialogueContainerObject.DialogueNodeData;
                container.ExposedProperties = dialogueContainerObject.ExposedProperties;
                container.CommentBlockData = dialogueContainerObject.CommentBlockData;
                EditorUtility.SetDirty(container);
            }

            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(DialogueContainer dialogueContainerObject)
        {
            if (!Edges.Any()) return false;
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var outputNode = (connectedSockets[i].output.node as DialogueNode);
                var inputNode = (connectedSockets[i].input.node as DialogueNode);
                dialogueContainerObject.NodeLinks.Add(new DialogueNodeLinkData
                {
                    FromGUID = outputNode.GUID,
                    PortName = connectedSockets[i].output.portName,
                    ToGUID = inputNode.GUID
                });
            }

            foreach (var node in Nodes.Where(node => !node.EntyPoint))
            {
                dialogueContainerObject.DialogueNodeData.Add(new DialogueNodeData
                {
                    GUID = node.GUID,
                    Text = node.DialogueText,
                    Position = node.GetPosition().position
                });
            }

            return true;
        }

        private void SaveExposedProperties(DialogueContainer dialogueContainer)
        {
            dialogueContainer.ExposedProperties.Clear();
            dialogueContainer.ExposedProperties.AddRange(_graphView.ExposedProperties);
        }

        private void SaveCommentBlocks(DialogueContainer dialogueContainer)
        {
            foreach (var block in CommentBlocks)
            {
                var nodes = block.containedElements.Where(x => x is DialogueNode).Cast<DialogueNode>().Select(x => x.GUID)
                    .ToList();

                dialogueContainer.CommentBlockData.Add(new CommentBlockData
                {
                    ChildNodes = nodes,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
        }

        public void LoadNarrative(string assetPath)
        {
            _dialogueContainer = AssetDatabase.LoadAssetAtPath<DialogueContainer>(assetPath);
            if (_dialogueContainer == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target Narrative Data does not exist!", "OK");
                return;
            }

            ClearGraph();
            GenerateDialogueNodes();
            ConnectDialogueNodes();
            AddExposedProperties();
            GenerateCommentBlocks();
        }
        
        private void ClearGraph()
        {
            if (_dialogueContainer.NodeLinks.Count <= 0)
            {
                return;
            }
                
            if (Nodes != null)
            {
                DialogueNode entryPoint = Nodes.Find(x => x.EntyPoint);
                if (entryPoint != null)
                {
                    entryPoint.GUID = _dialogueContainer.NodeLinks[0].FromGUID;
                    foreach (DialogueNode perNode in Nodes)
                    {
                        if (perNode.EntyPoint)
                        {
                            continue;
                        }
                        
                        Edges.Where(x => x.input.node == perNode).ToList()
                            .ForEach(edge => _graphView.RemoveElement(edge));
                        _graphView.RemoveElement(perNode);
                    }      
                }
                else
                {
                    Debug.LogError("GraphSaveUtility: Entry point node not found");
                }
            }
            else
            {
                Debug.LogError("GraphSaveUtility: Nodes list is null");
            }
        }
        
        private void GenerateDialogueNodes()
        {
            foreach (var perNode in _dialogueContainer.DialogueNodeData)
            {
                var tempNode = _graphView.CreateNode(perNode.Text, Vector2.zero);
                tempNode.GUID = perNode.GUID;
                _graphView.AddElement(tempNode);

                var nodePorts = _dialogueContainer.NodeLinks.Where(x => x.FromGUID == perNode.GUID).ToList();
                nodePorts.ForEach(x => _graphView.AddChoicePort(tempNode, x.PortName));
            }
        }

        private void ConnectDialogueNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var k = i; //Prevent access to modified closure
                var connections = _dialogueContainer.NodeLinks.Where(x => x.FromGUID == Nodes[k].GUID).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGUID = connections[j].ToGUID;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(
                        _dialogueContainer.DialogueNodeData.First(x => x.GUID == targetNodeGUID).Position,
                        _graphView.DefaultNodeSize));
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        private void AddExposedProperties()
        {
            _graphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in _dialogueContainer.ExposedProperties)
            {
                _graphView.AddPropertyToBlackBoard(exposedProperty);
            }
        }

        private void GenerateCommentBlocks()
        {
            foreach (var commentBlock in CommentBlocks)
            {
                _graphView.RemoveElement(commentBlock);
            }

            foreach (var commentBlockData in _dialogueContainer.CommentBlockData)
            {
               var block = _graphView.CreateCommentBlock(new Rect(commentBlockData.Position, _graphView.DefaultCommentBlockSize),
                    commentBlockData);
               block.AddElements(Nodes.Where(x=>commentBlockData.ChildNodes.Contains(x.GUID)));
            }
        }
    }
}