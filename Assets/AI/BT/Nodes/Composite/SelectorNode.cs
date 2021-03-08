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
