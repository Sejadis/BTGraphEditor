using System;
using System.Collections.Generic;
using System.Linq;
using AI.BT.Nodes;
using AI.BT.Nodes.Composite;
using AI.BT.Nodes.Decorator;
using AI.BTGraph;
using UnityEngine;

namespace AI.BT.Serialization
{
    [Serializable]
    public class SerializedBTNode
    {
        public string type;
        public string guid;
        public string parent;
        public List<string> children;
        public Rect graphRect;
        public List<PropertyKeyPair> propertyKeyMap;

        public SerializedBTNode(BTNode node)
        {
            guid = node.Guid.ToString();
            type = node.GetType().ToString();
            parent = node.Parent?.Guid.ToString() ?? String.Empty;
            children = new List<string>();
            switch (node)
            {
                case CompositeNode compositeNode:
                {
                    foreach (var child in compositeNode.Children)
                    {
                        children.Add(child.Guid.ToString());
                    }
                    break;
                }
                case DecoratorNode {child: { }} decoratorNode:
                {
                    children.Add(decoratorNode.child.Guid.ToString());
                    break;
                }
            }
            
            propertyKeyMap = new List<PropertyKeyPair>();
            var accessorFieldInfos = node.GetBlackboardAccessorFieldInfos();
            foreach (var fieldInfo in accessorFieldInfos)
            {
                var pkp = new PropertyKeyPair();
                pkp.propertyName = fieldInfo.Name;
                if (fieldInfo.GetValue(node) is BlackboardAccessor accessor)
                {
                    pkp.key = accessor.Key;
                    //TODO make sure all supported values are properly passed (as string might return null)
                    pkp.overrideValue = accessor.OverrideValue as string;
                }
                else
                {
                    Debug.LogWarning($"Cant access BlackboardAccessor {fieldInfo.Name} on {type}");
                }

                propertyKeyMap.Add(pkp);
            }
        }
    }
}