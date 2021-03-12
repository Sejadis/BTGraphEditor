using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.BT.Nodes.Composite
{
    [Serializable]
    public abstract class CompositeNode : BTNode
    {
        public List<BTNode> Children => children;
        protected List<BTNode> children;

        protected CompositeNode() : base()
        {
            children = new List<BTNode>();
        }

        //TODO if exited with State running, continue at running node
        protected void ResetChildrenState()
        {
            foreach (var node in children)
            {
                node.CurrentState = ResultState.Inactive;
            }
        }


        public override void Sort(Dictionary<Guid, Rect> nodePositions)
        {
            if (children.Count == 0)
            {
                return;
            }

            foreach (var child in children)
            {
                if (child is CompositeNode compositeNode)
                {
                    compositeNode.Sort(nodePositions);
                }
            }

            children = children.OrderBy(child => nodePositions[child.Guid].position.y).ToList();
        }

        public virtual void AddChild(BTNode child)
        {
            children ??= new List<BTNode>();

            children.Add(child);
        }
    }
}