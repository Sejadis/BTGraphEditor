using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AI.BT;
using AI.BTGraph;

namespace AI
{
    public static class Utility
    {
        public static List<Type> GetSubClasses(Type type)
        {
            Assembly defaultAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp");

            // NodeTypes = defaultAssembly?.GetTypes().Where(t => t.IsClass).ToList();
            return defaultAssembly?.GetTypes().Where(x => x.IsClass && x.IsSubclassOf(typeof(BTNode))).ToList();
        }
    }
}