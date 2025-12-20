using System.Reflection;

namespace Tdf
{
    internal class TdfValueContainer<T> : ITdfValueContainer
    {
        public TdfValueContainer()
        {
            ValueFieldInfo = GetType().GetField(nameof(Value))!;
        }

        public T? Value;

        public FieldInfo ValueFieldInfo { get; }
        object? ITdfValueContainer.Value { get => Value; }
    }
}
