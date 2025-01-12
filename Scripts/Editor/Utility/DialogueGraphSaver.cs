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
            container.Links = new List<LinkData>();
            container.Nodes = new List<NodeData>();

            List<Edge> edges = view.edges.ToList().FindAll(x => x.input.node != null);
            for (int i = 0; i < edges.Count(); i++)
            {
                GraphNode outputNode = (edges[i].output.node as GraphNode);
                GraphNode inputNode = (edges[i].input.node as GraphNode);
                container.Links.Add(new LinkData
                {
                    FromGUID = outputNode.GUID,
                    FromPortName = edges[i].output.portName,
                    ToGUID = inputNode.GUID,
                    ToPortName = edges[i].input.portName
                });
            }

            foreach (Node node in view.nodes)
            {
                if (node is GraphNode graphNode)
                {
                    container.Nodes.Add(GetData(graphNode));
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