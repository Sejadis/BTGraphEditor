using System;
using System.Collections.Generic;
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
        public static bool AllowMultipleChildren => false;

        public delegate void StateChanged(ResultState newState, BTNode source);

        public event StateChanged OnStateChanged;
        public Guid Guid => guid;

        protected BTNode()
        {
            guid = Guid.NewGuid();
            children = new List<BTNode>();
        }

        private ResultState currentState;

        public ResultState CurrentState
        {
            get => currentState;
            set
            {
                var old = currentState;
                currentState = value;
                // Debug.Log($"{this.GetType().Name}: {currentState.ToString()}");
                //TODO need a better way to check if changed
                // if (old != currentState)
                // {
                OnStateChanged?.Invoke(currentState, this);
                // }
            }
        }

        public void ResetEvent()
        {
            // OnStateChanged = null;
        }

        public abstract ResultState Execute();

        public virtual void AddChild(BTNode child)
        {
            if (children == null)
            {
                children = new List<BTNode>();
            }

            if (children.Count < 1 || this is IMayHaveMultipleChildren)
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