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
        private DialogueGraphView _graphView;
        private DialogueContainer _dialogueContainer;
        
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<GraphNode> Nodes => _graphView.nodes.ToList().Cast<GraphNode>().ToList();

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
            Edge[] connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (int i = 0; i < connectedSockets.Count(); i++)
            {
                DialogueGraphNode outputNode = (connectedSockets[i].output.node as DialogueGraphNode);
                DialogueGraphNode inputNode = (connectedSockets[i].input.node as DialogueGraphNode);
                dialogueContainerObject.NodeLinks.Add(new DialogueNodeLinkData
                {
                    FromGUID = outputNode.GUID,
                    PortName = connectedSockets[i].output.portName,
                    ToGUID = inputNode.GUID
                });
            }

            foreach (DialogueGraphNode node in Nodes)
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
        }
        
        private void ClearGraph()
        {
            if (_dialogueContainer.NodeLinks.Count <= 0)
            {
                return;
            }

            if (Nodes != null)
            {
                foreach (DialogueGraphNode perNode in Nodes)
                {
                    Edges.Where(x => x.input.node == perNode).ToList()
                        .ForEach(edge => _graphView.RemoveElement(edge));
                    _graphView.RemoveElement(perNode);
                }
            }
            else
            {
                Debug.LogError("GraphSaveUtility: Nodes list is null");
            }
        }
        
        private void GenerateDialogueNodes()
        {
            /*foreach (DialogueNodeData perNode in _dialogueContainer.DialogueNodeData)
            {
                var tempNode = _graphView.CreateNode(perNode.Text, Vector2.zero);
                tempNode.GUID = perNode.GUID;
                _graphView.AddElement(tempNode);

                List<DialogueNodeLinkData> nodePorts = _dialogueContainer.NodeLinks.Where(x => x.FromGUID == perNode.GUID).ToList();
                nodePorts.ForEach(x => _graphView.AddChoicePort(tempNode, x.PortName));
            }*/
        }

        private void ConnectDialogueNodes()
        {
            /*for (int i = 0; i < Nodes.Count; i++)
            {
                int k = i; //Prevent access to modified closure
                List<DialogueNodeLinkData> connections = _dialogueContainer.NodeLinks.Where(x => x.FromGUID == Nodes[k].GUID).ToList();
                for (int j = 0; j < connections.Count(); j++)
                {
                    string targetNodeGUID = connections[j].ToGUID;
                    DialogueGraphNode targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(
                        _dialogueContainer.DialogueNodeData.First(x => x.GUID == targetNodeGUID).Position,
                        _graphView.DefaultNodeSize));
                }
            }*/
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            Edge tempEdge = new Edge()
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
    }
}