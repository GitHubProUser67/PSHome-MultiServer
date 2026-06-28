using Tdf;

namespace BlazeCommon
{
    public class BlazeClientConfiguration(ITdfEncoder encoder, ITdfDecoder decoder)
    {
        public ITdfEncoder Encoder { get; } = encoder;
        public ITdfDecoder Decoder { get; } = decoder;

        readonly Dictionary<ushort, IBlazeClientComponent> _components = [];

        public bool AddComponent(IBlazeClientComponent component)
        {
            return _components.TryAdd(component.Id, component);
        }

        public bool RemoveComponent(ushort componentId, out IBlazeClientComponent? component)
        {
            return _components.Remove(componentId, out component);
        }

        public IBlazeClientComponent? GetComponent(ushort componentId)
        {
            _components.TryGetValue(componentId, out var component);
            return component;
        }
    }
}
