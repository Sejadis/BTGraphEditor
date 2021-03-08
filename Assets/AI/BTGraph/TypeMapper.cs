using System.Collections.Generic;

namespace AI.BTGraph
{
    public static class TypeMapper
    {
        public static Dictionary<string, string> typeMap = new Dictionary<string, string>()
        {
            {"Transform", "UnityEngine.Transform, UnityEngine"},
            {"Transform[]", "UnityEngine.Transform[], UnityEngine"},
            {"Int", "System.Int32"},
            {"Int[]", "System.Int32[]"},
            {"Float", "System.Single"},
            {"Float[]", "System.Single[]"},
            {"String", "System.String"},
            {"String[]", "System.String[]"},
        };
    }
}