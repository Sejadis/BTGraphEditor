using System.Collections.Generic;
using UnityEngine;

namespace AI.BT.Sensors
{
    public class KeySetter : BTSensor
    {
        public List<Transform> objects;
        public List<BTRunner> behaviorRunners;

        private new void Start()
        {
            foreach (var agent in behaviorRunners)
            {
                WriteToBlackBoard(objects.ToArray(),agent.Blackboard);
            }
        }
    }
}