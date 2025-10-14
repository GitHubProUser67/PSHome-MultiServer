using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Tdf;

namespace BlazeCommon
{
    public delegate void ConnectionDelegate(BlazeServerConnection connection);
    public delegate void ConnectionOnRequestDelegate(BlazeServerConnection connection, ProtoFirePacket packet, bool unhandled);
    public delegate void ConnectionOnErrorDelegate(BlazeServerConnection connection, Exception exception);
    public class BlazeServerConfiguration
    {
        public string Name { get; }
        public string MitmTargetIp { get; set; }
        public string MitmTargetHostname { get; set; }
        public ushort MitmTargetPort { get; set; }
        public bool MitmWriteToFile { get; set; }
        public SslProtocols MitmProtocols { get; set; }
        public IPEndPoint LocalEP { get; }
        public X509Certificate2? Certificate { get; set; }
        public bool ForceSsl { get; set; }
        public ITdfEncoder Encoder { get; }
        public ITdfDecoder Decoder { get; }
        public int ComponentNotFoundErrorCode { get; set; }
        public int CommandNotFoundErrorCode { get; set; }
        public int ErrSystemErrorCode { get; set; }
        public ConnectionDelegate? OnNewConnection { get; set; }
        public ConnectionDelegate? OnDisconnected { get; set; }
        public ConnectionOnRequestDelegate? OnRequest { get; set; }
        public ConnectionOnErrorDelegate? OnError { get; set; }

        Dictionary<ushort, IBlazeServerComponent> _components;

        public BlazeServerConfiguration(string name, IPEndPoint endPoint, ITdfEncoder encoder, ITdfDecoder decoder)
        {
            Name = name;
            LocalEP = endPoint;
            Encoder = encoder;
            Decoder = decoder;
            _components = new Dictionary<ushort, IBlazeServerComponent>();

            //Taken from Blaze 3
            ComponentNotFoundErrorCode = 1073872896;
            CommandNotFoundErrorCode = 1073938432;
            ErrSystemErrorCode = 1073807360;
        }


        public bool AddComponent<TComponent>() where TComponent : IBlazeServerComponent, new()
        {
            TComponent component = new TComponent();
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
            _components.TryGetValue(componentId, out IBlazeServerComponent? component);
            return component;
        }
    }
}
