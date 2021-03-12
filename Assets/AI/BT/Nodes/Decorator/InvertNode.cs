using System;
using UnityEngine;

namespace AI.BT.Nodes.Decorator
{
    [Serializable]
    public class InvertNode : DecoratorNode
    {
        public override ResultState Execute()
        {
            if (child == null)
            {
                //TODO or success?
                return CurrentState = ResultState.Failure;
            }

            var resultState = child.Execute();
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
            return CurrentState = resultState;
        }
    }
}