using System;
using System.Collections.Generic;
using System.Linq;
using AI.BT;
using AI.BT.Nodes;
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
        public delegate void BlackboardValuesChanged(BlackboardField values);

        public event BlackboardValuesChanged OnBlackboardValuesChanged;

        private List<Type> NodeTypes = new List<Type>();
        private List<RuntimeNodeData> NodeData = new List<RuntimeNodeData>();
        private BehaviourTreeGraphView graphView;
        private BehaviorTree selectedBehaviorTree;

        private Dictionary<BTNode, BTGraphNode> nodeMap;

        //string represents the blackboard key
        private Dictionary<string, BlackboardField> blackboardFields = new Dictionary<string, BlackboardField>();
        private Blackboard blackboard;
        private NodeSearchWindow _searchWindow;
        private Label currentAssetLabel;

        private BehaviorTree SelectedBehaviorTree
        {
            get => selectedBehaviorTree;
            set
            {
                selectedBehaviorTree = value;
                if (selectedBehaviorTree != null)
                {
                    LoadGraph(selectedBehaviorTree);
                }
                else
                {
                    graphView.ResetView();
                    blackboard.Clear();
                }

                currentAssetLabel.text = value?.name ?? string.Empty;
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

        private void SelectionChanged()
        {
            var treeProvider = Selection.activeGameObject?.GetComponent<IBehaviorTreeProvider>();
            if (treeProvider != null && treeProvider.BehaviorTree != null)
            {
                SelectedBehaviorTree = treeProvider.BehaviorTree;
            }
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
            // window.TreeInstanceID = instanceID;
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
            if (selectedBehaviorTree != null)
            {
                LoadGraph(selectedBehaviorTree);
            }
            Selection.selectionChanged += SelectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= SelectionChanged;
            rootVisualElement.Remove(graphView);
        }

        private void SetupGraphView()
        {
            var map = CreateMiniMap();
            var grid = CreateGrid();
            var toolbar = CreateToolbar();
            var blackboard = CreateBlackBoard();
            AddSearchWindow();
            graphView.Add(map);
            graphView.Add(blackboard);
            graphView.Insert(0, grid);
            rootVisualElement.Add(toolbar);
        }

        private void AddSearchWindow()
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Init(graphView, this);
            graphView.nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition, 0, 0), _searchWindow);
        }

        private Blackboard CreateBlackBoard()
        {
            blackboard = new Blackboard(graphView);
            blackboard.SetPosition(new Rect(10, 30, 150, 300));
            blackboard.addItemRequested = AddBlackboardValue;
            blackboard.editTextRequested = EditBlackboardValue;
            blackboard.showInMiniMap = true;
            return blackboard;
        }

        private void EditBlackboardValue(Blackboard blackboard, VisualElement element, string newValue)
        {
            if (!blackboardFields.ContainsKey(newValue) && element is BlackboardField blackboardField)
            {
                blackboardFields.Remove(blackboardField.text);
                blackboardFields.Add(newValue, blackboardField);
                blackboardField.text = newValue;
                InvokeValuesChanged(blackboardField);
            }
        }

        private void InvokeValuesChanged(BlackboardField blackboardField)
        {
            OnBlackboardValuesChanged?.Invoke(blackboardField);
        }

        private bool DoesBlackboardFieldNameExist(string name)
        {
            return blackboard.Children().Where(ve =>
            {
                if (ve is BlackboardField field)
                {
                    return field.text.Equals(name);
                }

                return false;
            }).Any();
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
                SelectedBehaviorTree = SaveLoadHandler.Save(graphView, selectedBehaviorTree);
                LoadGraph(selectedBehaviorTree);
            })
            {
                text = "Save"
            };

            var newButton = new ToolbarButton(() => { SelectedBehaviorTree = null; })
            {
                text = "New"
            };


            var runTreeButton = new ToolbarButton(RunTree)
            {
                text = "Run"
            };

            var treePicker = new ObjectField()
            {
                objectType = typeof(BehaviorTree)
            };

            currentAssetLabel = new Label(selectedBehaviorTree?.name);

            treePicker.RegisterValueChangedCallback(OnTreeChanged);
            toolbar.Add(saveButton);
            toolbar.Add(newButton);
            toolbar.Add(treePicker);
            toolbar.Add(runTreeButton);
            toolbar.Add(currentAssetLabel);
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


        private void AddBlackboardValue(Blackboard blackboard)
        {
            AddBlackboardValue("", null, blackboard);
        }

        public BlackboardField AddBlackboardValue(string key, Type type, Blackboard blackboard = null)
        {
            var field = CreateBlackboardField(key, type);
            if (blackboard == null)
            {
                this.blackboard.Add(field);
            }
            else
            {
                blackboard.Add(field);
            }

            return field;
        }

        private BlackboardField CreateBlackboardField(string key = "", Type type = null)
        {
            var name = !string.IsNullOrEmpty(key) ? key : "NewValue";
            while (DoesBlackboardFieldNameExist(name))
            {
                ///TODO replace with incrementing value
                name += "(1)";
            }

            var field = new BlackboardField(null, name, type?.Name ?? "string");
            var typeLabel = field.Q<Label>("typelabel");
            if (typeLabel != null)
            {
                field.Remove(typeLabel);
            }

            var dropdown = new ToolbarMenu();

            // dropdown.menu. = field;
            void Action(DropdownMenuAction a)
            {
                if (a.userData is BlackboardField blackboardField)
                {
                    blackboardField.typeText = a.name;
                    InvokeValuesChanged(blackboardField);
                }
            }

            foreach (var mapKey in TypeMapper.typeMap.Keys)
            {
                dropdown.menu.AppendAction(mapKey, Action, (_) => DropdownMenuAction.Status.Normal, field);

            }

            field.Add(dropdown);
            blackboardFields[name] = field;
            return field;
        }

        public void ClearBlackboard()
        {
            blackboard.Clear();
        }
    }
}