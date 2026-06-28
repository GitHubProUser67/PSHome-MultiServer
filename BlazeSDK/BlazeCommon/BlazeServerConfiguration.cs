using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Tdf;

namespace BlazeCommon
{
    public delegate void ConnectionDelegate(BlazeServerConnection connection);
    public delegate void ConnectionOnRequestDelegate(
        BlazeServerConnection connection,
        ProtoFirePacket packet,
        bool unhandled
    );
    public delegate void ConnectionOnErrorDelegate(
        BlazeServerConnection connection,
        Exception exception
    );

    public class BlazeServerConfiguration(
        string name,
        IPEndPoint endPoint,
        ITdfEncoder encoder,
        ITdfDecoder decoder
    )
    {
        public string Name { get; } = name;
        public string MitmTargetIp { get; set; }
        public string MitmTargetHostname { get; set; }
        public ushort MitmTargetPort { get; set; }
        public bool MitmWriteToFile { get; set; }
        public SslProtocols MitmProtocols { get; set; }
        public IPEndPoint LocalEP { get; } = endPoint;
        public X509Certificate2? Certificate { get; set; }
        public bool ForceSsl { get; set; }
        public ITdfEncoder Encoder { get; } = encoder;
        public ITdfDecoder Decoder { get; } = decoder;
        public int ComponentNotFoundErrorCode { get; set; } = 1073872896;
        public int CommandNotFoundErrorCode { get; set; } = 1073938432;
        public int ErrSystemErrorCode { get; set; } = 1073807360;
        public ConnectionDelegate? OnNewConnection { get; set; }
        public ConnectionDelegate? OnDisconnected { get; set; }
        public ConnectionOnRequestDelegate? OnRequest { get; set; }
        public ConnectionOnErrorDelegate? OnError { get; set; }

        readonly Dictionary<ushort, IBlazeServerComponent> _components = [];

        public bool AddComponent<TComponent>()
            where TComponent : IBlazeServerComponent, new()
        {
            var component = new TComponent();
            return _components.TryAdd(component.Id, component);
        }

        public bool AddComponent(IBlazeServerComponent component)
        {
            return _components.TryAdd(component.Id, component);
        }

        public bool RemoveComponent(ushort componentId, out IBlazeServerComponent? component)
        {
            return _components.Remove(componentId, out component);
        }

        public IBlazeServerComponent? GetComponent(ushort componentId)
        {
            _components.TryGetValue(componentId, out var component);
            return component;
        }
    }
}
