using System;
using System.Collections.Generic;
using AI.BT;

namespace Subtegral.DialogueSystem.Editor
{
    public static class NodeGraphMapper<T> where T: BTNode
    {
        public static readonly Dictionary<Type, Action<T>> Mapping = new Dictionary<Type, Action<T>>();
    }
}