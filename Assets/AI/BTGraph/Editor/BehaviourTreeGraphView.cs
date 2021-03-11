using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AI.BTGraph.Editor
{
    public class BehaviourTreeGraphView : GraphView
    {
        public BTGraphNode RootNode { get; }
        public GraphWindow EditorWindow { get; }

        public BehaviourTreeGraphView(GraphWindow editorWindow)
        {
            EditorWindow = editorWindow;
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Styles/GraphEditor.uss");
            styleSheets.Add(styleSheet);
            var node = CreateRootNode();
            RootNode = node;
            AddElement(node);

            graphViewChanged = OnGraphViewChanged;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewChange)
        {
            //TODO store new edges in nodes as children/parent to prevent the need to go through all edges all the time
            if (graphviewChange.edgesToCreate != null)
            {
                foreach (var edge in graphviewChange.edgesToCreate)
                {
                    if (edge.input.node is BTGraphNode inputNode && edge.output.node is BTGraphNode outputNode)
                    {
                        UpdateChildIndex(inputNode, true, outputNode);
                    }
                }
            }

            if (graphviewChange.movedElements == null) return graphviewChange;
            foreach (var movedElement in graphviewChange.movedElements)
            {
                if (movedElement is BTGraphNode graphNode)
                {
                    UpdateChildIndex(graphNode);
                }
            }

            return graphviewChange;
        }

        //TODO try to get a callback after edge creation so we can remove the new edge
        public void UpdateChildIndex(BTGraphNode graphNode, bool asParent = false, BTGraphNode newNode = null)
        {
            var nodeParent = asParent
                ? graphNode
                : edges.ToList().FirstOrDefault(edge => edge?.output == graphNode.OutputPort)?.input
                    .node as BTGraphNode;
            if (nodeParent == null) return;

            var children = edges.ToList().Where(edge => edge.input == nodeParent.InputPort)
                .Select(edge => edge.output.node as BTGraphNode).ToList();
            if (newNode != null)
            {
                children.Add(newNode);
            }

            children = children.OrderBy(child => child.GetPosition().position.y).ToList();
            for (var i = 0; i < children.Count; i++)
            {
                children[i].CurrentChildIndex = i;
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if ((port.portType == startPort.portType ||
                     port.portType == typeof(object) ||
                     startPort.portType == typeof(object)) &&
                    port.direction != startPort.direction &&
                    port != startPort &&
                    startPort.node != port.node)
                {
                    compatiblePorts.Add(port);
                }
            });
            return compatiblePorts; //ports.ToList();
        }

        public void ResetView()
        {
            foreach (var node in nodes.ToList())
            {
                if (node == RootNode)
                {
                    continue;
                }

                RemoveElement(node);
            }

            foreach (var edge in edges.ToList())
            {
                RemoveElement(edge);
            }
        }

        private BTGraphNode CreateRootNode()
        {
            var node = new BTGraphNode();
            node.capabilities &= ~Capabilities.Deletable;
            var rect = node.GetPosition();
            var windowSize = EditorWindow.position.size;
            rect.position = new Vector2(windowSize.x * 0.9f,
                windowSize.y * 0.5f);
            rect.size = new Vector2(150, 150);
            node.SetPosition(rect);
            return node;
        }
    }
}