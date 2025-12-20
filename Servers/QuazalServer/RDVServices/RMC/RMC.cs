
using CustomLogger;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.DDL;
using QuazalServer.QNetZ.Interfaces;
using System.Reflection;

namespace QuazalServer.RDVServices.RMC
{
    public static class RMC
    {
        public static void HandlePacket(QPacketHandlerPRUDP handler, QPacket p, QClient client)
        {
            client.SessionID = p.m_bySessionID;

            if (p.uiSeqId > client.SeqCounter)
                client.SeqCounter = p.uiSeqId;

            RMCPacket rmc = new(handler, p);
            if (rmc.isRequest)
                HandleRequest(handler, client, p, rmc);
            else
                HandleResponse(handler, client, p, rmc);
        }

        public static void HandleResponse(QPacketHandlerPRUDP handler, QClient client, QPacket p, RMCPacket rmc)
        {
            WriteLog(client, $"Received Response:{rmc}", false);
            string message = rmc.success ? "Success" : $"Fail : {rmc.error:X8} for callID = {rmc.callID}";
            WriteLog(client, $"Got response for {rmc.proto} = {message}", false);

            handler.SendACK(p, client);
        }

        public static void HandleRequest(QPacketHandlerPRUDP handler, QClient client, QPacket p, RMCPacket rmc)
        {
            if (rmc.callID > client.CallCounterRMC)
                client.CallCounterRMC = rmc.callID;

            WriteLog(client, "Request:" + rmc.ToString(), false);

            if (p.payload == null)
            {
                WriteLog(client, $"NULL Payload for packet protocol '{rmc.proto}' (protocolId = {(int)rmc.proto})", true);
                return;
            }

            MemoryStream m = new(p.payload);
            m.Seek(rmc._afterProtocolOffset, SeekOrigin.Begin);

            RMCContext rmcContext = new(rmc, handler, client, p);

            // create service instance
            var serviceFactory = handler.Factory.Item2.GetServiceFactory(rmc.proto, handler.Factory.Item1);

            if (serviceFactory == null)
            {
                WriteLog(client, $"No service registered for packet protocol '{rmc.proto}' (protocolId = {(int)rmc.proto})", true);
                handler.SendACK(rmcContext.Packet, client);
                return;
            }

            // set the execution context
            RMCServiceBase? serviceInstance = serviceFactory();

            serviceInstance.Context = rmcContext;
            MethodInfo? bestMethod = serviceInstance.GetServiceMethodById(handler.Factory.Item2, rmc.methodID);

            if (bestMethod == null)
            {
                WriteLog(client, $"No method '{rmc.methodID}' registered for protocol '{rmc.proto}'", true);
                handler.SendACK(rmcContext.Packet, client);
                return;
            }

            // try invoke method method
            // TODO: extended info
            var typeList = bestMethod.GetParameters().Select(x => x.ParameterType);
            object[]? parameters = DDLSerializer.ReadPropertyValues(typeList.ToArray(), m);

            if (parameters == null)
                WriteLog(client, () => "No request parameters!", false);
            else if (parameters.Any(param => param is Stream))
                WriteLog(client, () => "Request parameters contains a stream, not logged yet!", false);
            else
                WriteLog(client, () => "Request parameters:" + DDLSerializer.ObjectToString(parameters), false);

            try
            {
                object? returnValue = bestMethod.Invoke(serviceInstance, parameters);

                if (returnValue != null)
                {
                    if (typeof(RMCResult).IsAssignableFrom(returnValue.GetType()))
                    {
                        RMCResult rmcResult = (RMCResult)returnValue;

                        SendResponseWithACK(
                                handler,
                                rmcContext.Packet,
                                rmcContext.RMC,
                                rmcContext.Client,
                                rmcResult.Response,
                                rmcResult.Compression, rmcResult.Error);
                    }
                    else if (typeof((byte[], bool)).IsAssignableFrom(returnValue.GetType()))
                    {
                        (byte[], bool) TupleData = ((byte[], bool))returnValue;
                        SendResponseByteArrayPayload(handler, rmcContext.Packet, rmcContext.Client, TupleData.Item1, TupleData.Item2);
                    }
                    else if (typeof(byte[]).IsAssignableFrom(returnValue.GetType()))
                        SendResponseByteArrayPayload(handler, rmcContext.Packet, rmcContext.Client, (byte[])returnValue, false);
                    else
                    {
                        // TODO: try to cast and create RMCPResponseDDL???
                        LoggerAccessor.LogError("The data type returned by the RMC method is not handled yet!");
                        return;
                    }
                }
                else
                    handler.SendACK(rmcContext.Packet, client);
            }
            catch (TargetInvocationException tie)
            {
                handler.SendACK(rmcContext.Packet, client);

                WriteLog(client, $"Exception occurred in {rmc.proto}.{bestMethod.Name}", true);

                Exception? inner = tie.InnerException;
                if (inner != null)
                {
                    WriteLog(client, $"{inner.Message}", true);

                    if (inner.StackTrace != null)
                        WriteLog(client, $"{inner.StackTrace}", true);
                }
            }
        }

