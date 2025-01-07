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
        private DialogueGraphContainer _dialogueContainer;
        
        [OnOpenAsset(1)]
        public static bool OpenGraphAsset(int instanceID, int line)
        {
            UnityEngine.Object asset = EditorUtility.InstanceIDToObject(instanceID);
            if (!(asset is DialogueGraphContainer))
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
            
            GetWindow<DialogueGraphEditorWindow>()
                .Initialize(AssetDatabase.GetAssetPath(instanceID));

            return true;
        }

        private void Initialize(string assetPath)
        {
            _assetPath = assetPath;
            _fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            titleContent = new GUIContent(_fileName);
            
            Draw();
            LoadGraph();
        }

        private void Draw()
        {
            ClearVisualElement();
            ConstructGraphView();
            GenerateToolbar();
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
            DialogueGraphLoader.LoadGraph(_graphView, _assetPath);
        }

        private void SaveGraph()
        {
            DialogueGraphSaver.SaveGraph(_graphView, _assetPath);
        }

        private void GenerateMiniMap()
        {
            MiniMap miniMap = new MiniMap {anchored = true};
            Vector2 cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
            miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
            _graphView.Add(miniMap);
        }
    }
}