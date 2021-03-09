using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AI.BT.Nodes;
using AI.BT.Serialization;
using AI.BTGraph;
using UnityEngine;

namespace AI.BT
{
    public static class Extensions
    {
        public static RuntimeNodeData CreateRuntimeNodeData(this BTNode node)
        {
            return new RuntimeNodeData(node);
        }

        public static BTNode CreateBTNode(this SerializedBTNode node)
        {
            var instance = Activator.CreateInstance(Type.GetType(node.type) ?? throw new InvalidOperationException());
            var btNode = instance as BTNode;
            foreach (var propertyKeyPair in node.propertyKeyMap)
            {
                var fieldInfo = btNode.GetType().GetField(propertyKeyPair.propertyName) ??
                                throw new InvalidOperationException();
                if (btNode.GetOrCreateBlackboardAccessor(fieldInfo) is BlackboardAccessor accessor)
                {
                    accessor.Key = propertyKeyPair.key;
                }
            }

            return btNode;
        }
    }
}