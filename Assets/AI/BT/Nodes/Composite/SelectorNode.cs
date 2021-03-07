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
        public override ResultState Execute()
        {
            if (currentNode == 0)
            {
                ResetChildrenState();
            }
            if (currentNode >= children.Count || currentNode < 0)
            {
                //something wrong
            }
            else
            {
                var state = children[currentNode].Execute();
                switch (state)
                {
                    case ResultState.Running:
                        return CurrentState = ResultState.Running;
                    case ResultState.Success:
                        currentNode = 0;
                        return CurrentState = ResultState.Success;
                    case ResultState.Failure:
                        currentNode++;
                        if (currentNode == children.Count)
                        {
                            //all done
                            currentNode = 0;
                            return CurrentState = ResultState.Failure;
                        }
                        else
                        {
                            return CurrentState = ResultState.Running;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //we went through all children but all failed
            currentNode = 0;
            Debug.LogWarning("SelectorNode: all children done. " + ResultState.Failure +
                             " We should never reach this point in code");

            return CurrentState = ResultState.Failure;
        }
    }
}