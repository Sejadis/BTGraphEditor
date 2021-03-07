using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AI.BT;
using AI.BTGraph.Attribute;

namespace AI.BTGraph
{
    public class RuntimeNodeData
    {
        public Dictionary<string, Type> inputTypes;

        public Dictionary<string, Type> outputTypes;
        public Type type;
        //TODO combine children bools into 1 field
        public bool allowMultipleChildren;
        public bool hasNoChildren;

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

            // PropertyInfo allowMultiChildrenProperty = null;
            //
            // Type testingType = type;
            // var maxDepth = 5;
            // do
            // {
            //     allowMultiChildrenProperty =
            //         testingType.GetInterfaces().Contains(typeof(IMayHaveMultipleChildren));
            //     testingType = type.BaseType;
            // } while (allowMultiChildrenProperty == null && testingType != null && maxDepth-- > 0);
            // //
            // // if (allowMultiChildrenProperty == null)
            // // {
            // //     typeof(BTNode).GetProperty("AllowMultipleChildren", BindingFlags.Public | BindingFlags.Static);
            // // }
            //
            // var value = allowMultiChildrenProperty?.GetValue(null);
            // allowMultipleChildren = (bool?) value ?? true;
            allowMultipleChildren = type.GetInterfaces().Contains(typeof(IMayHaveMultipleChildren));
            hasNoChildren = type.GetInterfaces().Contains(typeof(IHasNoChildren));
        }
    }
}