using System;
using System.Collections.Generic;
using System.Reflection;
using AI.BT;

namespace AI.BTGraph
{
    public class RuntimeNodeData
    {
        public Dictionary<string, Type> inputTypes;

        public Dictionary<string, Type> outputTypes;
        public Type type;
        public bool allowMultipleChildren;

        public RuntimeNodeData(Type type, Dictionary<string, Type> inputTypes, bool allowMultipleChildren)
        {
            this.type = type;
            this.inputTypes = inputTypes;
            this.allowMultipleChildren = allowMultipleChildren;
        }

        public RuntimeNodeData(Type type)
        {
            this.type = type;
            inputTypes = new Dictionary<string, Type>();
            outputTypes = new Dictionary<string, Type>();
            
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute(typeof(InputAttribute)) != null)
                {
                    inputTypes[field.Name] = field.FieldType;
                }
                if (field.GetCustomAttribute(typeof(OutputAttribute)) != null)
                {
                    outputTypes[field.Name] = field.FieldType;
                }
            }

            PropertyInfo allowMultiChildrenProperty =
                type.GetProperty("AllowMultipleChildren", BindingFlags.Public | BindingFlags.Static);

            if (allowMultiChildrenProperty == null)
            {
                typeof(BTNode).GetProperty("AllowMultipleChildren", BindingFlags.Public | BindingFlags.Static);
            }

            var value = allowMultiChildrenProperty?.GetValue(null);
            allowMultipleChildren = (bool?) value ?? true;
        }
    }
}