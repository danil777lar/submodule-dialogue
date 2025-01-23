using Larje.Dialogue.Editor;
using UnityEngine;
using UnityEngine.UIElements;

public class EventGraphNodePanel : GraphNodePanel
{
    private EventGraphNode _node;
    
    public EventGraphNodePanel(EventGraphNode node)
    {
        _node = node;
    }
    
    public override void Draw(VisualElement root)
    {
        base.Draw(root);
    }
}
