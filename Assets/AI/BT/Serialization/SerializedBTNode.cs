using System;
using System.Collections.Generic;
using System.Linq;
using AI.BTGraph;
using UnityEngine;

namespace AI.BT.Serialization
{
    [Serializable]
    public class SerializedBTNode
    {
        public string type;
        public string guid;
        public string parent;
        public List<string> children;
        public Rect graphRect;

        public SerializedBTNode(BTNode node)
        {
            guid = node.Guid.ToString();
            type = node.GetType().ToString();
            parent = node.Parent?.Guid.ToString() ?? String.Empty;
            children = new List<string>();
            foreach (var child in node.Children)
            {
                children.Add(child.Guid.ToString());
            }
        }

        public SerializedBTNode(BTGraphNode node)
        {
            guid = node.Guid.ToString();
            type = node.RuntimeNodeData.type.ToString();
            graphRect = node.GetPosition();
            if (node.OutputPort?.connected ?? false)
            {
                var edge = node.OutputPort.connections.ToList()[0];
                parent = (edge.input.node as BTGraphNode).Guid.ToString();
            }

            children = new List<string>();
            if (node.InputPort.connected)
            {
                foreach (var edge in node.InputPort.connections.ToList())
                {
                    children.Add((edge.output.node as BTGraphNode).Guid.ToString());
                }
            }
        }
    }
}