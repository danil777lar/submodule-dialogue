using System.Linq;
using Larje.Dialogue.Editor;
using Larje.Dialogue.Runtime.Graph;
using Larje.Dialogue.Runtime.Graph.Data;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
        Edge[] connectedSockets = view.edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedSockets.Count(); i++)
        {
            DialogueGraphNode outputNode = (connectedSockets[i].output.node as DialogueGraphNode);
            DialogueGraphNode inputNode = (connectedSockets[i].input.node as DialogueGraphNode);
            container.NodeLinks.Add(new LinkData
            {
                FromGUID = outputNode.GUID,
                PortName = connectedSockets[i].output.portName,
                ToGUID = inputNode.GUID
            });
        }

        foreach (Node node in view.nodes)
        {
            if (node is GraphNode graphNode)
            {
                container.NodeData.Add(new NodeData
                {
                    GUID = graphNode.GUID,
                    Position = graphNode.GetPosition().position
                });
            }
        }

        return true;
    }
}
