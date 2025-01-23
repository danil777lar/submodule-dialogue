using Larje.Dialogue.Editor;
using UnityEngine;
using UnityEngine.UIElements;

public class EnterGraphNodePanel : GraphNodePanel
{
    private EnterGraphNode _node;
    
    public EnterGraphNodePanel(EnterGraphNode node)
    {
        _node = node;
    }
    
    public override void Draw(VisualElement root)
    {
        base.Draw(root);
    }
}
