using System;
using UnityEngine;

namespace AI.BT.Nodes.Decorator
{
    [Serializable]
    public class InvertNode : DecoratorNode
    {
        public override ResultState Execute()
        {
            if (children.Count != 1)
            {
                Debug.LogWarning($"InvertNode has {children.Count} nodes. Should have 1");
                return ResultState.Failure;
            }

            var resultState = children[0].Execute();
            switch (resultState)
            {
                case ResultState.Running:
                    break;
                case ResultState.Success:
                    resultState = ResultState.Failure;
                    break;
                case ResultState.Failure:
                    resultState = ResultState.Success;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.Log("InvertNode: -> " + resultState);
            return resultState;
        }
    }
}