using System;

namespace AI.BT
{
    public class BlackboardValue
    {
        public object value;
        public Type type;

        public BlackboardValue(object value) => SetValue(value);

        public BlackboardValue() { }

        public bool IsSet { get; private set; }

        public T GetValue<T>()
        {
            return (T) value;
        }

        public void SetValue(object value)
        {
            this.value = value;
            type = value.GetType();
            IsSet = true;
        }

        public void UnSet()
        {
            value = null;
            IsSet = false;
        }
    }
}