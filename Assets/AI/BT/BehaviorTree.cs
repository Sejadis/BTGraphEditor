using System.Collections.Generic;
using UnityEngine;

namespace AI.BT
{
    // [CreateAssetMenu(fileName = "Assets/bt", menuName = "Create/BT")]
    public class BehaviorTree
    {
        public RootNode rootNode;
        public List<BTNode> nodes = new List<BTNode>();
    }
}