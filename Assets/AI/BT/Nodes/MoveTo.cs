using AI.BTGraph.Attribute;
using UnityEngine;
using UnityEngine.AI;

namespace AI.BT.Nodes
{
    public class MoveTo : BTNode
    {
        [Input] public NavMeshAgent agent;
        [Input] public Transform target;
        private bool pathSet = false;
        public override ResultState Execute()
        {
            if (agent == null)
            {
                agent = GameObject.FindObjectOfType<NavMeshAgent>();
                if (agent == null)
                {
                    // Debug.Log("MoveToNode: " + CurrentState);

                    return CurrentState = ResultState.Failure;
                }
            }

            if (target == null)
            {
                var targets = GameObject.FindGameObjectsWithTag("Target");
                if (targets == null)
                {
                    // Debug.Log("MoveToNode: " + CurrentState);

                    return CurrentState = ResultState.Failure;
                }

                target = targets[Random.Range(0, targets.Length)].transform;
                if (Vector3.Distance(agent.transform.position, target.position) < 1f)
                {
                    target = targets[Random.Range(0, targets.Length)].transform;
                }
                Debug.Log("new target is " + target.transform.name);
            }

            if (!pathSet)
            {
                agent.SetDestination(target.position);
                pathSet = true;
            }

            if (pathSet)
            {
                if (Vector3.Distance(agent.transform.position,target.position) < 2f)
                {
                    target = null;
                    pathSet = false;
                    return CurrentState = ResultState.Success;
                }

                return CurrentState = ResultState.Running;
            }

            return CurrentState = ResultState.Failure;
        }
    }
}