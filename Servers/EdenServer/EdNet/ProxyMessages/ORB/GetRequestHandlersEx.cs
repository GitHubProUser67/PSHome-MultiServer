using EdNetService.CRC;
using EdNetService.Models;
using EndianTools;
using NetworkLibrary.Extension;
using System.Net;
using System.Reflection;

namespace EdenServer.EdNet.ProxyMessages
{
    public class GetRequestHandlersEx : AbstractProxyMessage
    {
        private static readonly FieldInfo[] storeBank = typeof(edStoreBank).GetFields(BindingFlags.Public | BindingFlags.Static);

        public override byte[]? Process(IPEndPoint endpoint, IPEndPoint target, ClientTask task, ushort PacketMagic)
        {
            byte numOfHandlers = (byte)EdPropsU8.COREREQUESTS_MAX_GETRPC_HANDLERS_EX;
            List<ushort> request_crc_in = new List<ushort>(numOfHandlers);
            uint targetIp = EndianUtils.ReverseUint(InternetProtocolUtils.GetIPAddressAsUInt(target.Address.MapToIPv4()));
            ushort targetPort = (ushort)target.Port;

            EdStore request = task.Request;

            task.Client.UserId = request.ExtractUInt32();
            task.Client.SessionId = request.ExtractUInt64();

            for (byte i = 0; i < numOfHandlers; i++)
                request_crc_in.Add(request.ExtractUInt16());

            // Context (dunno what can be worth looking in this)
            request.ExtractRawBytes((ushort)request.FreeSize);

            EdStore response = new EdStore(null, (8 * request_crc_in.Count) + 4);

            response.InsertStart(edStoreBank.CRC_COREREQUESTS_A_GET_REQUEST_HANDLERS_EX);

            foreach (ushort handler_crc in request_crc_in)
            {
                if (handler_crc != 0)
                {
                    var matchingField = storeBank
                        .Select(f => new { f.Name, CRC = (ushort?)f.GetValue(null) })
                        .FirstOrDefault(f => f.CRC == handler_crc);

                    if (matchingField != null && matchingField.CRC.HasValue)
                    {
                        string serviceName = matchingField.Name;
#if DEBUG
                        CustomLogger.LoggerAccessor.LogInfo($"[GetRequestHandlersEx] - Found service:{serviceName} for CRC:{matchingField.CRC.Value:X4}");
#endif
                        if (serviceName.EndsWith("SERVER"))
                        {
                           var service = EdenServerConfiguration.GetServerConfigByServiceName(serviceName);
                            if (service != null)
                            {
                                string? configIp = service.Value.address;
                                ushort? configPort = service.Value.port;
                                if (!string.IsNullOrEmpty(configIp) && configPort.HasValue)
                                {
                                    response.InsertUInt16(handler_crc);
                                    response.InsertUInt32(InternetProtocolUtils.GetIPAddressAsUInt(configIp));
                                    response.InsertUInt16(configPort.Value);
                                    continue;
                                }
                            }
                        }

                        // Default to Proxy IP.
                        response.InsertUInt16(handler_crc);
                        response.InsertUInt32(targetIp);
                        response.InsertUInt16(targetPort);
                    }
                    else
                    {
#if DEBUG
                        CustomLogger.LoggerAccessor.LogWarn($"[GetRequestHandlersEx] - Unknown service for CRC:{handler_crc:X4}");
#endif
                        response.InsertUInt16(handler_crc);
                        response.InsertUInt32(0);
                        response.InsertUInt16(0);
                    }
                }
            }

            response.InsertEnd();

            task.Response = response;
            task.Target = endpoint;
            task.ClientMode = ClientMode.ProxyServer;

            return null;
        }
    }
}
