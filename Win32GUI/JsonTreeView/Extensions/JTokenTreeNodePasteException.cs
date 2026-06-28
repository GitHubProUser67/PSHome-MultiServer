namespace ZTn.Json.JsonTreeView.Extensions
{
    public class JTokenTreeNodePasteException(Exception sourceException)
        : AggregateException(sourceException) { }
}
