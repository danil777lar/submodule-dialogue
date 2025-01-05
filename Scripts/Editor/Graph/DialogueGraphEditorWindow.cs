using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Dialogue.Runtime.Graph;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Larje.Dialogue.Editor
{
    public class DialogueGraphEditorWindow : EditorWindow
    {
        private string _fileName = "New Narrative";
        private string _assetPath = "";
        
        private DialogueGraphView _graphView;
        private DialogueContainer _dialogueContainer;
        
        [OnOpenAsset(1)]
        public static bool OpenGraphAsset(int instanceID, int line)
        {
            UnityEngine.Object asset = EditorUtility.InstanceIDToObject(instanceID);
            if (!(asset is DialogueContainer))
            {
                return false;
            }
            
            bool windowIsOpen = EditorWindow.HasOpenInstances<DialogueGraphEditorWindow>();
            if (!windowIsOpen)
            {
                EditorWindow.CreateWindow<DialogueGraphEditorWindow>();
            }
            else
            {
                EditorWindow.FocusWindowIfItsOpen<DialogueGraphEditorWindow>();
            }

            DialogueGraphEditorWindow window = EditorWindow.GetWindow<DialogueGraphEditorWindow>();
            
            window.Initialize(AssetDatabase.GetAssetPath(instanceID));

            return true;
        }

        private void Initialize(string assetPath)
        {
            _assetPath = assetPath;
            _fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            
            Draw();
            LoadGraph();
        }

        private void Draw()
        {
            ClearVisualElement();
            ConstructGraphView();
            GenerateToolbar();
            //GenerateMiniMap();
            //GenerateBlackBoard();
        }
        
        private void OnDisable()
        {
            ClearVisualElement();
        }

        private void OnEnable()
        {
            Draw();
        }
        
        private void ClearVisualElement()
        {
            rootVisualElement.Clear();
        }

        private void ConstructGraphView()
        {
            _graphView = new DialogueGraphView(this)
            {
                name = _fileName,
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            Toolbar toolbar = new Toolbar();
            toolbar.Add(new Button(SaveGraph) {text = "Save"});
            rootVisualElement.Add(toolbar);
        }
        
        private void LoadGraph()
        {
            DialogueGraphEditorWindow window = GetWindow<DialogueGraphEditorWindow>();
            window.titleContent = new GUIContent(_fileName);
            
            DialogueGraphSaveUtility.GetInstance(_graphView).LoadNarrative(_assetPath);
        }

        private void SaveGraph()
        {
            DialogueGraphSaveUtility.GetInstance(_graphView).SaveGraph(_assetPath);
        }

        private void GenerateMiniMap()
        {
            MiniMap miniMap = new MiniMap {anchored = true};
            Vector2 cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
            miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
            _graphView.Add(miniMap);
        }

        private void GenerateBlackBoard()
        {
            var blackboard = new Blackboard(_graphView);
            blackboard.Add(new BlackboardSection {title = "Exposed Variables"});
            blackboard.addItemRequested = _blackboard =>
            {
                _graphView.AddPropertyToBlackBoard(ExposedProperty.CreateInstance(), false);
            };
            blackboard.editTextRequested = (_blackboard, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField) element).text;
                if (_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "This property name already exists, please chose another one.",
                        "OK");
                    return;
                }

                var targetIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
                _graphView.ExposedProperties[targetIndex].PropertyName = newValue;
                ((BlackboardField) element).text = newValue;
            };
            blackboard.SetPosition(new Rect(10,30,200,300));
            _graphView.Add(blackboard);
            _graphView.Blackboard = blackboard;
        }
    }
}