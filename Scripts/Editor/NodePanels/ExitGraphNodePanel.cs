using Larje.Dialogue.Editor;
using UnityEngine;
using UnityEngine.UIElements;

public class ExitGraphNodePanel : GraphNodePanel
{
    private ExitGraphNode _node;
    
    public ExitGraphNodePanel(ExitGraphNode node)
    {
        _node = node;
    }
    
    public override void Draw(VisualElement root)
    {
        base.Draw(root);
    }
}
