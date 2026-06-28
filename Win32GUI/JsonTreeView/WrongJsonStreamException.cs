namespace ZTn.Json.JsonTreeView
{
    public class WrongJsonStreamException(string message, Exception innerException)
        : Exception(message, innerException) { }
}
