using System.Linq;
using System.Net.NetworkInformation;

namespace MultiServerLibrary.Extension
{
    public static class UDPUtils
    {
        /// <summary>
        /// Know if the given UDP port is available.
        /// <para>Savoir si le port UDP en question est disponible.</para>
        /// </summary>
        /// <param name="port">The port on which we scan.</param>
        /// <returns>A boolean.</returns>
        public static bool IsUDPPortAvailable(int port)
        {
#if DEBUG
            CustomLogger.LoggerAccessor.LogInfo("[UDPUtils] - Checking Port {0}", port);
#endif
            // Evaluate current system udp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our UdpClient is occupied, we will set isAvailable to false.
            bool isAvailable = !IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners().Any(p => p.Port == port);
#if DEBUG
            CustomLogger.LoggerAccessor.LogInfo("[UDPUtils] - Port {0} available = {1}", port, isAvailable);
#endif
            return isAvailable;
        }
    }
}
