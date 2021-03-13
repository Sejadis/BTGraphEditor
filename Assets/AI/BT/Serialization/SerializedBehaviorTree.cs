using System;
using System.Collections.Generic;

namespace AI.BT
{
    [Serializable]
    public class SerializedBehaviorTree
    {
        public SerializedBTNode rootNode;
        public List<SerializedBTNode> nodes;

        public SerializedBehaviorTree(BehaviorTree behaviorTree) : this()
        {
            foreach (var node in behaviorTree.nodes)
            {
                var newNode = new SerializedBTNode(node) {graphRect = behaviorTree.nodePositions[node.Guid]};
                nodes.Add(newNode);
            }

            rootNode = nodes.Find(node => node.guid.Equals(behaviorTree.rootNode.Guid.ToString()));
        }

        public SerializedBehaviorTree()
        {
            nodes = new List<SerializedBTNode>();
        }
    }
}