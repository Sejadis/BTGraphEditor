using System;
using UnityEngine;

namespace AI.BT.Nodes.Decorator
{
    [Serializable]
    public class FailureNode : DecoratorNode
    {
        public override ResultState Execute()
        {
            return CurrentState = ResultState.Failure;
        }
    }
}