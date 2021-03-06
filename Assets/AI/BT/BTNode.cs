using System;
using System.Collections.Generic;
using Subtegral.DialogueSystem.Editor;

namespace AI.BT
{
    [Serializable]
    public abstract class BTNode
    {
        protected BTNode parent;
        protected List<BTNode> children = new List<BTNode>();

        public List<BTNode> Children => children;
        public static bool AllowMultipleChildren => true;

        public BTNode Parent => parent;
        public enum ResultState
        {
            Running, Success, Failure
        }


        public abstract ResultState Execute();

        public virtual void AddChild(BTNode child)
        {
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
