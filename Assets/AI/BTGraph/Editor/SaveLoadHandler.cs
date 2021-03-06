using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using AI.BT;
using AI.BTGraph.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AI.BTGraph
{
    public static class SaveLoadHandler
    {
        public static void LoadGraphFromFile(BehaviourTreeGraphView graphView)
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/json.bt");
            var serializedBehaviorTree = JsonUtility.FromJson<SerializedBehaviorTree>(json);
            var behaviorTree = serializedBehaviorTree.ToBehaviorTree();
            LoadGraphFromTree(behaviorTree,graphView);

        }
        public static void LoadGraphFromTree(BehaviorTree tree, BehaviourTreeGraphView graphView)
        {
            var nodeMap = new Dictionary<BTNode, BTGraphNode>();
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
                var edge = new Edge()
                {
                    input = inNode.InputPort,
                    output = outNode.OutputPort,
                };
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
                    //skip, we set it to the views root manually
                    continue;
                }

                var runtimeData = node.CreateRuntimeNodeData();
                var graphNode = new BTGraphNode(runtimeData);
                result.Add(graphNode);
                nodeMap[node] = graphNode;
            }

            return result;
        }

        public static bool Save(BehaviourTreeGraphView graphView)
        {
            var graphRoot = graphView.RootNode;
            if (graphRoot == null) return false;

            var graphNodes = graphView.nodes.ToList().Cast<BTGraphNode>();
            var nodeMap = new Dictionary<BTGraphNode, BTNode>();

            CreateAndMapNodes(graphNodes, ref nodeMap);

            var behaviorTree = new BehaviorTree();//ScriptableObject.CreateInstance<BehaviorTree>();
            foreach (var key in nodeMap.Keys)
            {
                behaviorTree.nodes.Add(nodeMap[key]);
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

            //for debug logs
            behaviorTree.rootNode.Execute();

            // AssetDatabase.CreateAsset(behaviorTree, "Assets/bTree.asset");
            // EditorUtility.SetDirty(behaviorTree);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
            
            
            // var seri = new XmlSerializer(typeof(BehaviorTree), Utility.GetSubClasses(typeof(BTNode)).ToArray());
            // var stream = new FileStream(Application.persistentDataPath + "/test.bt",FileMode.Create);
            // seri.Serialize(stream, behaviorTree);

            string json = JsonUtility.ToJson(new SerializedBehaviorTree(behaviorTree),false);
            File.WriteAllText(Application.persistentDataPath + "/json.bt", json);
            return true;
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