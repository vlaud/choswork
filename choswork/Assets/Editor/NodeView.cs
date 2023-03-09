public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Node node;

    public NodeView(Node node)
    {
        this.node = node;
        this.title = node.name;
    }
}
