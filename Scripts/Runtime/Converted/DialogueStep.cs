using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Dialogue.Runtime.Converted
{
    public class DialogueStep
    {
        public string Id;
        public string Text;
        public List<Choice> Choices;

        public class Choice
        {
            public string Text;
        }
    }
}