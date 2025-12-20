using Horizon.RT.Models;
using System.Net;

namespace Horizon.LIBRARY.Pipeline.Udp
{
    public class ScertDatagramPacket
    {
        public EndPoint Source { get; set; }

        public EndPoint Destination { get; set; }

        public BaseScertMessage Message { get; set; }

        public ScertDatagramPacket(BaseScertMessage message, EndPoint destination)
        {
            Destination = destination;
            Source = null;
            Message = message;
        }

        public ScertDatagramPacket(BaseScertMessage message, EndPoint target, EndPoint source)
        {
            Source = source;
            Destination = target;
            Message = message;
        }

        /// <summary>
        /// Whether or not this message passes the log filter.
        /// </summary>
        public virtual bool CanLog()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public override string ToString()
        {
            return $"Source: {Source} | Destination: {Destination} | Message: {Message}";
        }
    }
}