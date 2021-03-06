using System;
using System.Collections.Generic;

namespace AI.BT
{
    [Serializable]
    public class SerializedBehaviorTree
    {
        public SerializedBTNode rootNode;
        public List<SerializedBTNode> nodes;

        public SerializedBehaviorTree(BehaviorTree behaviorTree)
        {
            nodes = new List<SerializedBTNode>();
            foreach (var node in behaviorTree.nodes)
            {
                nodes.Add(new SerializedBTNode(node));
            }

            rootNode = nodes.Find(node => node.guid.Equals(behaviorTree.rootNode.Guid.ToString()));
        }

        public BehaviorTree ToBehaviorTree()
        {
            var tree = new BehaviorTree();
            var nodeMap = new Dictionary<string, BTNode>();
            //populate nodes
            foreach (var node in nodes)
            {
                var btNode = node.CreateBTNode();
                tree.nodes.Add(btNode);
                nodeMap[node.guid] = btNode;
            }

            //all nodes created, make links
            foreach (var node in nodes)
            {
                if (node.parent != string.Empty)
                {
                    nodeMap[node.guid].SetParent(nodeMap[node.parent]);
                }

                foreach (var child in node.children)
                {
                    nodeMap[node.guid].AddChild(nodeMap[child]);
                }
            }

            tree.rootNode = nodeMap[rootNode.guid] as RootNode;

            return tree;
        }
    }
}