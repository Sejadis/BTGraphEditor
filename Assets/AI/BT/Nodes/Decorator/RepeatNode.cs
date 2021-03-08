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
            if (limit < 0)
            {
                //TODO also catch 0 as unlimited?
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
            if (children.Count != 1)
            {
                return CurrentState = ResultState.Failure;
            }

            if (maxRepeats == -1)
            {
                //save the value
                maxRepeats = limit;
            }

            children[0].Execute();
            if (limit == -1 || --limit > 0)
            {
                return CurrentState = ResultState.Running;
            }

            limit = maxRepeats;
            return CurrentState = ResultState.Success;
        }
    }
}