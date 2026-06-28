using Newtonsoft.Json.Linq;

namespace ZTn.Json.JsonTreeView
{
    /// <summary>
    /// Exception thrown when a <see cref="JToken"/> instance is of an unattended type.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the class with the faulty <see cref="JToken"/> instance.
    /// </remarks>
    /// <param name="jToken"></param>
    public sealed class UnattendedJTokenTypeException(JToken jToken)
        : Exception("Unattended JToken type encountered: " + jToken.GetType().FullName) { }
}
