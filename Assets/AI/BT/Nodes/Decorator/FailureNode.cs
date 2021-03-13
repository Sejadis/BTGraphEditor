using System;

namespace AI.BT.Nodes
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