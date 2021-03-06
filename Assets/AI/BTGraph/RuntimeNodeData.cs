using System;
using System.Collections.Generic;
using System.Reflection;
using AI.BT;
using AI.BT.Nodes;
using AI.BTGraph.Attribute;

namespace AI.BTGraph
{
    public class RuntimeNodeData
    {
        public Dictionary<string, KeyTypePair> inputTypes;

        public Dictionary<string, KeyTypePair> outputTypes;

        public Type type;

        //TODO combine children bools into 1 field
        public bool allowMultipleChildren;
        public bool hasNoChildren;

        public RuntimeNodeData(Type type, Dictionary<string, KeyTypePair> inputTypes, bool allowMultipleChildren)
        {
            this.type = type;
            this.inputTypes = inputTypes;
            this.allowMultipleChildren = allowMultipleChildren;
        }

        public RuntimeNodeData(Type type)
        {
            this.type = type;
            inputTypes = new Dictionary<string, KeyTypePair>();
            outputTypes = new Dictionary<string, KeyTypePair>();

            var fields = type.GetFields();
            foreach (var field in fields)
            {
                var isInput = field.GetCustomAttribute(typeof(InputAttribute)) != null;
                var isOutput = field.GetCustomAttribute(typeof(OutputAttribute)) != null;
                var allowsManualInput = field.GetCustomAttribute(typeof(OptionAttribute)) != null;

                var ktp = new KeyTypePair("", field.FieldType, allowsManualInput);
                if (isInput)
                {
                    inputTypes[field.Name] = ktp;
                }

                if (isOutput)
                {
                    outputTypes[field.Name] = ktp;
                }
            }

            allowMultipleChildren = type.IsSubclassOf(typeof(CompositeNode));
            hasNoChildren = type.IsSubclassOf(typeof(LeafNode));
        }

        public RuntimeNodeData(BTNode node) : this(node.GetType())
        {
            var fields = node.GetBlackboardAccessorFieldInfos();
            foreach (var field in fields)
            {
                if (inputTypes.TryGetValue(field.Name, out var ktp) || outputTypes.TryGetValue(field.Name, out ktp))
                {
                    if (node.GetOrCreateBlackboardAccessor(field) is BlackboardAccessor accessor)
                    {
                        ktp.key = accessor.Key;
                        ktp.overrideValue = accessor.OverrideValue as string;
                    }
                }
            }
        }
    }
}