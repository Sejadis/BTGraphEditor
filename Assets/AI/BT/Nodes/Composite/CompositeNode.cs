using System;

namespace AI.BT
{
    [Serializable]
    public abstract class CompositeNode : BTNode, IMayHaveMultipleChildren
    {
        //TODO if exited with State running, continue at running node
        protected void ResetChildrenState()
        {
            foreach (var node in children)
            {
                node.CurrentState = ResultState.Inactive;
            }
        }
    }
}