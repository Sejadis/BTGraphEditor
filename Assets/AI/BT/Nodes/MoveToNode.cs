using AI.BTGraph.Attribute;
using UnityEngine;
using UnityEngine.AI;

namespace AI.BT.Nodes
{
    public class MoveTo : BTNode
    {
        public NavMeshAgent agent;
        [Input] public BlackboardAccessor<Transform> Target;
        private Transform currentTarget;
        private bool pathSet = false;

        public override ResultState Execute()
        {
            if (agent == null)
            {
                agent = GameObject.FindObjectOfType<NavMeshAgent>();
                if (agent == null)
                {
                    return CurrentState = ResultState.Failure;
                }
            }

            if (!Target.TryGetValue(out var newTarget))
            {
                return CurrentState = ResultState.Failure;
            }

            if (!newTarget.Equals(currentTarget) || Vector3.Distance(agent.destination, newTarget.position) > 3f)
            {
                agent.SetDestination(newTarget.position);
                currentTarget = newTarget;
            }

            if (currentTarget != null && Vector3.Distance(agent.transform.position, currentTarget.position) < 2f)
            {
                return CurrentState = ResultState.Success;
            }
            else if (currentTarget == null)
            {
                return CurrentState = ResultState.Failure;
            }
            else
            {
                //TODO figure out how to check if failed
                return CurrentState = ResultState.Running;
            }

            return CurrentState = ResultState.Failure;
        }
    }
}