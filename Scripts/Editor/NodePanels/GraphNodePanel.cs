using Larje.Dialogue.Editor;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphNodePanel
{
    protected virtual GraphNode Node { get; set; }
    
    public virtual void Draw(VisualElement root)
    {
        root.Add(new Label($"{this.GetType()}"));
    }
}
