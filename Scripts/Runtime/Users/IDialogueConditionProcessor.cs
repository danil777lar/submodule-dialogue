using UnityEngine;

public interface IDialogueConditionProcessor
{
    public bool TryCheckCondition(string condition, out bool result);
}
