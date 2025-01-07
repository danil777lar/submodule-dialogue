using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Larje.Dialogue.Editor;
using Larje.Dialogue.Runtime.Graph;
using Larje.Dialogue.Runtime.Graph.Data;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Larje.Dialogue.Editor.Utility
{
    public static class DialogueGraphSaver
    {
        public static void SaveGraph(DialogueGraphView view, string assetPath)
        {
            DialogueGraphContainer container = AssetDatabase.LoadAssetAtPath<DialogueGraphContainer>(assetPath);

            SaveNodes(container, view);

            EditorUtility.SetDirty(container);
            AssetDatabase.SaveAssets();
        }

        private static bool SaveNodes(DialogueGraphContainer container, DialogueGraphView view)
        {
            container.NodeLinks = new List<LinkData>();
            container.NodeData = new List<NodeData>();

            Edge[] connectedSockets = view.edges.Where(x => x.input.node != null).ToArray();
            for (int i = 0; i < connectedSockets.Count(); i++)
            {
                GraphNode outputNode = (connectedSockets[i].output.node as GraphNode);
                GraphNode inputNode = (connectedSockets[i].input.node as GraphNode);
                container.NodeLinks.Add(new LinkData
                {
                    FromGUID = outputNode.GUID,
                    FromPortName = connectedSockets[i].output.portName,
                    ToGUID = inputNode.GUID,
                    ToPortName = connectedSockets[i].input.portName
                });
            }

            foreach (Node node in view.nodes)
            {
                if (node is GraphNode graphNode)
                {
                    container.NodeData.Add(GetData(graphNode));
                }
            }

            return true;
        }

        private static NodeData GetData(GraphNode node)
        {
            NodeData data = new NodeData();

            data.Type = node.GetType().ToString();
            data.Position = node.GetPosition().position;
            
            data.Fields = new List<NodeData.Field>();
            foreach (FieldInfo field in node.GetType().GetFields())
            {
                data.Fields.Add(new NodeData.Field
                {
                    Name = field.Name,
                    Type = field.FieldType.ToString(),
                    Value = field.GetValue(node)?.ToString()
                });
            }

            return data;
        }
    }
}