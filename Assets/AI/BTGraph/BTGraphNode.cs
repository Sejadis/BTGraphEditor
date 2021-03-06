using System;
using System.Collections.Generic;
using AI.BT;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AI.BTGraph
{
    public class BTGraphNode : Node
    {
        public string GUID;
        public RuntimeNodeData RuntimeNodeData { get; }
        private Port inputPort;
        private Port outputPort;

        public Port InputPort => inputPort;

        public Port OutputPort => outputPort;

        public BTGraphNode(RuntimeNodeData runtimeNodeData)
        {
            name = runtimeNodeData.type.Name;
            RuntimeNodeData = runtimeNodeData;
            GUID = Guid.NewGuid().ToString();
            title = RuntimeNodeData.type.Name;
            outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
                typeof(BTNode.ResultState));
            outputPort.portName = "OUT";
            outputContainer.Add(outputPort);
            inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input,
                RuntimeNodeData.allowMultipleChildren ? Port.Capacity.Multi : Port.Capacity.Single,
                typeof(BTNode.ResultState));
            inputPort.portName = "IN";

            inputContainer.Add(inputPort);

            CreatePorts(RuntimeNodeData.inputTypes, inputContainer, Direction.Input);
            CreatePorts(RuntimeNodeData.outputTypes, outputContainer, Direction.Output, Port.Capacity.Multi);

            RefreshPorts();
            RefreshExpandedState();
        }

        private void CreatePorts(Dictionary<string, Type> types, VisualElement visualElement, Direction direction,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            foreach (var kvp in types)
            {
                var port = InstantiatePort(Orientation.Horizontal, direction, capacity, kvp.Value);
                port.portName = kvp.Key;
                visualElement.Add(port);
            }
        }

        /// <summary>
        /// Creates a RootNode
        /// </summary>
        public BTGraphNode()
        {
            name = "ROOT";
            title = name;
            RuntimeNodeData = new RuntimeNodeData(typeof(RootNode));
            var port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                typeof(BTNode.ResultState));
            port.portName = "IN";
            inputContainer.Add(port);

            RefreshPorts();
            RefreshExpandedState();
        }
    }
}