using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AI.BT.Serialization;
using AI.BTGraph;
using UnityEngine;

namespace AI.BT
{
    public static class Extensions
    {
        public static RuntimeNodeData CreateRuntimeNodeData(this BTNode node)
        {
            var type = node.GetType();
            return new RuntimeNodeData(type);
        }

        public static BTNode CreateBTNode(this BTGraphNode node)
        {
            var instance = Activator.CreateInstance(node.RuntimeNodeData.type);
            var btNode = instance as BTNode;
            return btNode;
        }

        public static BTNode CreateBTNode(this SerializedBTNode node)
        {
            var instance = Activator.CreateInstance(Type.GetType(node.type) ?? throw new InvalidOperationException());

            var btNode = instance as BTNode;
            return btNode;
        }
    }
}