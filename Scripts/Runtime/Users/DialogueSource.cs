using System;
using System.Linq;
using Larje.Dialogue.Runtime.Graph;
using UnityEngine;

public class DialogueSource : MonoBehaviour
{
    [SerializeField] protected DialogueGraphContainer dialogue;
    
    protected virtual void SendEvent(string eventName)
    {
        IDialogueEventProcessor[] events = GetComponentsInChildren<IDialogueEventProcessor>();
        foreach (IDialogueEventProcessor eventProcessor in events)
        {
            eventProcessor.SendEvent(eventName);
        }
    }
    
    protected virtual bool CheckCondition(string condition)
    {
        bool result = false;
        IDialogueConditionProcessor[] conditions = GetComponentsInChildren<IDialogueConditionProcessor>();
        foreach (IDialogueConditionProcessor c in conditions)
        {
            if (c.TryCheckCondition(condition, out bool conditionResult))
            {
                result &= conditionResult;
                break;
            }
        }
        
        return result;
    }
}
