using System;
using UnityEngine;

namespace AI.BT.Nodes.Decorator
{
    public class RunningOrSuccessNode : DecoratorNode
    {
        public override ResultState Execute()
        {
            if (children.Count != 1)
            {
                Debug.LogWarning($"RunningOrSuccessNode has {children.Count} nodes. Should have 1");
                return ResultState.Failure;
            }

            var resultState = children[0].Execute();
            if (resultState == ResultState.Failure)
            {
                resultState = ResultState.Success;
            }
            return CurrentState = resultState;
        }
    }
}