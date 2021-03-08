using UnityEngine;

namespace AI.BT
{
    public class BlackboardAccessor<T>
    {
        public Blackboard Blackboard { get; set; }
        private string key;

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

        public bool TryGetValue(out T value)
        {
            return Blackboard.TryGetValue(Key, out value);
        }

        private void SetKey()
        {
            Blackboard?.RegisterKey(key);
        }

        public void SetValue(Object value)
        {
            Blackboard.SetValue(Key, value);
        }

        public bool IsSet()
        {
            return Blackboard.IsSet(Key);
        }
    }
}