using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Dialogue.Editor.Utility;
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
        private string _assetPath;
        private NodeSearchWindow _searchWindow;
        private DialogueGraphContainer _savedRecord;
        private DialogueGraphContainer _currentRecord;
        private Dictionary<DialogueGraphContainer, string> _undoStack = new Dictionary<DialogueGraphContainer, string>();

        public DialogueGraphView(DialogueGraphEditorWindow editorWindow, string assetPath)
        {
            _assetPath = assetPath;
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

        public void AddNode(GraphNode node, bool notify = true)
        {
            node.EventRemovePort += OnPortRemoved;
            AddElement(node);
            
            node.RefreshPorts();
            node.RefreshExpandedState();
            
            if (notify)
            {
                Record("Add Node");
            }
        }
        
        public void AddEdge(Edge edge, bool notify = true)
        {
            Add(edge);
            
            if (notify)
            {
                Record("Add Edge");
            }
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
        
        public void Save()
        {
            _savedRecord = DialogueGraphSaver.SaveGraph(this, _assetPath);
            _undoStack.Clear();
            _currentRecord = _savedRecord;
        }

        public void Load()
        {
            _savedRecord = DialogueGraphLoader.LoadGraph(this, _assetPath);
            _undoStack.Clear();
            _currentRecord = _savedRecord;
        }

        public void Record(string actionName)
        {
            DialogueGraphContainer newState;
            if (DialogueGraphSaver.TrySaveState(this, _currentRecord, out newState))
            {
                _undoStack.Add(newState, actionName);
                _currentRecord = newState;
            }

            LogUndoCurrentStack();
        }
        
        public void Undo()
        {
            if (_undoStack.Count > 0 && _currentRecord != null && _currentRecord != _savedRecord)
            {
                int index = _undoStack.Keys.ToList().IndexOf(_currentRecord);
                if (index > 0)
                {
                    _currentRecord = _undoStack.Keys.ToList()[index - 1];
                }
                else
                {
                    _currentRecord = _savedRecord;
                }
                DialogueGraphLoader.LoadState(this, _currentRecord);
            }            
            
            LogUndoCurrentStack();
        }

        public void Redo()
        {
            if (_currentRecord != null)
            {
                int index = _undoStack.Keys.ToList().IndexOf(_currentRecord);
                if (index < _undoStack.Count - 1)
                {
                    _currentRecord = _undoStack.Keys.ToList()[index + 1];
                    DialogueGraphLoader.LoadState(this, _currentRecord);
                }
            }
            
            LogUndoCurrentStack();
        }

        private void LogUndoCurrentStack()
        {
            string debug = "";
            foreach (KeyValuePair<DialogueGraphContainer, string> record in _undoStack)
            {
                string marker = _currentRecord == record.Key ? "=>    " : "";
                debug += $"{marker}{record.Value} \n";
            }

            Debug.Log(debug);
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