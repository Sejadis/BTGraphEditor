using System.Collections.Generic;
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