using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AI.BT;
using AI.BT.Nodes;
using AI.BTGraph.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AI.BTGraph
{
    public static class SaveLoadHandler
    {
        public static void LoadGraphFromFile(BehaviourTreeGraphView graphView)
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/json.bt");
            var serializedBehaviorTree = JsonUtility.FromJson<SerializedBehaviorTree>(json);
            var behaviorTree = ScriptableObject.CreateInstance<BehaviorTree>();
            behaviorTree.SetFromSerializedTree(serializedBehaviorTree);
            graphView.RootNode.SetPosition(serializedBehaviorTree.rootNode.graphRect);
            LoadGraphFromTree(behaviorTree, graphView, out _);
        }

        public static void LoadGraphFromTree(BehaviorTree tree, BehaviourTreeGraphView graphView,
            out Dictionary<BTNode, BTGraphNode> nodeMap)
        {
            nodeMap = new Dictionary<BTNode, BTGraphNode>();
            var edges = new List<Edge>();
            var nodes = new List<BTGraphNode>();

            HashSet<(Type, string)> blackboardEntries = GetBlackboardEntries(tree.nodes);
            graphView.EditorWindow.ClearBlackboard();
            Dictionary<string, BlackboardField> blackboardMap = new Dictionary<string, BlackboardField>();
            foreach (var tuple in blackboardEntries)
            {
                blackboardMap[tuple.Item2] = graphView.EditorWindow.AddBlackboardValue(tuple.Item2, tuple.Item1);
            }

            //keep the root node
            nodeMap[tree.rootNode] = graphView.RootNode;

            nodes = LoadNodes(tree, nodeMap, graphView.EditorWindow, blackboardMap);
            edges = LoadEdges(tree, nodeMap);

            foreach (var edge in edges)
            {
                graphView.AddElement(edge);
            }

            foreach (var node in nodes)
            {
                graphView.AddElement(node);
                graphView.UpdateChildIndex(node);
                node.RefreshPorts();
                node.RefreshExpandedState();
            }
        }

        private static HashSet<(Type, string)> GetBlackboardEntries(List<BTNode> nodes)
        {
            var result = new HashSet<(Type, string)>();
            foreach (var node in nodes)
            {
                var fieldInfos = node.GetBlackboardAccessorFieldInfos();
                foreach (var fieldInfo in fieldInfos)
                {
                    var key = (node.GetOrCreateBlackboardAccessor(fieldInfo) as BlackboardAccessor)?.Key;
                    var type = fieldInfo.FieldType.GetGenericArguments()[0];
                    //TODO replace <none> with something not magic stringy
                    if (!string.IsNullOrEmpty(key) && !key.Equals("none") && !type.Name.Equals("Object"))
                    {
                        result.Add((type, key));
                    }
                }
            }

            return result;
        }

        private static List<Edge> LoadEdges(BehaviorTree behaviorTree, Dictionary<BTNode, BTGraphNode> nodeMap)
        {
            var result = new List<Edge>();
            foreach (var node in behaviorTree.nodes)
            {
                if (node.Parent == null)
                {
                    //no parent, so no edge to add
                    continue;
                }

                //the childs output port is conected to the parents input port
                var outNode = nodeMap[node];
                var inNode = nodeMap[node.Parent];
                var edge = outNode.OutputPort.ConnectTo(inNode.InputPort);

                result.Add(edge);
            }

            return result;
        }

        private static List<BTGraphNode> LoadNodes(BehaviorTree behaviorTree,
            Dictionary<BTNode, BTGraphNode> nodeMap, GraphWindow graphWindow,
            Dictionary<string, BlackboardField> blackboardFields)
        {
            var result = new List<BTGraphNode>();
            foreach (var node in behaviorTree.nodes)
            {
                if (node == behaviorTree.rootNode)
                {
                    //only set saved position,skip the rest, we set it to the views root manually
                    nodeMap[node].SetPosition(behaviorTree.nodePositions[node.Guid]);
                    continue;
                }

                var runtimeData = node.CreateRuntimeNodeData();
                var graphNode = new BTGraphNode(runtimeData, blackboardFields);
                graphWindow.OnBlackboardValuesChanged += graphNode.OnBlackboardValuesChanged;
                graphNode.SetPosition(behaviorTree.nodePositions[node.Guid]);
                result.Add(graphNode);
                nodeMap[node] = graphNode;
            }

            return result;
        }

        public static BehaviorTree Save(BehaviourTreeGraphView graphView, BehaviorTree behaviorTree)
        {
            var graphRoot = graphView.RootNode;
            if (graphRoot == null) return null;
            var graphNodes = graphView.nodes.ToList().Cast<BTGraphNode>();
            var nodeMap = new Dictionary<BTGraphNode, BTNode>();
            var createNew = false;
            if (behaviorTree == null)
            {
                createNew = true;
                behaviorTree = ScriptableObject.CreateInstance<BehaviorTree>(); //new BehaviorTree(); 
            }
            else
            {
                behaviorTree.Clear();
            }

            CreateAndMapNodes(graphNodes, ref nodeMap, behaviorTree);

            foreach (var node in nodeMap.Keys)
            {
                behaviorTree.nodes.Add(nodeMap[node]);
                behaviorTree.nodePositions[nodeMap[node].Guid] = node.GetPosition();
            }

            if (nodeMap.TryGetValue(graphView.RootNode, out var rootNode))
            {
                behaviorTree.rootNode = rootNode as RootNode;
            }
            else
            {
                Debug.LogError("RootNode does not exist");
            }

            var edges = graphView.edges.ToList();
            foreach (var edge in edges)
            {
                var child = nodeMap[edge.output.node as BTGraphNode];
                var parent = nodeMap[edge.input.node as BTGraphNode];
                switch (parent)
                {
                    case CompositeNode compositeNode:
                    {
                        compositeNode.AddChild(child);
                        break;
                    }
                    case DecoratorNode decoratorNode:
                    {
                        decoratorNode.child = child;
                        break;
                    }
                    default:
                        throw new InvalidOperationException(
                            "Trying to add a child to a node that cant contain children");
                }
                child.SetParent(parent);
            }

            behaviorTree.SortNodes();

            if (createNew)
            {
                AssetDatabase.CreateAsset(behaviorTree, "Assets/bTree.asset");
            }

            EditorUtility.SetDirty(behaviorTree);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return behaviorTree;
        }

        private static void CreateAndMapNodes(IEnumerable<BTGraphNode> graphNodes,
            ref Dictionary<BTGraphNode, BTNode> nodeMap, BehaviorTree behaviorTree)
        {
            //we are sorting here so when we are loading them they are in proper order to show childindex (cant sort on creation as they dont have a position yet)
            graphNodes = graphNodes.OrderBy(node => node.GetPosition().position.y);
            foreach (var node in graphNodes)
            {
                if (!nodeMap.ContainsKey(node))
                {
                    var btNode = node.CreateBTNode(behaviorTree);
                    nodeMap[node] = btNode;
                }
            }
        }
    }
}