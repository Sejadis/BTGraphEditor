using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BT.Sensors
{
    public abstract class BTSensor : MonoBehaviour
    {
        protected Blackboard blackboard;
        public string Key;

        private void Awake()
        {
            blackboard = GetComponent<IBlackboardProvider>().Blackboard;
        }

        /// <summary>
        /// Writes a value to the blackboard if not null, otherwise unset the key
        /// </summary>
        /// <param name="value"></param>
        protected void WriteToBlackBoard(object value)
        {
            if (string.IsNullOrEmpty(Key))
            {
                Debug.LogError("Trying to set blackboard value with empty key", this);
                return;
            }
            if (value != null)
            {
                blackboard?.SetValue(Key, value);
            }
            else
            {
                blackboard?.UnSet(Key);
            }
        }
    }
}