        public static void SendResponseWithACK(QPacketHandlerPRUDP handler, QPacket p, RMCPacket rmc, QClient client, RMCPResponse reply, bool useCompression = true, uint error = 0)
        {
            WriteLog(client, "Response:" + reply.ToString(), false);
            WriteLog(client, () => "Response data:" + reply.PayloadToString(), false);

            handler.SendACK(p, client);
#if DEBUG
            WriteLog(client, () => "Sent ACK.", false);
#endif
            SendResponsePacket(handler, p, rmc, client, reply, useCompression, error);
        }

        public static void SendRMCCall(QPacketHandlerPRUDP handler, QClient client, ushort protoId, uint methodId, RMCPRequest requestData)
        {
            QPacket packet = new(handler.AccessKey)
            {
                m_oSourceVPort = new QPacket.VPort(0x31),
                m_oDestinationVPort = new QPacket.VPort(0x3f),

                type = QPacket.PACKETTYPE.DATA,
                flags = new List<QPacket.PACKETFLAG>() { QPacket.PACKETFLAG.FLAG_RELIABLE | QPacket.PACKETFLAG.FLAG_NEED_ACK },
                payload = Array.Empty<byte>(),
                m_bySessionID = client.SessionID
            };

            RMCPacket rmc = new(handler)
            {
                proto = protoId,
                methodID = methodId
            };

            WriteLog(client, $"Sending call {protoId}.{methodId}", false);
            WriteLog(client, () => "Call data:" + requestData.PayloadToString(), false);

            SendRequestPacket(handler, packet, rmc, client, requestData, true, 0);
        }

        private static void SendResponsePacket(QPacketHandlerPRUDP handler, QPacket p, RMCPacket rmc, QClient client, RMCPResponse reply, bool useCompression, uint error)
        {
            rmc.isRequest = false;
            rmc.response = reply;
            rmc.error = error;

            byte[] rmcResponseData = rmc.ToBuffer();

            QPacket np = new(handler.AccessKey, p.toBuffer(handler.AccessKey))
            {
                flags = new List<QPacket.PACKETFLAG>() { QPacket.PACKETFLAG.FLAG_NEED_ACK, QPacket.PACKETFLAG.FLAG_RELIABLE },
                m_oSourceVPort = p.m_oDestinationVPort,
                m_oDestinationVPort = p.m_oSourceVPort,
                m_uiSignature = client.IDsend,
                usesCompression = useCompression
            };

            handler.MakeAndSend(client, p, np, rmcResponseData);
        }

        private static void SendResponseByteArrayPayload(QPacketHandlerPRUDP handler, QPacket p, QClient client, byte[] data, bool useCompression)
        {
            QPacket np = new(handler.AccessKey, p.toBuffer(handler.AccessKey))
            {
                flags = new List<QPacket.PACKETFLAG>() { QPacket.PACKETFLAG.FLAG_NEED_ACK, QPacket.PACKETFLAG.FLAG_RELIABLE },
                m_oSourceVPort = p.m_oDestinationVPort,
                m_oDestinationVPort = p.m_oSourceVPort,
                m_uiSignature = client.IDsend,
                usesCompression = useCompression
            };

            handler.MakeAndSend(client, p, np, data);
        }

        private static void SendRequestPacket(QPacketHandlerPRUDP handler, QPacket p, RMCPacket rmc, QClient client, RMCPRequest request, bool useCompression, uint error)
        {
            rmc.isRequest = true;
            rmc.request = request;
            rmc.error = error;
            rmc.callID = ++client.CallCounterRMC;

            byte[] rmcRequestData = rmc.ToBuffer();

            QPacket np = new(handler.AccessKey, p.toBuffer(handler.AccessKey))
            {
                flags = new List<QPacket.PACKETFLAG>() { QPacket.PACKETFLAG.FLAG_RELIABLE | QPacket.PACKETFLAG.FLAG_NEED_ACK },
                m_uiSignature = client.IDsend,
                usesCompression = useCompression
            };

            handler.MakeAndSend(client, p, np, rmcRequestData);
        }

        private static void WriteLog(QClient client, Func<string> resolve, bool err)
        {
            string? unknwnClientName = client.PlayerInfo != null ? client.PlayerInfo.Name : "<unkClient>";
            if (err)
                LoggerAccessor.LogError($"[RMC] ({unknwnClientName}) {resolve.Invoke()}");
            else
                LoggerAccessor.LogInfo($"[RMC] ({unknwnClientName}) {resolve.Invoke()}");
        }

        private static void WriteLog(QClient client, string s, bool err)
        {
            string? unknwnClientName = client.PlayerInfo != null ? client.PlayerInfo.Name : "<unkClient>";
            if (err)
                LoggerAccessor.LogError($"[RMC] ({unknwnClientName}) {s}");
            else
                LoggerAccessor.LogInfo($"[RMC] ({unknwnClientName}) {s}");
        }
    }
}
