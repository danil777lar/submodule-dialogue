using Larje.Dialogue.Editor;
using UnityEngine;
using UnityEngine.UIElements;

public class ConditionGraphNodePanel : GraphNodePanel
{
    private ConditionGraphNode _node;
    
    public ConditionGraphNodePanel(ConditionGraphNode node)
    {
        _node = node;
    }
    
    public override void Draw(VisualElement root)
    {
        base.Draw(root);
    }
}
