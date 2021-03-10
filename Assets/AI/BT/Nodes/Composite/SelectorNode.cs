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
                        break;
                    case ResultState.Success:
                        currentNode = 0;
                        return CurrentState = ResultState.Success;
                        break;
                    case ResultState.Failure:
                        continue;
                        break;
                }
            }

            //end reached, reset and return failure
            currentNode = 0;
            return CurrentState = ResultState.Failure;
        }
    }
}