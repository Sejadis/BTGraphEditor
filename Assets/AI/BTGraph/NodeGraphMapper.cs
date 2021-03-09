using System;
using System.Collections.Generic;
using AI.BT;
using AI.BT.Nodes;

namespace AI.BTGraph
{
    public static class NodeGraphMapper<T> where T: BTNode
    {
        public static readonly Dictionary<Type, Action<T>> Mapping = new Dictionary<Type, Action<T>>();
    }
}