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
            ResetChildrenState();

            foreach (var child in children)
            {
                var state = child.Execute();
                if (state == ResultState.Success)
                {
                    continue;
                }

                return CurrentState = state;
            }
            
            //all children succeeded or doesnt have children
            return CurrentState = ResultState.Success;
        }
    }
}