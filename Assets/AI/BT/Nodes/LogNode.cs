using System;
using UnityEngine;

namespace AI.BT.Nodes
{
    [Serializable]
    public class LogNode :BTNode
    {
        public override ResultState Execute()
        {
            Debug.Log("I am a log node");
            return CurrentState = ResultState.Success;
        }
    }
}