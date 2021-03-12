using UnityEngine;
using UnityEngine.AI;

namespace AI.BT.Sensors
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentSensor : BTSensor
    {
        private NavMeshAgent agent;
        private new void Start()
        {
            base.Start();
            agent = GetComponent<NavMeshAgent>();
            WriteToBlackBoard(agent);
        }
        
    }
}