using System;
using System.Diagnostics;

namespace AI.BT
{
    [Serializable]
    [DebuggerDisplay("Property: {propertyName} Key: {key}")]
    public class PropertyKeyPair
    {
        public string propertyName;
        public string key;
    }
}