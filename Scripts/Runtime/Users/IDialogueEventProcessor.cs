using UnityEngine;

public interface IDialogueEventProcessor
{
    public void SendEvent(string eventId);
    public string[] SupportedEvents { get; }
}
