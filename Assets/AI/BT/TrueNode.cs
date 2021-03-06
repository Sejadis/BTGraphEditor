using System;
using UnityEngine;

namespace AI.BT
{
    [Serializable]
    public class TrueNode : BTNode
    {
        // public override Type Type => typeof(TrueNode);

        public override ResultState Execute()
        {
            var result = ResultState.Failure;
            Debug.Log("TrueNode: " +  result);

            return result;
        }
    }
}
