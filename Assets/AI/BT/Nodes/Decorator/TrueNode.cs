using System;
using UnityEngine;

namespace AI.BT.Nodes.Decorator
{
    [Serializable]
    public class TrueNode : BTNode
    {
        // public override Type Type => typeof(TrueNode);

        public override ResultState Execute()
        {
            CurrentState = ResultState.Success;
            Debug.Log("TrueNode: " +  CurrentState);

            return CurrentState;
        }
    }
}
