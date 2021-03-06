using System;
using System.Collections.Generic;

namespace AI.BT
{
    [Serializable]
    public class SerializedBTNode
    {
        public string guid;
        public string type;
        public string parent;
        public List<string> children;

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
    }
}