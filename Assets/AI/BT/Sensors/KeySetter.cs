using System;
using System.Collections.Generic;
using AI.BT.Sensors;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AI.BT
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