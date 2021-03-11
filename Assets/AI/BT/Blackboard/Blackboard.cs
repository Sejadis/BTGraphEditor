using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.BT
{
    public class Blackboard : ISerializationCallbackReceiver
    {
        private Dictionary<string, BlackboardValue> values = new Dictionary<string, BlackboardValue>();
        private List<string> keys;

        public void SetValue(string key, object value)
        {
            if (value == null)
            {
                UnSet(key);
            }
            else
            {
                if (values.TryGetValue(key, out var bbValue))
                {
                    bbValue.SetValue(value);
                }
                else
                {
                    values[key] = new BlackboardValue(value);
                }
            }
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (values.TryGetValue(key, out var bbValue))
            {
                value = bbValue.GetValue<T>();
                return bbValue.IsSet;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public void UnSet(string key)
        {
            if (values.TryGetValue(key, out var bbValue))
            {
                bbValue.UnSet();
            }
        }

        public void RegisterKey(string key)
        {
            if (!values.TryGetValue(key, out _))
            {
                values[key] = new BlackboardValue();
            }
        }

        public bool IsSet(string key)
        {
            return values.TryGetValue(key, out var value) && value.IsSet;
        }

        public Type GetType(string key)
        {
            values.TryGetValue(key, out var value);
            return value?.type;
        }

        public void OnBeforeSerialize()
        {
            keys = values.Keys.ToList();
        }

        public void OnAfterDeserialize()
        {
            values = new Dictionary<string, BlackboardValue>();
            foreach (var key in keys)
            {
                RegisterKey(key);
            }

            keys = null;
        }
    }
}