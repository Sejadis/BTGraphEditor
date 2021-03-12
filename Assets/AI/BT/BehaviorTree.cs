using System;
using System.Collections.Generic;
using System.Linq;
using AI.BT.Nodes;
using AI.BT.Serialization;
using UnityEditor;
using UnityEngine;

namespace AI.BT
{
    // [CreateAssetMenu(fileName = "Assets/bt", menuName = "Create/BT")]
    public class BehaviorTree : ScriptableObject, ISerializationCallbackReceiver
    {
        public Blackboard Blackboard = new Blackboard();
        [NonSerialized] private bool isInitialized;
        [NonSerialized] public RootNode rootNode;
        [NonSerialized] public List<BTNode> nodes = new List<BTNode>();
        [NonSerialized] public Dictionary<Guid, Rect> nodePositions = new Dictionary<Guid, Rect>();
        [SerializeField] private SerializedBehaviorTree serializedBehaviorTree;


        public void Run()
        {
            if (!isInitialized)
            {
                Initialize();
            }

            rootNode.Execute();
        }
        
        public void Clear()
        {
            Blackboard = new Blackboard();
            isInitialized = false;
            rootNode = null;
            nodes.Clear();
            nodePositions.Clear();
            serializedBehaviorTree = null;
        }

        private void Initialize()
        {
            foreach (var node in nodes)
            {
                node.SetBlackboardForAllAccessors(Blackboard);
            }
        }

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

                foreach (var propertyKeyPair in node.propertyKeyMap)
                {
                    Blackboard.RegisterKey(propertyKeyPair.key);
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

        public void SortNodes()
        {
            rootNode.Sort(nodePositions);
        }

        public BehaviorTree Clone()
        {
            var clone = CreateInstance<BehaviorTree>();
            clone.serializedBehaviorTree = new SerializedBehaviorTree(this);
            clone.OnAfterDeserialize();
            return clone;
        }
    }
}