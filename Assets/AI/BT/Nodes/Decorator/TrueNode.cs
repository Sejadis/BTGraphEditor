using System;
using UnityEngine;

namespace AI.BT.Nodes.Decorator
{
    [Serializable]
    public class TrueNode : DecoratorNode
    {
        public override ResultState Execute()
        {
            return CurrentState = ResultState.Success;
        }
    }
}