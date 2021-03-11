using System;
using System.Collections.Generic;
using System.Linq;
using AI.BT;
using AI.BT.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AI.BTGraph
{
    public class BTGraphNode : Node
    {
        public Guid Guid;
        public RuntimeNodeData RuntimeNodeData { get; }
        public Port InputPort { get; }
        public Port OutputPort { get; }

        public List<PropertyKeyPair> BlackboardConnections
        {
            get
            {
                return portBlackboardValues.Select(kvp =>
                    new PropertyKeyPair()
                    {
                        propertyName = kvp.Key.portName, //field name matches port name
                        key = kvp.Value.Label.text, //label text matches key
                        //TODO make sure all supported values are properly passed (as string might return null)
                        overrideValue =
                            kvp.Value
                                    .overrideValue as
                                string //override value if source is manual input instead of blackboard
                    }
                ).ToList();
            }
        }

        private readonly List<Port> ports = new List<Port>();

        private readonly Dictionary<Port, PortBlackboardValue>
            portBlackboardValues = new Dictionary<Port, PortBlackboardValue>();

        private readonly Label indexLabel;
        private int currentChildIndex;

        public int CurrentChildIndex
        {
            get => currentChildIndex;
            set
            {
                if (currentChildIndex != value)
                {
                    currentChildIndex = value;
                    UpdateIndex();
                }
            }
        }

        private void UpdateIndex()
        {
            indexLabel.text = currentChildIndex.ToString();
        }

        public BTGraphNode(Type type) : this(new RuntimeNodeData(type))
        {
        }

        public BTGraphNode(RuntimeNodeData runtimeNodeData)
        {
            name = runtimeNodeData.type.Name;
            RuntimeNodeData = runtimeNodeData;
            Guid = Guid.NewGuid();
            title = RuntimeNodeData.type.Name;
            OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
                typeof(ResultState));
            OutputPort.portName = "OUT";
            ports.Add(OutputPort);
            outputContainer.Add(OutputPort);
            if (!runtimeNodeData.hasNoChildren)
            {
                InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input,
                    RuntimeNodeData.allowMultipleChildren ? Port.Capacity.Multi : Port.Capacity.Single,
                    typeof(ResultState));
                InputPort.portName = "IN";
                ports.Add(InputPort);
                inputContainer.Add(InputPort);
            }

            CreatePorts(RuntimeNodeData.inputTypes, inputContainer, Direction.Input);
            CreatePorts(RuntimeNodeData.outputTypes, outputContainer, Direction.Output, Port.Capacity.Multi);

            indexLabel = new Label("0");
            titleContainer.Add(indexLabel);

            RefreshPorts();
            RefreshExpandedState();
        }

        public BTGraphNode(RuntimeNodeData runtimeNodeData, Dictionary<string, BlackboardField> blackboardFieldMap) :
            this(runtimeNodeData)
        {
            foreach (var key in blackboardFieldMap.Keys)
            {
                //add all matching fields
                OnBlackboardValuesChanged(blackboardFieldMap[key]);
            }

            foreach (var port in portBlackboardValues.Keys)
            {
                var portBlackboardValue = portBlackboardValues[port];

                if (!runtimeNodeData.inputTypes.TryGetValue(port.portName, out var key) &&
                    !runtimeNodeData.outputTypes.TryGetValue(port.portName, out key))
                {
                    continue;
                }

                if (blackboardFieldMap.TryGetValue(key.key, out var blackboardField))
                {
                    portBlackboardValue.SetCurrentFieldAndUpdateVisuals(blackboardField);
                }
            }
        }

        private void CreatePorts(Dictionary<string, KeyTypePair> types, VisualElement visualElement,
            Direction direction,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            foreach (var kvp in types)
            {
                //if we have a generic type use it as the port type
                //TODO change to only work with BlackboardAccessor
                var genericTypes = kvp.Value.type.GetGenericArguments();
                var type = genericTypes.Length > 0 ? genericTypes[0] : kvp.Value.type;
                var port = InstantiatePort(Orientation.Horizontal, direction, capacity, type);
                port.portName = kvp.Key;
                port.name = port.portName;
                port.style.justifyContent = Justify.SpaceBetween;
                var portBlackboardValue = new PortBlackboardValue(kvp.Value.allowsManualInput, kvp.Value.overrideValue);
                port.contentContainer.Add(portBlackboardValue);

                portBlackboardValues[port] = portBlackboardValue;

                ports.Add(port);
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
            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                typeof(ResultState));
            InputPort.portName = "IN";
            inputContainer.Add(InputPort);

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


        public void OnBlackboardValuesChanged(BlackboardField blackboardField)
        {
            foreach (var port in ports)
            {
                string typeString;
                if (!TypeMapper.typeMap.TryGetValue(blackboardField.typeText, out typeString))
                {
                    //type is not mapped
                    typeString = blackboardField.name;
                }

                var valueType = Type.GetType(typeString);
                if (port.portType == valueType || port.portType == typeof(object))
                {
                    if (portBlackboardValues.TryGetValue(port, out var value))
                    {
                        value.AddFieldReference(blackboardField);
                    }
                }
                else
                {
                    if (portBlackboardValues.TryGetValue(port, out var value))
                    {
                        value.RemoveFieldReference(blackboardField);
                    }
                }
            }
        }
    }
}