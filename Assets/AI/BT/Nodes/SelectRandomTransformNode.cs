using System.Runtime.InteropServices;
using AI.BT.Nodes;
using AI.BTGraph.Attribute;
using UnityEngine;

namespace AI.BT
{
    public class SelectRandomTransformNode :BTNode
    {
        [Input] public BlackboardAccessor<Transform[]> array;
        [Output] public BlackboardAccessor<Transform> selection;
        public override ResultState Execute()
        {
            if (array.TryGetValue(out var transforms) && transforms.Length > 0)
            {
                selection.SetValue(transforms[Random.Range(0,transforms.Length)]);
                return CurrentState = ResultState.Success;
            }

            return CurrentState = ResultState.Failure;
        }
    }
}