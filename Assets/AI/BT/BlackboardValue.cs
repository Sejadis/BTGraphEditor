namespace AI.BT
{
    public class BlackboardValue
    {
        public object value;

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
            IsSet = true;
        }

        public void UnSet()
        {
            value = null;
            IsSet = false;
        }
    }
}