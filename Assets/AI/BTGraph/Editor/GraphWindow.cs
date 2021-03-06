using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AI.BT;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AI.BTGraph.Editor
{
    public class GraphWindow : EditorWindow
    {
        private List<Type> NodeTypes = new List<Type>();
        private List<RuntimeNodeData> NodeData = new List<RuntimeNodeData>();
        private BehaviourTreeGraphView graphView;

        [MenuItem("Graph/Show")]
        public static void ShowWindow()
        {
            var window = GetWindow<GraphWindow>();
            window.titleContent = new GUIContent("Behavior Graph");
            window.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            graphView = new BehaviourTreeGraphView()
            {
                name = "Behaviour Graph"
            };
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);


            SetupGraphView();
            CreateTestNodes();
        }
        
        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }
        
        private void SetupGraphView()
        {
            var map = CreateMiniMap();
            var grid = CreateGrid();
            var toolbar = CreateToolbar();
            // graphView.Add(map);
            graphView.Insert(0, grid);
            rootVisualElement.Add(toolbar);
        }

        private static MiniMap CreateMiniMap()
        {
            var map = new MiniMap();
            map.SetPosition(new Rect(10, 10, 150, 100));
            map.capabilities &= ~Capabilities.Movable;
            return map;
        }

        private GridBackground CreateGrid()
        {
            var grid = new GridBackground();
            grid.StretchToParentSize();
            return grid;
        }

        private Toolbar CreateToolbar()
        {
            var toolbar = new Toolbar();

            var button = new Button(() => Save())
            {
                text = "Save"
            };
            var treePicker = new ObjectField()
            {
                objectType = typeof(BehaviorTree)
            };
            treePicker.RegisterValueChangedCallback(OnTreeChanged);
            toolbar.Add(button);
            toolbar.Add(treePicker);
            return toolbar;
        }

        private void OnTreeChanged(ChangeEvent<Object> changeEvent)
        {
            // graphView.Clear();
            Debug.Log("Tree changed");
            var tree = changeEvent.newValue as BehaviorTree;
            if (tree == null)
            {
                return;
            }

            LoadGraphFromTree(tree);
        }

        
        private void LoadGraphFromTree(BehaviorTree tree)
        {
            var edges = new List<Edge>();
            var nodes = new List<BTGraphNode>();

            var runtimeData = tree.rootNode.CreateRuntimeNodeData();
            var parentNode = new BTGraphNode(runtimeData);
            nodes.Add(parentNode);
            foreach (var child in tree.rootNode.Children)
            {
                runtimeData = child.CreateRuntimeNodeData();
                var childNode = new BTGraphNode(runtimeData);
                var edge = new Edge()
                {
                    input = parentNode.InputPort,
                    output = childNode.OutputPort,
                };
                edges.Add(edge);
                nodes.Add(childNode);
            }

            foreach (var node in nodes)
            {
                graphView.AddElement(node);
            }

            foreach (var edge in edges)
            {
                graphView.AddElement(edge);
            }
        }

        private void Save()
        {
            var root = graphView.RootNode;
            if (root == null) return;
            var edges = graphView.edges.ToList();
            var bt = CreateInstance<BehaviorTree>();
            bt.rootNode = new RootNode();
            var currentNode = root;

            //TODO refactor to CreateChildren(BTGraphNode)
            foreach (var edge in edges)
            {
                if (edge.input.node == currentNode)
                {
                    var connectedNode = edge.output.node as BTGraphNode;
                    if (connectedNode == null)
                    {
                        Debug.LogError("Using node that is not derived from BTGraphNode: " + edge.output.node.name);
                    }
                    else
                    {
                        Assembly defaultAssembly = AppDomain.CurrentDomain.GetAssemblies()
                            .SingleOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp");
                        var instance = Activator.CreateInstance(connectedNode.RuntimeNodeData.type);
                        // var instance = defaultAssembly?.CreateInstance(connectedNode.RuntimeNodeData.type.Name);
                        var node = instance as BTNode;
                        bt.rootNode.AddChild(node);
                        // node.SetParent();
                        Debug.Log("Root is connected to:" + edge.output.node.name);
                    }
                }
            }

            //for debug logs
            bt.rootNode.Execute();
            
            AssetDatabase.CreateAsset(bt, "Assets/bTree.asset");
            EditorUtility.SetDirty(bt);
            AssetDatabase.SaveAssets();
        }
        
        private void CreateTestNodes()
        {
            if (GetClassData())
            {
                if (CreateRuntimeData())
                {
                    foreach (var data in NodeData)
                    {
                        var node = new BTGraphNode(data);
                        graphView.AddElement(node);
                    }
                }
            }
        }
        
        private bool CreateRuntimeData()
        {
            NodeData.Clear();

            foreach (var type in NodeTypes)
            {


                var data = new RuntimeNodeData(type);
                NodeData.Add(data);
            }

            if (NodeTypes == null || NodeTypes.Count == 0)
            {
                Debug.Log("Could not create node data");
                return false;
            }

            return true;
        }

        private bool GetClassData()
        {
            //TODO unnecessary?
            NodeTypes.Clear();

            NodeTypes = Utility.GetSubClasses(typeof(BTNode));

            if (NodeTypes == null || NodeTypes.Count == 0)
            {
                Debug.Log(
                    "Did not find any scripts that reference <b><color=white>BTNode</color></b>. Are they in a different assembly than Assembly-CSharp?");
                return false;
            }
            return true;
        }
        
    }
}