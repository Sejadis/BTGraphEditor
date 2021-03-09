using System;
using UnityEngine;

namespace AI.BT.Nodes.Composite
{
    /// <summary>
    /// Will execute all child nodes in order
    /// Fails if any child fails
    /// Succeeds if all children succeed
    /// </summary>
    [Serializable]
    public class SequenceNode : CompositeNode
    {
        private int currentNode = 0;

        public override ResultState Execute()
        {
            ResetChildrenState();

            for (var i = currentNode; i < children.Count; i++)
            {
                var child = children[i];
                var state = child.Execute();
                switch (state)
                {
                    case ResultState.Running:
                        currentNode = i;
                        return CurrentState = ResultState.Running;
                        break;
                    case ResultState.Success:
                        continue;
                        break;
                    case ResultState.Failure:
                        return CurrentState = ResultState.Failure;
                        break;
                }
            }

            //end reached, reset and return success
            currentNode = 0;
            return CurrentState = ResultState.Success;
        }
    }
}