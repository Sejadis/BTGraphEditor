using System;

namespace AI.BT.Nodes
{
    [Serializable]
    public class SuccessNode : DecoratorNode
    {
        public override ResultState Execute()
        {
            return CurrentState = ResultState.Success;
        }
    }
}