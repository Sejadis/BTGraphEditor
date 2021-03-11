using System;
using AI.BTGraph.Attribute;

namespace AI.BT.Nodes
{
    [Serializable]
    public class WaitNode : BTNode
    {
        [Input,Option] public BlackboardAccessor<float> WaitTime;
        private DateTime startTime = DateTime.MinValue;

        public WaitNode()
        {
        }
        //
        // public WaitNode(float waitTime)
        // {
        //     if (waitTime == 0)
        //     {
        //         throw new ArgumentException("waitTime can not be 0");
        //     }
        //
        //     this.waitTime = waitTime;
        // }

        public override ResultState Execute()
        {
            if (!WaitTime.IsSet())
            {
                //TODO decide on proper handling
                return CurrentState = ResultState.Failure;
            }
            CurrentState = ResultState.Running;
            if (startTime == DateTime.MinValue)
            {
                startTime = DateTime.Now;
            }

            float seconds;
            if (!WaitTime.TryGetValue(out seconds))
            {
                //TODO necessary?
                seconds = 0f;
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