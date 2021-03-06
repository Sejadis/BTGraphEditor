using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BT
{
    [Serializable]
    public sealed class RootNode : BTNode
    {
        public new static bool AllowMultipleChildren => false;

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
                    
                }
            }

            Debug.Log("RootNode: " +  resultState);
            return resultState;
        }

        public override void SetParent(BTNode parent)
        {
            throw new InvalidOperationException("RootNode can not have a parent");
        }
        
        // public override (Type, ) SetMapping()
        // {
        //     return(typeof(RootNode),() => return new RootNode());
        // }
    }
}