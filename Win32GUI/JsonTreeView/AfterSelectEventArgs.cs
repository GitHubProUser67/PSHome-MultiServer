namespace ZTn.Json.JsonTreeView
{
    public class AfterSelectEventArgs(
        string typeName,
        string jTokenTypeName,
        Func<string> getJsonString
    ) : EventArgs
    {
        public string TypeName { get; private set; } = typeName;
        public string JTokenTypeName { get; } = jTokenTypeName;
        public Func<string> GetJsonString { get; } = getJsonString;
    }
}
