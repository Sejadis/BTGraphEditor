using System;
using System.Collections.Generic;
using System.Reflection;
using AI.BTGraph;

namespace AI.BT
{
    public static class Extensions
    {
        public static RuntimeNodeData CreateRuntimeNodeData(this BTNode node)
        {
            var type = node.GetType();
            return new RuntimeNodeData(type);
        }
    }
}