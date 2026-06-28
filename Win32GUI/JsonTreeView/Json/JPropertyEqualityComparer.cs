using Newtonsoft.Json.Linq;

namespace ZTn.Json.JsonTreeView.Json
{
    /// <summary>
    /// Defines methods to support the comparison of <see cref="JProperty"/> objects for equality.
    /// </summary>
    sealed class JPropertyEqualityComparer : IEqualityComparer<JProperty>
    {
        /// <inheritdoc />
        public bool Equals(JProperty x, JProperty y)
        {
            return x.Name == y.Name;
        }

        /// <inheritdoc />
        public int GetHashCode(JProperty obj)
        {
            return obj == null ? 0 : obj.Name.GetHashCode();
        }
    }
}
