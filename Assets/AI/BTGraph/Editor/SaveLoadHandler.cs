using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using AI.BT;
using AI.BT.Nodes;
using AI.BT.Serialization;
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
            LoadGraphFromTree(behaviorTree, graphView,out _);
        }

        public static void LoadGraphFromTree(BehaviorTree tree, BehaviourTreeGraphView graphView, out Dictionary<BTNode, BTGraphNode> nodeMap)
        {
            nodeMap = new Dictionary<BTNode, BTGraphNode>();
            var edges = new List<Edge>();
            var nodes = new List<BTGraphNode>();
            //keep the root node
            nodeMap[tree.rootNode] = graphView.RootNode;

            nodes = LoadNodes(tree, nodeMap);
            edges = LoadEdges(tree, nodeMap);

            foreach (var edge in edges)
            {
                graphView.AddElement(edge);
            }

            foreach (var node in nodes)
            {
                graphView.AddElement(node);
                node.RefreshPorts();
                node.RefreshExpandedState();
            }
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
            Dictionary<BTNode, BTGraphNode> nodeMap)
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
                var graphNode = new BTGraphNode(runtimeData);
                graphNode.SetPosition(behaviorTree.nodePositions[node.Guid]);
                result.Add(graphNode);
                nodeMap[node] = graphNode;
            }

            return result;
        }

        public static BehaviorTree Save(BehaviourTreeGraphView graphView )
        {
            //TODO decide between versions
            
            //V2
            // var nodes = graphView.nodes.ToList().Cast<BTGraphNode>();
            // var serializedTree = new SerializedBehaviorTree();
            // var root = new SerializedBTNode(graphView.RootNode);
            // serializedTree.rootNode = root;
            // serializedTree.nodes.Add(root);
            // foreach (var node in nodes)
            // {
            //     if (node.GUID.Equals(root.guid))
            //     {
            //         continue;
            //     }
            //     
            //     serializedTree.nodes.Add(new SerializedBTNode(node));
            // }
            //
            // string jsonTree = JsonUtility.ToJson(serializedTree, false);
            // File.WriteAllText(Application.persistentDataPath + "/json.bt", jsonTree);
            // return true;

            
            //V1            
            var graphRoot = graphView.RootNode;
            if (graphRoot == null) return null;

            var graphNodes = graphView.nodes.ToList().Cast<BTGraphNode>();
            var nodeMap = new Dictionary<BTGraphNode, BTNode>();

            CreateAndMapNodes(graphNodes, ref nodeMap);

            var behaviorTree = ScriptableObject.CreateInstance<BehaviorTree>();//new BehaviorTree(); 
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

                child.SetParent(parent);
                parent.AddChild(child);
            }

            AssetDatabase.CreateAsset(behaviorTree, "Assets/bTree.asset");
            EditorUtility.SetDirty(behaviorTree);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
  
            return behaviorTree;
        }

        private static void CreateAndMapNodes(IEnumerable<BTGraphNode> graphNodes,
            ref Dictionary<BTGraphNode, BTNode> nodeMap)
        {
            foreach (var node in graphNodes)
            {
                if (!nodeMap.ContainsKey(node))
                {
                    var btNode = node.CreateBTNode();
                    nodeMap[node] = btNode;
                }
            }
        }
    }
}