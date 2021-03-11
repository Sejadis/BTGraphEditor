using System;

namespace AI.BTGraph
{
    public class KeyTypePair
    {
        public string key;
        public Type type;
        public bool allowsManualInput;
        public string overrideValue;

        public KeyTypePair(string key, Type type, bool allowsManualInput)
        {
            this.allowsManualInput = allowsManualInput;
            this.key = key;
            this.type = type;
        }

        public string typeString => type.Name;
    }
}