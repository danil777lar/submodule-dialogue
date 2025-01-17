using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContent
{
    public List<DialogueLocalization> Localizations;
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
