using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Larje.Dialogue.Runtime.Graph.Data
{
    [Serializable]
    public class NodeData
    {
        public string Type;
        public Vector2 Position;
        public List<Field> Fields;
        
        public T GetField<T>(string name)
        {
            Field field = Fields.Find(x => x.Name == name);
            if (field != null)
            {
                return (T) Convert.ChangeType(field.Value, typeof(T));
            }
            
            return default;
        }
        
        public object GetField(string name)
        {
            Field field = Fields.Find(x => x.Name == name);
            if (field != null)
            {
                Assembly asm = typeof(string).Assembly;
                Type type = asm.GetType(field.Type);
                return Convert.ChangeType(field.Value, type);
            }
            
            return null;
        }

        public bool IsTypeOf(string type)
        {
            return Type.Split(".").Last() == type.Split(".").Last();
        }
        
        [Serializable]
        public class Field
        {
            public string Name;
            public string Type;
            public string Value;
        }
    }
}