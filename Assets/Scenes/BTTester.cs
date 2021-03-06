using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AI.BT;
using UnityEditor;
using UnityEngine;

public class BTTester : MonoBehaviour
{

    [MenuItem("Graph/Run Tree %#e")]
    private static void RunTree()
    {
        // ClearLog();
        // var tree = AssetDatabase.LoadAssetAtPath<BehaviorTree>("Assets/bTree.asset");
        // tree.rootNode.Execute();
    }
    
    
    private static void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
