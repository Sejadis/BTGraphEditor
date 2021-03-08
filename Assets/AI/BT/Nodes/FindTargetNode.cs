using System.Collections.Generic;
using AI.BTGraph.Attribute;
using UnityEngine;

namespace AI.BT.Nodes
{
    public class FindTargetNode : BTNode
    {
        [Input] public BlackboardAccessor<List<Transform>> Targets = new BlackboardAccessor<List<Transform>>("targets");
        [Output] public BlackboardAccessor<Transform> Target = new BlackboardAccessor<Transform>("target");

        public override ResultState Execute()
        {
            var resultState = ResultState.Inactive;
            if (Targets.TryGetValue(out var targets))
            {
                resultState = targets.Count > 0 ? ResultState.Success : ResultState.Failure;
            }
            else
            {
                resultState = ResultState.Failure;
            }

            Transform target = null;
            if (resultState == ResultState.Success)
            {
                target = targets[0];
            }

            Target.SetValue(target);

            return CurrentState = resultState;
        }
    }
}