using Tdf;

namespace BlazeCommon
{
    public class BlazeClientConfiguration
    {
        public ITdfEncoder Encoder { get; }
        public ITdfDecoder Decoder { get; }

        Dictionary<ushort, IBlazeClientComponent> _components;

        public BlazeClientConfiguration(ITdfEncoder encoder, ITdfDecoder decoder)
        {
            Encoder = encoder;
            Decoder = decoder;
            _components = new Dictionary<ushort, IBlazeClientComponent>();
        }

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
            _components.TryGetValue(componentId, out IBlazeClientComponent? component);
            return component;
        }
    }
}
