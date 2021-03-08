using System;
using System.Collections.Generic;
using AI.BT;
using AI.BT.Nodes;
using AI.BT.Nodes.Decorator;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Blackboard = UnityEditor.Experimental.GraphView.Blackboard;
using Object = UnityEngine.Object;

namespace AI.BTGraph.Editor
{
    public class GraphWindow : EditorWindow
    {
        private List<Type> NodeTypes = new List<Type>();
        private List<RuntimeNodeData> NodeData = new List<RuntimeNodeData>();
        private BehaviourTreeGraphView graphView;
        private BehaviorTree selectedBehaviorTree;
        private Dictionary<BTNode, BTGraphNode> nodeMap;

        public BehaviorTree SelectedBehaviorTree
        {
            get => selectedBehaviorTree;
            set
            {
                selectedBehaviorTree = value;
                LoadGraph(selectedBehaviorTree);
            }
        }

        [MenuItem("Graph/Show")]
        public static GraphWindow ShowWindow()
        {
            var window = GetWindow<GraphWindow>();
            window.titleContent = new GUIContent("Behavior Graph");
            window.minSize = new Vector2(800, 600);
            return window;
        }

        [OnOpenAsset(1)]
        public static bool OpenBehaviourTree(int instanceID, int line)
        {
            var tree = EditorUtility.InstanceIDToObject(instanceID) as BehaviorTree;
            if (tree == null)
            {
                //not a behavior tree
                return false;
            }

            var window = ShowWindow();
            window.SelectedBehaviorTree = tree;
            return true;
        }

        private void OnEnable()
        {
            graphView = new BehaviourTreeGraphView(this)
            {
                name = "Behaviour Graph"
            };

            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
            SetupGraphView();
            // CreateTestNodes();
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
            var blackboard = CreateBlackBoard();
            graphView.Add(map);
            graphView.Add(blackboard);
            graphView.Insert(0, grid);
            rootVisualElement.Add(toolbar);
        }

        private Blackboard CreateBlackBoard()
        {
            var blackboard = new Blackboard(graphView);
            blackboard.SetPosition(new Rect(10, 30, 150, 300));
            blackboard.addItemRequested = AddBlackboardValue;
            return blackboard;
        }

        private MiniMap CreateMiniMap()
        {
            var map = new MiniMap();
            var windowSize = position.size;
            var mapSize = new Vector2(150, 100);

            var mapPosition = new Vector2(
                //base pos is upper right corner, offset by window size, offset by margin
                windowSize.x - mapSize.x - 25,
                25);

            var mapRect = map.GetPosition();
            mapRect.size = mapSize;
            mapRect.position = mapPosition;
            map.SetPosition(mapRect);
            map.anchored = true;
            map.OnResized();
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

            var saveButton = new ToolbarButton(() =>
            {
                selectedBehaviorTree = SaveLoadHandler.Save(graphView);
                LoadGraph(selectedBehaviorTree);
            })
            {
                text = "Save"
            };

            var testNodeButton = new ToolbarButton(CreateTestNodes)
            {
                text = "Create TestNode"
            };

            var runTreeButton = new ToolbarButton(RunTree)
            {
                text = "Run"
            };

            var treePicker = new ObjectField()
            {
                objectType = typeof(BehaviorTree)
            };

            treePicker.RegisterValueChangedCallback(OnTreeChanged);
            toolbar.Add(saveButton);
            toolbar.Add(testNodeButton);
            toolbar.Add(treePicker);
            toolbar.Add(runTreeButton);
            return toolbar;
        }

        private void RunTree()
        {
            selectedBehaviorTree?.Run();
        }

        private void OnTreeChanged(ChangeEvent<Object> changeEvent)
        {
            var tree = changeEvent.newValue as BehaviorTree;
            if (tree == null)
            {
                return;
            }

            SelectedBehaviorTree = tree;
        }

        private void LoadGraph(BehaviorTree behaviorTree)
        {
            graphView.ResetView();
            SaveLoadHandler.LoadGraphFromTree(behaviorTree, graphView, out nodeMap);
            foreach (var node in nodeMap.Keys)
            {
                node.ResetEvent();
                node.OnStateChanged += OnNodeStateChanged;
            }
        }

        private void OnNodeStateChanged(ResultState newstate, BTNode source)
        {
            if (nodeMap.TryGetValue(source, out var graphNode))
            {
                graphNode.SetRuntimeState(newstate);
            }
        }

        private void CreateTestNodes()
        {
            var node1 = new BTGraphNode(typeof(TrueNode));
            var node2 = new BTGraphNode(typeof(WaitNode));
            node1.extensionContainer.Add(node2);
            node2.StretchToParentSize();
            node1.capabilities |= Capabilities.Resizable;
            node1.RefreshExpandedState();
            graphView.AddElement(node1);
            return;
            //test
            if (GetClassData())
            {
                if (CreateRuntimeData())
                {
                    foreach (var data in NodeData)
                    {
                        if (data.type == typeof(RootNode))
                        {
                            continue;
                        }

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


        private void AddBlackboardValue(Blackboard blackboard)
        {
            var field = new BlackboardField(null, "test", "string");
            var typeLabel = field.Q<Label>("typelabel");
            field.Remove(typeLabel);
            // var dropdown = 
            // dropdown.AppendAction("Increase Count", a => { m_ActiveCount++; UpdateText(); }, a => DropdownMenuAction.Status.Normal);
            // dropdown.AppendAction("Decrease Count", a => { m_ActiveCount--; UpdateText(); }, a => DropdownMenuAction.Status.Normal);
            // dropdown.styles.color = Color.blue;
            // dropdown.styles.backgroundColor = Color.red;
            // dropdown.styles.paddingTop = 4;
            // dropdown.styles.paddingBottom = 4;
            // dropdown.styles.paddingLeft = 4;
            // dropdown.styles.paddingRight = 4;
            blackboard.Add(field);
        }
    }
}