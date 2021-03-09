using AI.BTGraph.Attribute;

namespace AI.BT.Nodes
{
    public class IsSetNode : BTNode
    {
        [Input] public BlackboardAccessor<object> keyToCheck;

        public override ResultState Execute()
        {
            return CurrentState = keyToCheck.IsSet() ? ResultState.Success : ResultState.Failure;
        }
    }
}