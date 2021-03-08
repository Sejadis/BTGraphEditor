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

        private void Initialize()
        {
            foreach (var node in nodes)
            {
                var fields = node.GetType().GetFields();
                foreach (var fieldInfo in fields)
                {
                    if (!fieldInfo.FieldType.IsGenericType) continue;

                    if (fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(BlackboardAccessor<>))
                    {
                        var field = fieldInfo.GetValue(node);
                        if (field == null)
                        {
                            //TODO probably move creation to when setting the key
                            var args = fieldInfo.FieldType.GetGenericArguments();
                            var genericType = typeof(BlackboardAccessor<>).MakeGenericType(args);
                            field = Activator.CreateInstance(genericType);
                            fieldInfo.SetValue(node,field);
                        }
                        var property = field.GetType().GetProperty("Blackboard");
                        if (property != null)
                        {
                            property.SetValue(field, Blackboard);
                        }
                    }
                }
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