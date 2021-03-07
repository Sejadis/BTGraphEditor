using System;
using AI.BTGraph.Attribute;

namespace AI.BT.Nodes.Decorator
{
    [Serializable]
    public class RepeatNode : DecoratorNode
    {
        [Input] public int repeats = 3;
        private int maxRepeats = -1;
        public override ResultState Execute()
        {
            if (children.Count != 1)
            {
                return CurrentState = ResultState.Failure;
            }

            if (maxRepeats == -1)
            {
                //save the value
                maxRepeats = repeats;
            }
            children[0].Execute();
            if (repeats == -1 || --repeats > 0)
            {
                return CurrentState = ResultState.Running;
            }

            repeats = maxRepeats;
            return CurrentState = ResultState.Success;
        }
    }
}