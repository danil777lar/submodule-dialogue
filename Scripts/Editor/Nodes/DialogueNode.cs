﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Larje.Dialogue.Editor
{
    public class DialogueNode : Node
    {
        public string DialogueText;
        public string GUID;
        public bool EntyPoint = false;
    }
}