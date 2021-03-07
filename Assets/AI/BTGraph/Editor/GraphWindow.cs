using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
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

            var saveButton = new Button(() => SaveLoadHandler.Save(graphView))
            {
                text = "Save"
            };

            var testNodeButton = new Button(CreateTestNodes)
            {
                text = "Create TestNode"
            };
            var treePicker = new ObjectField()
            {
                objectType = typeof(BehaviorTree)
            };
            treePicker.RegisterValueChangedCallback(OnTreeChanged);
            toolbar.Add(saveButton);
            toolbar.Add(testNodeButton);
            toolbar.Add(treePicker);
            return toolbar;
        }

        private void OnTreeChanged(ChangeEvent<Object> changeEvent)
        {
            var tree = changeEvent.newValue as BehaviorTree;
            if (tree == null)
            {
                return;
            }
            graphView.ResetView();
            SaveLoadHandler.LoadGraphFromTree(tree, graphView);
        }

        private void CreateTestNodes()
        {
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
    }
}