﻿using System;
using System.Collections.Generic;
using AI.BT;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace AI.BTGraph.Editor
{
    public class BehaviourTreeGraphView : GraphView
    {
        public BTGraphNode RootNode { get; }
        public BehaviourTreeGraphView()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var node = CreateRootNode();
            RootNode = node;
            AddElement(node);

        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                
                if (port.portType == startPort.portType && 
                    port.direction != startPort.direction &&
                    port != startPort && 
                    startPort.node != port.node)
                {
                    compatiblePorts.Add(port);
                }
            });
            return compatiblePorts; //ports.ToList();
        }

        private BTGraphNode CreateRootNode()
        {
            var node = new BTGraphNode();
            node.capabilities &= ~Capabilities.Deletable;
            return node;
        }
    }
}
