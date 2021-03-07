using System;
using System.Collections.Generic;
using System.Linq;
using AI.BT;
using AI.BT.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AI.BTGraph
{
    public class BTGraphNode : Node
    {
        public Guid Guid;
        public RuntimeNodeData RuntimeNodeData { get; }
        private Port inputPort;
        private Port outputPort;

        public Port InputPort => inputPort;

        public Port OutputPort => outputPort;

        public BTGraphNode(Type type) : this(new RuntimeNodeData(type))
        {
        }

        public BTGraphNode(RuntimeNodeData runtimeNodeData)
        {
            name = runtimeNodeData.type.Name;
            RuntimeNodeData = runtimeNodeData;
            Guid = Guid.NewGuid();
            title = RuntimeNodeData.type.Name;
            outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
                typeof(ResultState));
            outputPort.portName = "OUT";
            outputContainer.Add(outputPort);
            if (!runtimeNodeData.hasNoChildren)
            {
                inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input,
                    RuntimeNodeData.allowMultipleChildren ? Port.Capacity.Multi : Port.Capacity.Single,
                    typeof(ResultState));
                inputPort.portName = "IN";
                inputContainer.Add(inputPort);
            }


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
            Guid = Guid.NewGuid();
            name = "ROOT";
            title = name;
            RuntimeNodeData = new RuntimeNodeData(typeof(RootNode));
            inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                typeof(ResultState));
            inputPort.portName = "IN";
            inputContainer.Add(inputPort);

            RefreshPorts();
            RefreshExpandedState();
        }

        public void SetRuntimeState(ResultState newstate)
        {
            var children = Children();
            foreach (var element in children.Where(child => child.name.Equals("node-border")))
            {
                Color c = new Color(0, 0, 0, 0);
                switch (newstate)
                {
                    case ResultState.Running:
                        c = Color.yellow;
                        break;
                    case ResultState.Success:
                        c = Color.green;
                        break;
                    case ResultState.Failure:
                        c = Color.red;
                        break;
                }

                element.style.backgroundColor = c;
            }
        }
    }
}