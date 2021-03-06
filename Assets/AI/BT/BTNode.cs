using System;
using System.Collections.Generic;
using Subtegral.DialogueSystem.Editor;
using UnityEngine;

namespace AI.BT
{
    [Serializable]
    public abstract class BTNode
    {
        private Guid guid;
        protected BTNode parent;
        protected List<BTNode> children;

        public BTNode Parent => parent;
        public List<BTNode> Children => children;
        public static bool AllowMultipleChildren => true;

        public Guid Guid => guid;

        protected BTNode()
        {
            guid = Guid.NewGuid();
            children = new List<BTNode>();
        }

        public abstract ResultState Execute();

        public virtual void AddChild(BTNode child)
        {
            if (children == null)
            {
                children = new List<BTNode>();
            }
            
            if (children.Count < 1 || AllowMultipleChildren)
            {
                children.Add(child);
            }
            else
            {
                throw new InvalidOperationException("RootNode can not have more than 1 children");
            }
        }

        public virtual void SetParent(BTNode parent)
        {
            this.parent = parent;
        }
    }
}