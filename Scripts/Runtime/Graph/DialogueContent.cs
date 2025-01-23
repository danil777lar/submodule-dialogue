using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContent
{
    public List<DialogueLocalization> Localizations;

    public void AddLocalization(string languageCode)
    {
        DialogueLocalization localization = new DialogueLocalization
        {
            LanguageCode = languageCode,
            Speech = new Speech(),
            Choices = new List<Speech>()
        };
        
        Localizations.Add(localization);
    }
    
    public void RemoveLocalization(DialogueLocalization localization)
    {
        
    }
}
        
[Serializable]
public class DialogueLocalization
{
    public string LanguageCode;
    public Speech Speech;
    public List<Speech> Choices;
}

[Serializable]
public class Speech
{
    public string Title;
    public string Text;
}
