using System;
using System.Collections.Generic;
using Larje.Dialogue.Editor;
using Larje.Dialogue.Runtime.Converted;
using PlasticGui.WorkspaceWindow.CodeReview;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Action = System.Action;

public class DialogueGraphNodeWindow : EditorWindow
{
    private const float WIDTH = 400f;
    private const float HEIGHT = 500f;

    private int _selectedLangIndex;
    private DialogueGraphNode _node;
    private Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();
    
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
        titleContent.text = $"Dialogue Node: {_node.Content.Localizations[0].Speech.Title}";
        
        DrawToolbar();
        DrawLocalization();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal();

        int index = 0; 
        foreach (DialogueLocalization speech in _node.Content.Localizations)
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
            _node.Content.Localizations.Add(new DialogueLocalization()
            {
                LanguageCode = "new",
                Speech = new Speech()
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
        DialogueLocalization localization = _node.Content.Localizations[_selectedLangIndex];
        
        DrawFoldout("MAIN OPTIONS", null, new Action[]
        {
            () =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Language Code:", GUILayout.Width(100f));
                GUILayout.Space(10f);
                localization.LanguageCode = GUILayout.TextField(localization.LanguageCode, GUILayout.Width(30f));
                GUILayout.EndHorizontal();
            },

            () =>
            {
                if (GUILayout.Button("Delete", GUILayout.Width(150f)))
                {
                    _node.Content.Localizations.Remove(localization);
                    _selectedLangIndex = Mathf.Max(0, _selectedLangIndex - 1);            
                }
            },
            
            () =>
            {
                if (GUILayout.Button("Add choice", GUILayout.Width(150f)))
                {
                    foreach (DialogueLocalization loc in _node.Content.Localizations)
                    {
                        loc.Choices.Add(new Speech());
                    }
                                
                }
            }
        });
        
        DrawSpeech(localization);
        DrawChoices(localization.Choices);
    }

    private void DrawSpeech(DialogueLocalization localization)
    {
        DrawFoldout("MAIN SPEECH", null, new Action[]
        {
            () =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Title:", GUILayout.Width(50f));
                localization.Speech.Title = GUILayout.TextField(localization.Speech.Title);
                GUILayout.EndHorizontal();
            },
            () =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Text:", GUILayout.Width(50f));
                localization.Speech.Text = GUILayout.TextField(localization.Speech.Text);
                GUILayout.EndHorizontal();
            }
        });
    }

    private void DrawChoices(List<Speech> choices)
    {
        GUILayout.Space(20f);
        if (!_foldouts.ContainsKey("CHOICES"))
        {
            _foldouts.Add("CHOICES", true);
        }
        
        _foldouts["CHOICES"] = EditorGUILayout.Foldout(_foldouts["CHOICES"], "CHOICES");
        if (_foldouts["CHOICES"])
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
        
            foreach (Speech choice in choices)
            {
                DrawFoldout($"{choices.IndexOf(choice)}: {choice.Title}", null, new Action[]
                {
                    () =>
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Title:", GUILayout.Width(50f));
                        choice.Title = GUILayout.TextField(choice.Title);
                        GUILayout.EndHorizontal();
                    },
                    () =>
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Text:", GUILayout.Width(50f));
                        choice.Text = GUILayout.TextField(choice.Text);
                        GUILayout.EndHorizontal();
                    }
                });
            }
        
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();   
        }
    }

    private void DrawFoldout(string foldoutName, Action titleLine, Action[] lines)
    {
        GUILayout.Space(10f);
        if (!_foldouts.ContainsKey(foldoutName))
        {
            _foldouts.Add(foldoutName, true);
        }
        
        GUILayout.BeginHorizontal();
        _foldouts[foldoutName] = EditorGUILayout.Foldout(_foldouts[foldoutName], foldoutName);
        titleLine?.Invoke();
        GUILayout.EndHorizontal();

        if (_foldouts[foldoutName])
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            
            foreach (Action line in lines)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("|", GUILayout.Width(10f));
                line();
                GUILayout.EndHorizontal();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
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
