using System;
using AI.BTGraph.Attribute;

namespace AI.BT.Nodes
{
    [Serializable]
    public class WaitNode : BTNode
    {
        [Input] public float waitTime = 3;
        [Output] public int transform;

        private DateTime startTime = DateTime.MinValue;
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
            CurrentState = ResultState.Running;
            if (startTime == DateTime.MinValue)
            {
                startTime = DateTime.Now;
            }

            if (DateTime.Now - startTime > new TimeSpan(0, 0, 3))
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