using System.Collections.Generic;
using UnityEngine;

namespace Larje.Dialogue.Runtime.Converted
{
    public class Dialogue
    {
        public Step StartStep;
        public List<Step> Steps;

        public class Step
        {
            public string Id;
            public string Text;
            public List<Choice> Choices;
        }

        public class Choice
        {
            public string Text;
            public string NextStepId;
            public List<string> Events;
        }
    }
}