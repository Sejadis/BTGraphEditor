using System;
using UnityEngine;

namespace AI.BT.Nodes.Decorator
{
    [Serializable]
    public class FalseNode : DecoratorNode
    {
        // public override Type Type => typeof(TrueNode);

        public override ResultState Execute()
        {
            CurrentState = ResultState.Failure;
            Debug.Log("FalseNode: " +  CurrentState);

            return CurrentState;
        }
    }
}