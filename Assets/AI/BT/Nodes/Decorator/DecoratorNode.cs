using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BT.Nodes
{
    [Serializable]
    public abstract class DecoratorNode : BTNode
    {
        public BTNode child;

        public override void Sort(Dictionary<Guid, Rect> nodePositions)
        {
            child?.Sort(nodePositions);
        }
    }
}