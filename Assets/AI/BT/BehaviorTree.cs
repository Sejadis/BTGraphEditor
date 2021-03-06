using UnityEngine;

namespace AI.BT
{
    [CreateAssetMenu(fileName = "Assets/bt", menuName = "Create/BT")]
    public class BehaviorTree : ScriptableObject
    {
        public RootNode rootNode;
    }
}