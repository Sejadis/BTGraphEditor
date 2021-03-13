using System;
using AI.BTGraph.Attribute;

namespace AI.BT.Nodes
{
    [Serializable]
    public class RepeatNode : DecoratorNode
    {
        [Input, Option] public BlackboardAccessor<int> Repeats;

        public override ResultState Execute()
        {
            if (!Repeats.IsSet() || !Repeats.TryGetValue(out var limit))
            {
                return CurrentState = ResultState.Failure;
            }

            if (limit <= 0)
            {
                //TODO handle 0 differently for unlimited?
                return ResultState.Failure;
            }

            for (var i = 0; i < limit; i++)
            {
                var state = child.Execute();
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