namespace AI.BT.Nodes
{
    public class RunningOrSuccessNode : DecoratorNode
    {
        public override ResultState Execute()
        {
            if (child == null)
            {
                //TODO or success?
                return CurrentState = ResultState.Failure;
            }

            var resultState = child.Execute();
            if (resultState == ResultState.Failure)
            {
                resultState = ResultState.Success;
            }
            return CurrentState = resultState;
        }
    }
}