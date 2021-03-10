using System;
using UnityEngine;

namespace AI.BT.Nodes.Decorator
{
    [Serializable]
    public class FalseNode : DecoratorNode
    {
        public override ResultState Execute()
        {
            return CurrentState = ResultState.Failure;
        }
    }
}