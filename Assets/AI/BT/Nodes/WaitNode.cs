using System;
using AI.BTGraph;
using UnityEngine;

namespace AI.BT
{
    [Serializable]
    public class WaitNode : BTNode
    {
        [Input] public float waitTime;
        [Output] public int transform;

        private float startTime = -1;
        // public override Type Type => typeof(WaitNode);

        public WaitNode()
        {
        }

        public WaitNode(float waitTime)
        {
            if (waitTime == 0)
            {
                throw new ArgumentException("waitTime can not be 0");
            }

            this.waitTime = waitTime;
        }

        public override ResultState Execute()
        {
            var result = ResultState.Running;
            if (startTime == -1)
            {
                startTime = Time.time;
            }

            if (Time.time > startTime + waitTime)
            {
                result = ResultState.Success;
            }

            Debug.Log("WaitNode: " + result);

            return result;
        }
    }
}