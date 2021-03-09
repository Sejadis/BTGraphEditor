using System;

namespace AI.BTGraph
{
    public class KeyTypePair
    {
        public string key;
        public Type type;

        public KeyTypePair(string key, Type type)
        {
            this.key = key;
            this.type = type;
        }
        public string typeString => type.Name;
    }
}