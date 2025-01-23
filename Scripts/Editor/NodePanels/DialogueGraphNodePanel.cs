using System;
using System.Collections.Generic;
using Codice.CM.Common;
using Larje.Dialogue.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

public class DialogueGraphNodePanel : GraphNodePanel
{
    private int _selectedLangIndex;
    private DialogueGraphNode _node;
    
    public DialogueGraphNodePanel(DialogueGraphNode node)
    {
        _node = node;
    }

    public override void Draw(VisualElement root)
    {
        base.Draw(root);
        root.Add(DrawToolbar());
        DrawSettings();
        DrawMainSpeech();
        //root.Add(DrawMainSpeech());
    }

    private VisualElement DrawToolbar()
    {
        ToolbarMenu toolbar = new ToolbarMenu();
        toolbar.text = $"Language: {_node.Content.Localizations[_selectedLangIndex].LanguageCode.ToUpper()}";

        int index = 0; 
        foreach (DialogueLocalization speech in _node.Content.Localizations)
        {
            bool selected = index == _selectedLangIndex;
            toolbar.menu.AppendAction(speech.LanguageCode.ToUpper(), (action) =>
            {
                _selectedLangIndex = _node.Content.Localizations.IndexOf(speech);
                Refresh();
            });

            index++;
        }

        toolbar.Add(new ToolbarSpacer());
        
        toolbar.Add(new Button(() =>
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
            _selectedLangIndex = _node.Content.Localizations.Count - 1;
            Refresh();
        })
        {
            text = "+"
        });
        
        return toolbar;
    }

    private void DrawSettings()
    {
        DialogueLocalization localization = _node.Content.Localizations[_selectedLangIndex];
        Box settings = GetBox(_root, "MAIN SETTINGS:");

        TextField languageCodeField = new TextField();
        languageCodeField.label = "Language Code:";
        languageCodeField.value = localization.LanguageCode;
        languageCodeField.RegisterCallback<ChangeEvent<string>>((value) =>
        {
            localization.LanguageCode = value.newValue;
        });
        languageCodeField.RegisterCallback<FocusOutEvent>((value) =>
        {
            Refresh();
        });
        settings.Add(languageCodeField);

        Button deleteButton = new Button(() =>
        {
            _node.Content.Localizations.Remove(localization);
            _selectedLangIndex = Mathf.Max(0, _selectedLangIndex - 1);
            Refresh();
        });
        deleteButton.text = "Delete";
        settings.Add(deleteButton);
        
        Button addChoiceButton =  new Button(() =>
        {
            foreach (DialogueLocalization loc in _node.Content.Localizations)
            {
                loc.Choices.Add(new Speech());
            }
            Refresh();
        });
        addChoiceButton.text = "Add choice";
        settings.Add(addChoiceButton);
    }
    
    private void DrawMainSpeech()
    {
        DialogueLocalization localization = _node.Content.Localizations[_selectedLangIndex];
        Box box = GetBox(_root, "MAIN SPEECH:");
        
        TextField titleField = new TextField();
        titleField.label = "Title:";
        titleField.value = localization.Speech.Title;
        titleField.RegisterCallback<ChangeEvent<string>>((value) =>
        {
            localization.Speech.Title = value.newValue;
        });
        titleField.RegisterCallback<FocusOutEvent>((value) =>
        {
            Refresh();
        });
        box.Add(titleField);
        
        TextField textField = new TextField();
        textField.style.whiteSpace = WhiteSpace.Normal;
        textField.multiline = true;
        textField.label = "Text:";
        textField.value = localization.Speech.Text;
        textField.RegisterCallback<ChangeEvent<string>>((value) =>
        {
            localization.Speech.Text = value.newValue;
        });
        textField.RegisterCallback<FocusOutEvent>((value) =>
        {
            Refresh();
        });
        box.Add(textField);
        

    }

    private void DrawSpeech(DialogueLocalization localization)
    {
        /*DrawFoldout("MAIN SPEECH", null, new Action[]
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
        });*/
    }

    private void DrawChoices(List<Speech> choices)
    {
        /*GUILayout.Space(20f);
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
        }*/
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
