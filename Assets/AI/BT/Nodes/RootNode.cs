using System;
using UnityEngine;

namespace AI.BT.Nodes
{
    [Serializable]
    public sealed class RootNode : BTNode
    {
        public override ResultState Execute()
        {
            //TODO remove and execute on max 1 child
            //(currently here for testing purposes)
            var resultState = ResultState.Success;
            foreach (var child in children)
            {
                var state = child.Execute();
                switch (state)
                {
                    case ResultState.Failure:
                        resultState = state;
                        break;
                    case ResultState.Running:
                        resultState = state;
                        break;
                }
            }
            
            return resultState;
        }

        public override void SetParent(BTNode parent)
        {
            throw new InvalidOperationException("RootNode can not have a parent");
        }
    }
}