using AI.BTGraph.Attribute;

namespace AI.BT.Nodes
{
    public class SubGraphNode : LeafNode
    {
        [Option]
        public BehaviorTree subGraph;
        public override ResultState Execute()
        {
            return CurrentState = subGraph.rootNode.Execute();
        }
    }
}