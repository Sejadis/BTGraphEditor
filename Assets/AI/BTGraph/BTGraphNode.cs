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
        private Port inputPort;
        private Port outputPort;

        private List<Port> ports = new List<Port>();

        //TODO refactor tuple into separate element
        private Dictionary<Port, (ToolbarMenu, Label)> dropdowns = new Dictionary<Port, (ToolbarMenu, Label)>();

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
            ports.Add(outputPort);
            outputContainer.Add(outputPort);
            if (!runtimeNodeData.hasNoChildren)
            {
                inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input,
                    RuntimeNodeData.allowMultipleChildren ? Port.Capacity.Multi : Port.Capacity.Single,
                    typeof(ResultState));
                inputPort.portName = "IN";
                ports.Add(inputPort);
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
                //if we have a generic type use it as the port type
                //TODO change to only work with BlackboardAccessor
                var genericTypes = kvp.Value.GetGenericArguments();
                var type = genericTypes.Length > 0 ? genericTypes[0] : kvp.Value;
                var port = InstantiatePort(Orientation.Horizontal, direction, capacity, type);
                port.portName = kvp.Key;
                port.style.justifyContent = Justify.SpaceBetween;
                var element = new VisualElement();
                element.style.flexDirection = FlexDirection.Row;
                var label = new Label("(none)");
                var dropdown = new ToolbarMenu();
                element.Add(label);
                element.Add(dropdown);
                port.contentContainer.Add(element);
                // port.contentContainer.Add(label);
                // port.contentContainer.Add(dropdown);

                dropdowns[port] = (dropdown, label);

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


        public void OnBlackboardValuesChanged(List<(string, string)> values)
        {
            foreach (var port in ports)
            {
                var bbValues = values.Where(tuple =>
                    {
                        string typeString;
                        if (!TypeMapper.typeMap.TryGetValue(tuple.Item2, out typeString))
                        {
                            //type is not mapped
                            typeString = tuple.Item2;
                        }

                        var valueType = Type.GetType(typeString);
                        return port.portType == valueType;
                    })
                    .Select(tuple => tuple.Item1);
                if (bbValues.Any())
                {
                    ClearDropdown(dropdowns[port].Item1);
                    foreach (var bbValue in bbValues)
                    {
                        dropdowns[port].Item1.menu.AppendAction(bbValue,
                            action => { dropdowns[port].Item2.text = $"({action.name})"; });
                    }

                    // var logString = "Keys found for Port " + port.portName + ":";
                    // logString = bbValues.Aggregate(logString, (current, value) => current + (" " + value));
                    //
                    // Debug.Log(logString);
                }
            }
        }

        private void ClearDropdown(ToolbarMenu dropdown)
        {
            for (var i = dropdown.menu.MenuItems().Count - 1; i >= 0; i--)
            {
                dropdown.menu.RemoveItemAt(i);
            }
        }
    }
}