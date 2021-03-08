using System;

namespace AI.BT
{
    [Serializable]
    public abstract class CompositeNode : BTNode, IMayHaveMultipleChildren
    {
        protected int currentNode = 0;

        protected void ResetChildrenState()
        {
            foreach (var node in children)
            {
                node.CurrentState = ResultState.Inactive;
            }
        }
    }
}