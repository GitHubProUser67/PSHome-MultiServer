using System.Reflection;

namespace Tdf
{
    internal interface ITdfValueContainer
    {
        object? Value { get; }
        FieldInfo ValueFieldInfo { get; }
    }
}
