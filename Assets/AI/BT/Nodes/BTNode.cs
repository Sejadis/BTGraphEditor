using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AI.BT
{
    [Serializable]
    public abstract class BTNode
    {
        public BTNode Parent => parent;
        public List<BTNode> Children => children;
        public static bool AllowMultipleChildren => false;

        public delegate void StateChanged(ResultState newState, BTNode source);

        public event StateChanged OnStateChanged;

        public Guid Guid => guid;

        protected BTNode parent;
        protected List<BTNode> children;

        private Guid guid;
        private bool isInitialized = false;

        protected BTNode()
        {
            //TODO ? grab all accessors and store in list ?
            guid = Guid.NewGuid();
            children = new List<BTNode>();
        }

        //TODO ? reflection methods as extension methods?

        #region reflection

        public List<FieldInfo> GetBlackboardAccessorFieldInfos()
        {
            var result = new List<FieldInfo>();
            var fields = GetType().GetFields();
            foreach (var fieldInfo in fields)
            {
                if (!fieldInfo.FieldType.IsGenericType) continue;

                if (fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(BlackboardAccessor<>))
                {
                    result.Add(fieldInfo);
                }
            }

            return result;
        }

        public object GetOrCreateBlackboardAccessor(FieldInfo fieldInfo)
        {
            var accessor = fieldInfo.GetValue(this);
            if (accessor == null)
            {
                accessor = BlackboardAccessor.CreateFromFieldInfo(fieldInfo);
                fieldInfo.SetValue(this, accessor);
            }

            return accessor;
        }

        public void SetBlackboardForAllAccessors(Blackboard blackboard)
        {
            var fields = GetType().GetFields();
            foreach (var fieldInfo in fields)
            {
                if (!fieldInfo.FieldType.IsGenericType) continue;
                
                if (fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(BlackboardAccessor<>))
                {
                    var field = fieldInfo.GetValue(this);
                    if (field == null)
                    {
                        field = BlackboardAccessor.CreateFromFieldInfo(fieldInfo);
                        fieldInfo.SetValue(this,field);
                    }
                    var property = field.GetType().GetProperty("Blackboard");
                    if (property != null)
                    {
                        property.SetValue(field, blackboard);
                    }
                }
            }
        }

        #endregion

        public void Initialize(Blackboard blackboard, List<PropertyKeyPair> blackboardConnections)
        {
            if (isInitialized)
            {
                Debug.LogWarning("Trying to re-initialize a node that is already initialized");
                return;
            }

            //TODO take a look at closure allocation
            foreach (var fieldInfo in GetBlackboardAccessorFieldInfos())
            {
                // //TODO probably move creation to when setting the key
                var accessor = GetOrCreateBlackboardAccessor(fieldInfo);

                //set blackboard
                var blackboardProperty = accessor.GetType().GetProperty("Blackboard");
                if (blackboardProperty != null)
                {
                    blackboardProperty.SetValue(accessor, blackboard);
                }

                //set key
                var key = blackboardConnections.Find(pkp => pkp.propertyName.Equals(fieldInfo.Name)).key;
                key = key.Replace("(", "").Replace(")", "");
                if (!string.IsNullOrEmpty(key))
                {
                    var keyProperty = accessor.GetType().GetProperty("Key");
                    if (keyProperty != null)
                    {
                        keyProperty.SetValue(accessor, key);
                    }
                }
            }

            isInitialized = true;
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