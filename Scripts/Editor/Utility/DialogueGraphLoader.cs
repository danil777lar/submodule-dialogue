using System;
using System.Linq;
using System.Reflection;
using Larje.Dialogue.Editor;
using Larje.Dialogue.Runtime.Graph;
using Larje.Dialogue.Runtime.Graph.Data;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;

namespace Larje.Dialogue.Editor.Utility
{
    public static class DialogueGraphLoader
    {
        public static void LoadGraph(DialogueGraphView view, string assetPath)
        {
            DialogueGraphContainer container = AssetDatabase.LoadAssetAtPath<DialogueGraphContainer>(assetPath);

            LoadNodes(container, view);

            EditorUtility.SetDirty(container);
            AssetDatabase.SaveAssets();
        }

        private static void LoadNodes(DialogueGraphContainer container, DialogueGraphView view)
        {
            foreach (LinkData link in container.NodeLinks)
            {
                /*GraphNode outputNode = view.nodes.Find(x => x.GUID == link.FromGUID);
                GraphNode inputNode = view.nodes.Find(x => x.GUID == link.ToGUID);

                view.AddEdge(view.GenerateEdge(outputNode.outputContainer.Children().First(x => x.name == link.FromPortName) as Port,
                    inputNode.inputContainer.Children().First(x => x.name == link.ToPortName) as Port));*/
            }

            foreach (NodeData nodeData in container.NodeData)
            {
                GraphNode node = Activator.CreateInstance(Type.GetType(nodeData.Type)) as GraphNode;
                
                foreach (NodeData.Field field in nodeData.Fields)
                {
                    FieldInfo fieldInfo = node.GetType().GetField(field.Name);
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(node, nodeData.GetField(field.Name));
                    }
                }
                
                view.AddNode(node.Load(nodeData.Position));
            }
        }
    }
}