using System;
using Larje.Dialogue.Editor;
using PlasticGui.WorkspaceWindow.CodeReview;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphNodeWindow : EditorWindow
{
    private const float WIDTH = 400f;
    private const float HEIGHT = 500f;

    private int _selectedLangIndex;
    private DialogueGraphNode _node;
    
    public static void OpenWindow(DialogueGraphNode node)
    {
        bool windowIsOpen = EditorWindow.HasOpenInstances<DialogueGraphNodeWindow>();
        if (!windowIsOpen)
        {
            EditorWindow.CreateWindow<DialogueGraphNodeWindow>();
        }
        else
        {
            EditorWindow.FocusWindowIfItsOpen<DialogueGraphNodeWindow>();
        }
            
        GetWindow<DialogueGraphNodeWindow>()
            .Initialize(node);
    }

    public void Initialize(DialogueGraphNode node)
    {
        _node = node;
        maxSize = new Vector2(WIDTH, HEIGHT);
        minSize = maxSize;
    }
    
    private void OnGUI()
    {
        DrawToolbar();
        DrawLocalization();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal();

        int index = 0; 
        foreach (DialogueGraphNode.DialogueLocalization speech in _node.Content.Localizations)
        {
            bool selected = index == _selectedLangIndex;
            GUILayoutOption[] options = GetLangButtonOptions();
            GUIStyle style = selected ? GetLangButtonSelectedStyle() : GetLangButtonStyle();
            if (GUILayout.Button(speech.LanguageCode.ToUpper(), style, options))
            {
                _selectedLangIndex = _node.Content.Localizations.IndexOf(speech);
            }

            index++;
            GUILayout.Space(10);
        }
        
        if (GUILayout.Button("+", GetLangButtonStyle(), GetLangButtonOptions()))
        {
            _node.Content.Localizations.Add(new DialogueGraphNode.DialogueLocalization()
            {
                LanguageCode = "new",
                Speech = new DialogueGraphNode.Speech()
                {
                    Title = "Title",
                    Text = "Text" 
                }
            });
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLocalization()
    {
        DialogueGraphNode.DialogueLocalization localization = _node.Content.Localizations[_selectedLangIndex];
        if (GUILayout.Button("Delete"))
        {
            _node.Content.Localizations.Remove(localization);
            _selectedLangIndex = Mathf.Max(0, _selectedLangIndex - 1);            
        }

        GUILayout.Label("Language Code");
        localization.LanguageCode = GUILayout.TextField(localization.LanguageCode);
        DrawSpeechUI(localization.Speech);
    }

    private void DrawSpeechUI(DialogueGraphNode.Speech speech)
    {
        GUILayout.Space(10f);
        
        GUILayout.Label("Title");
        speech.Title = GUILayout.TextField(speech.Title);
        
        GUILayout.Label("Text");
        speech.Text = GUILayout.TextField(speech.Text);
    }
    
    private GUILayoutOption[] GetLangButtonOptions()
    {
        int size = 30;
        return new GUILayoutOption[]
        {
            GUILayout.Height(size),
            GUILayout.Width(size),
            GUILayout.ExpandHeight(false),
            GUILayout.ExpandWidth(true)
        };
    }
    
    private GUIStyle GetLangButtonStyle()
    {
        int padding = 3;
        return new GUIStyle(GUI.skin.button)
        {
            padding = new RectOffset(padding, padding, padding, padding),
            normal =
            {
                background = Texture2D.blackTexture,
            }
        };
    }
    
    private GUIStyle GetLangButtonSelectedStyle()
    {
        int padding = 3;
        return new GUIStyle(GUI.skin.button)
        {
            padding = new RectOffset(padding, padding, padding, padding),
        };
    }
}
