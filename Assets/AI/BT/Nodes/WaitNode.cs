using System;
using AI.BTGraph.Attribute;

namespace AI.BT.Nodes
{
    [Serializable]
    public class WaitNode : BTNode
    {
        [Input] public float waitTime = 3;
        [Output] public int transform;
        [Input] public BlackboardAccessor<float> WaitTime = new BlackboardAccessor<float>("waitTime");
        private DateTime startTime = DateTime.MinValue;

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
            CurrentState = ResultState.Running;
            if (startTime == DateTime.MinValue)
            {
                startTime = DateTime.Now;
            }

            float seconds;
            if (!WaitTime.TryGetValue(out seconds))
            {
                seconds = waitTime;
            }
            if (DateTime.Now - startTime > TimeSpan.FromSeconds((double) seconds))
            {
                CurrentState = ResultState.Success;
                //reset
                startTime = DateTime.MinValue;
            }

            // Debug.Log("WaitNode: " + CurrentState);

            return CurrentState;
        }
    }
}