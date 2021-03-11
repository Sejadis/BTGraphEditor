using AI.BTGraph.Attribute;

namespace AI.BT.Nodes
{
    public class SubGraphNode : BTNode, IHasNoChildren
    {
        [Option]
        public BehaviorTree subGraph;
        public override ResultState Execute()
        {
            return CurrentState = subGraph.rootNode.Execute();
        }
    }
}