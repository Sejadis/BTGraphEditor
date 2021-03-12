using System;
using System.Collections.Generic;
using AI.BT.Nodes.Composite;
using AI.BT.Nodes.Decorator;
using UnityEngine;

namespace AI.BT.Nodes
{
    [Serializable]
    public sealed class RootNode : DecoratorNode
    {
        public override ResultState Execute()
        {
            //TODO remove and execute on max 1 child
            //(currently here for testing purposes)
            var state = child?.Execute();

            return CurrentState = state ?? ResultState.Success;
        }

        public override void SetParent(BTNode parent)
        {
            throw new InvalidOperationException("RootNode can not have a parent");
        }
    }
}