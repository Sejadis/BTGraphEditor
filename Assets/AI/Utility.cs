using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
        
        public static string SplitCamelCase( this string str )
        {
            return Regex.Replace( 
                Regex.Replace( 
                    str, 
                    @"(\P{Ll})(\P{Ll}\p{Ll})", 
                    "$1 $2" 
                ), 
                @"(\p{Ll})(\P{Ll})", 
                "$1 $2" 
            );
        }
    }
}