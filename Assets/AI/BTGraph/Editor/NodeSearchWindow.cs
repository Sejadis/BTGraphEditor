using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using AI.BT;
using AI.BT.Nodes;
using AI.BTGraph;
using AI.BTGraph.Editor;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private BehaviourTreeGraphView _graphView;
    private EditorWindow _window;
    public GraphWindow GraphWindow { get; set; }

    public void Init(BehaviourTreeGraphView graphView, EditorWindow window)
    {
        _graphView = graphView;
        _window = window;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var nodeTypes = Utility.GetSubClasses(typeof(BTNode));
        nodeTypes.Sort((x, y) => string.Compare(x.Name, y.Name));
        var searchTree = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create"), 0),
        };

        searchTree.Add(new SearchTreeGroupEntry(new GUIContent("Composite"), 1));
        foreach (var type in nodeTypes)
        {
            if (type.IsSubclassOf(typeof(CompositeNode)))
            {
                searchTree.Add(
                    new SearchTreeEntry(new GUIContent(type.Name.SplitCamelCase()))
                    {
                        userData = type,
                        level = 2
                    });
            }
        }

        searchTree.Add(new SearchTreeGroupEntry(new GUIContent("Decorator"), 1));
        foreach (var type in nodeTypes)
        {
            if (type.IsSubclassOf(typeof(DecoratorNode)))
            {
                searchTree.Add(
                    new SearchTreeEntry(new GUIContent(type.Name.SplitCamelCase()))
                    {
                        userData = type,
                        level = 2
                    });
            }
        }


        //all
        foreach (var type in nodeTypes)
        {
            if (type == typeof(RootNode))
            {
                //RooTNode can not be crated manually
                continue;
            }

            searchTree.Add(
                new SearchTreeEntry(new GUIContent(type.Name.SplitCamelCase()))
                {
                    userData = type,
                    level = 1
                });
        }

        return searchTree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var mousePos = _window.rootVisualElement.ChangeCoordinatesTo(
            _window.rootVisualElement.parent,
            context.screenMousePosition - _window.position.position);
        var localMousePos = _graphView.WorldToLocal(mousePos);
        var node = new BTGraphNode(SearchTreeEntry.userData as Type);
        var rect = node.GetPosition();
        rect.position = localMousePos;
        node.SetPosition(rect);
        _graphView.EditorWindow.OnBlackboardValuesChanged += node.OnBlackboardValuesChanged;
        _graphView.AddElement(node);
        return true;
    }
}