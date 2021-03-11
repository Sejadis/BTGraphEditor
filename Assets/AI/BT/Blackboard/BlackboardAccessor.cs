using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AI.BT
{
    public class BlackboardAccessor
    {
        public static object CreateFromFieldInfo(FieldInfo fieldInfo)
        {
            var args = fieldInfo.FieldType.GetGenericArguments();
            var genericType = typeof(BlackboardAccessor<>).MakeGenericType(args);
            var field = Activator.CreateInstance(genericType);
            return field;
        }

        public Blackboard Blackboard { get; set; }
        protected string key;

        public BlackboardAccessor()
        {
        }

        public BlackboardAccessor(string key)
        {
            Key = key;
        }

        public string Key
        {
            get => key;
            set
            {
                key = value;
                if (!string.IsNullOrEmpty(key))
                {
                    SetKey();
                }
            }
        }

        public object OverrideValue { get; set; }

        protected void SetKey()
        {
            Blackboard?.RegisterKey(key);
        }
    }

    public class BlackboardAccessor<T> : BlackboardAccessor
    {
        public BlackboardAccessor() : base()
        {
        }

        public BlackboardAccessor(string waittime) : base(waittime)
        {
        }

        public bool TryGetValue(out T value)
        {
            value = default;
            try
            {
                value = (T) OverrideValue;
            }
            catch (Exception e)
            {
                //TODO handle all supported types
                if (typeof(T) == typeof(float))
                {
                    Single.TryParse(OverrideValue as string, out var result);
                    value = (T) (object) result;
                }
            }

            return value != null || Blackboard.TryGetValue(Key, out value);
        }


        public void SetValue(Object value)
        {
            Blackboard.SetValue(Key, value);
        }

        public bool IsSet()
        {
            //TODO
            //OverrideValue != null
            return !string.IsNullOrEmpty(OverrideValue as string) || Blackboard.IsSet(Key);
        }
    }
}