namespace ZTn.Json.JsonTreeView.Extensions
{
    public class JTokenTreeNodeDeleteException(Exception sourceException)
        : AggregateException(sourceException) { }
}
