using System;
using System.Collections.Generic;
using AI.BT.Nodes;
using AI.BT.Serialization;
using UnityEngine;

namespace AI.BT
{
    // [CreateAssetMenu(fileName = "Assets/bt", menuName = "Create/BT")]
    public class BehaviorTree : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public RootNode rootNode;
        [NonSerialized] public List<BTNode> nodes = new List<BTNode>();
        [NonSerialized] public Dictionary<Guid, Rect> nodePositions = new Dictionary<Guid, Rect>();
        [SerializeField] private SerializedBehaviorTree serializedBehaviorTree;

        public void SetFromSerializedTree(SerializedBehaviorTree serializedBehaviorTree)
        {
            var nodeMap = new Dictionary<string, BTNode>();
            //populate nodes
            foreach (var node in this.serializedBehaviorTree.nodes)
            {
                var btNode = node.CreateBTNode();
                nodes.Add(btNode);
                nodeMap[node.guid] = btNode;
                nodePositions[btNode.Guid] = node.graphRect;
            }

            //all nodes created, make links
            foreach (var node in this.serializedBehaviorTree.nodes)
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

            rootNode = nodeMap[serializedBehaviorTree.rootNode.guid] as RootNode;
        }

        public void OnBeforeSerialize()
        {
            serializedBehaviorTree = new SerializedBehaviorTree(this);
        }

        public void OnAfterDeserialize()
        {
            nodePositions.Clear();
            nodes.Clear();
            SetFromSerializedTree(serializedBehaviorTree);
            //free memory
            //TODO necessary?
            serializedBehaviorTree = null;
        }
    }
}