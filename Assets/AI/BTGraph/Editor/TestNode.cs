using System;
using UnityEditor.Experimental.GraphView;

namespace AI.BTGraph.Editor
{
    public class TestNode : Node
    {
        public TestNode(Type type = null)
        {
            type = type ?? typeof(int);
            var outPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
                type);
            outputContainer.Add(outPort);
            var inPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi,
                type);
            inputContainer.Add(inPort);
            RefreshPorts();
            RefreshExpandedState();
        }
    }
}