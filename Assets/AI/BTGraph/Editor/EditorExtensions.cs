using System;
using AI.BT;
using AI.BT.Nodes;

namespace AI.BTGraph.Editor
{
    public static class EditorExtensions
    {
        public static BTNode CreateBTNode(this BTGraphNode node, BehaviorTree behaviorTree)
        {
            var instance = Activator.CreateInstance(node.RuntimeNodeData.type);
            var btNode = instance as BTNode;
            btNode.Initialize(behaviorTree.Blackboard,node.BlackboardConnections);
            return btNode;
        }
    }
}