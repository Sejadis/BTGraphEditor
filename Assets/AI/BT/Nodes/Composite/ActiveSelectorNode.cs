using System;

namespace AI.BT.Nodes.Composite
{
    public class ActiveSelectorNode : CompositeNode
    {
        public override ResultState Execute()
        {
            ResetChildrenState();

            foreach (var child in children)
            {
                var state = child.Execute();
                if (state == ResultState.Failure)
                {
                    continue;
                }

                return CurrentState = state;
            }
            
            //all children failed or doesnt have children
            return CurrentState = ResultState.Failure;
        }
    }
}