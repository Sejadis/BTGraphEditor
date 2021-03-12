using System;
using AI.BTGraph.Attribute;

namespace AI.BT.Nodes.Decorator
{
    [Serializable]
    public class RepeatNode : DecoratorNode
    {
        [Input] public int limit = 3;
        private int maxRepeats = -1;

        public override ResultState Execute()
        {
            if (limit <= 0)
            {
                //TODO handle 0 differently for unlimited?
                return ResultState.Failure;
            }
            for (int i = 0; i < limit; i++)
            {
                var state = children[0].Execute();
                if (state == ResultState.Success)
                {
                    continue;
                }
                return CurrentState = state;
            }

            return ResultState.Success;
        }
    }
}