using QuazalServer.AtariMelbourneHouse;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.v2Services
{
    [RMCService((ushort)RMCProtocolId.LairAccountManagementProtocol)]
    public class LairAccountManagementProtocolService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult AmhLairLogin()
        {
            // We need to send the EDNET ip in hex little endian.

            var destip = QuazalServerConfiguration.ServerBindAddress;
            if (!string.IsNullOrEmpty(QuazalServerConfiguration.EdNetBindAddressOverride))
                destip = QuazalServerConfiguration.EdNetBindAddressOverride;
            else if (QuazalServerConfiguration.UsePublicIP)
                destip = QuazalServerConfiguration.ServerPublicBindAddress;

            AmhLairProxy.TryConvertIpAddressToHex(destip, out var result);
            return Result(new { retVal = result });
        }
    }
}
