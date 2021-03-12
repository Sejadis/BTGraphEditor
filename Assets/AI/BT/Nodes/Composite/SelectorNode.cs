using System;
using UnityEngine;

namespace AI.BT.Nodes.Composite
{
    /// <summary>
    /// Will execute all child nodes in order
    /// Succeeds if any child succeeds
    /// Fails if all children fail
    /// </summary>
    [Serializable]
    public class SelectorNode : CompositeNode
    {
        protected int currentNode = 0;

        public override ResultState Execute()
        {
            ResetChildrenState();

            for (var i = currentNode; i < children.Count; i++)
            {
                var state = children[i].Execute();
                switch (state)
                {
                    case ResultState.Running:
                        currentNode = i;
                        return CurrentState = ResultState.Running;
                    case ResultState.Success:
                        currentNode = 0;
                        return CurrentState = ResultState.Success;
                    case ResultState.Failure:
                        continue;
                }
            }

            //end reached, reset and return failure
            currentNode = 0;
            return CurrentState = ResultState.Failure;
        }
    }
}