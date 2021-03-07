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
                        currentNode++;
                        if (currentNode == children.Count)
                        {
                            //all children done
                            currentNode = 0;
                            return CurrentState = ResultState.Success;
                        }
                        else
                        {
                            return CurrentState = ResultState.Running;
                        }
                    case ResultState.Failure:
                        currentNode = 0;
                        return CurrentState = ResultState.Failure;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //we went through all children and all succeeded
            currentNode = 0;
            Debug.LogWarning("SequenceNode: all children done. " + ResultState.Success +
                             " We should never reach this point in code");

            return CurrentState = ResultState.Success;
        }
    }
}