using Larje.Dialogue.Editor;
using PlasticGui.WorkspaceWindow.CodeReview;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphNodeWindow : EditorWindow
{
    private const float WIDTH = 400f;
    private const float HEIGHT = 500f;
    
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
        ClearUI();
        InitSize();
        DrawUI();
    }

    private void ClearUI()
    {
        rootVisualElement.Clear();
    }

    private void InitSize()
    {
        maxSize = new Vector2(WIDTH, HEIGHT);
        minSize = maxSize;
    }    
    
    private void DrawUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy
        Label label = new Label("Hello World!");
        root.Add(label);

        // Create button
        Button button = new Button();
        button.name = "button";
        button.text = "Button";
        root.Add(button);

        // Create toggle
        Toggle toggle = new Toggle();
        toggle.name = "toggle";
        toggle.label = "Toggle";
        root.Add(toggle);
    }
}
