using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BT.Sensors
{
    public abstract class BTSensor : MonoBehaviour
    {
        protected Blackboard blackboard;
        public BTRunner BlackboardProvider;
        public string Key;

        protected void Start()
        {
            if (BlackboardProvider != null)
            {
                blackboard = BlackboardProvider.Blackboard;
            }
            else
            {
                blackboard = GetComponent<IBlackboardProvider>().Blackboard;
            }
        }

        /// <summary>
        /// Writes a value to the blackboard if not null, otherwise unset the key
        /// </summary>
        /// <param name="value"></param>
        protected void WriteToBlackBoard(object value, Blackboard _blackboard = null)
        {
            if (string.IsNullOrEmpty(Key))
            {
                Debug.LogError("Trying to set blackboard value with empty key", this);
                return;
            }

            var blackboardToUse = _blackboard != null ? _blackboard : blackboard;
            if (value != null)
            {
                blackboardToUse?.SetValue(Key, value);
            }
            else
            {
                blackboardToUse?.UnSet(Key);
            }
        }
    }
}