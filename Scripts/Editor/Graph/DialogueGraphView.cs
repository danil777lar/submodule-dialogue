using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Dialogue.Runtime.Graph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Larje.Dialogue.Editor
{
    public class DialogueGraphView : GraphView
    {
        public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
        
        public Blackboard Blackboard = new Blackboard();
        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
        private NodeSearchWindow _searchWindow;

        public DialogueGraphView(DialogueGraphEditorWindow editorWindow)
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("DialogueGraph/GraphView");
            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
            
            AddSearchWindow(editorWindow);
        }

        public void AddNode(GraphNode node)
        {
            node.EventRemovePort += OnPortRemoved;
            AddElement(node);
            
            node.RefreshPorts();
            node.RefreshExpandedState();
        }
        
        public void AddEdge(Edge edge)
        {
            Add(edge);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            Port startPortView = startPort;

            ports.ForEach((port) =>
            {
                Port portView = port;
                if (startPortView != portView && startPortView.node != portView.node)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        private void OnPortRemoved(Port port)
        {
            IEnumerable<Edge> targetEdge = edges.ToList()
                .Where(x => x.output.portName == port.portName && x.output.node == port.node);
            
            if (targetEdge.Any())
            {
                Edge edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }
        }

        private void AddSearchWindow(DialogueGraphEditorWindow editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }
    }
}