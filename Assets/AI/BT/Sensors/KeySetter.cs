using System.Collections.Generic;
using UnityEngine;

namespace AI.BT.Sensors
{
    public class KeySetter : BTSensor
    {
        public List<Transform> objects;

        private void Start()
        {
            WriteToBlackBoard(objects.ToArray());
        }
    }
}