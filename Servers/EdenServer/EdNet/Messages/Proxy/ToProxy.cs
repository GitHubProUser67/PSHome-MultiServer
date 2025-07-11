using CustomLogger;
using EdenServer.ClientChallengeService;
using EdenServer.EdNet.ProxyMessages;
using EdenServer.EdNet.ProxyMessages.Database.EdBuffer;
using EdenServer.EdNet.ProxyMessages.Database.Login;
using EdenServer.EdNet.ProxyMessages.ORB;
using EdenServer.EdNet.ProxyMessages.ORB.File;
using EdNetService.CRC;
using EdNetService.Models;
using EndianTools;
using MultiServerLibrary.Extension;
using System.Net;
using System.Text;

namespace EdenServer.EdNet.Messages
{
    public class ToProxy : AbstractMessage
    {
        const string Url = "http://github.com/GitHubProUser67/MultiServer3";

        // true value means we force LittleEndian cryptography.
        private static readonly Dictionary<string, bool> allowedVersions = new Dictionary<string, bool>() { { "MC 1.45 A", true }, { "MC 1.66 A", true }, { "[VED]TDU2", false } };

        private static Dictionary<ushort, Type?> ORBCrcToClass { get; } = new Dictionary<ushort, Type?>() {

            // Core
            { edStoreBank.COREREQUESTS_R_LOG, typeof(Log) },
            { edStoreBank.CRC_COREREQUESTS_R_GET_REQUEST_HANDLERS_EX, typeof(GetRequestHandlersEx) },

            // Login
            { edStoreBank.COREREQUESTS_R_LOGIN_PC, typeof(LoginPC) },

            // ORB File
            { edStoreBank.CRC_R_ORB_OPENFILE, typeof(OpenFile) },
            { edStoreBank.CRC_R_ORB_GETFILE, typeof(GetFile) },

            // Events
            { edStoreBank.CRC_E_STATS_USER_STATISTICS_UPDATE_V2, null },
        };

        private byte[]? UrlBytes;

        private uint ProductId;
        private byte[]? Version;
        private byte[]? Key;
        private byte[]? Username;

        private uint Question1;
        private uint Question2;
        private uint Question3;
        private uint Answer1;
        private uint Answer2;
        private uint Answer3;

        private uint TargetIp;
        private ushort TargetPort;
        private uint ClientId;
        private ushort PayloadSize;
        private byte[]? Payload;

        public override bool Process(AbstractEdenServer server, IPEndPoint endpoint, EdStore store)
        {
            if (server is not ProxyServer) return false;

            ClientObject? client = null;
            byte[]? ResultBuffer = null;
            byte Command = store.ExtractUInt8();
            store.ExtractUInt8();

            switch (Command)
            {
                case 0: // Client to Host - Initialize
                    UrlBytes = new byte[512];
                    EdStore initResponse = new EdStore(null, 616);

                    Array.Copy(Encoding.ASCII.GetBytes(Url), UrlBytes, Url.Length);

                    ProductId = store.ExtractUInt32();
                    Version = store.ExtractRawBytes(65) ?? new byte[65];
                    Key = store.ExtractRawBytes(20);
                    Username = store.ExtractRawBytes(22);
                    store.ExtractUInt8();

                    (uint, uint, uint) Questions = ChallengeHandler.GenerateClientQuestions(Encoding.ASCII.GetString(Version.Trim()));

                    initResponse.InsertStart((ushort)ProxyCrcList.FROM_PROXY_HEADER);
                    initResponse.InsertUInt8(1); // Host to Client - Initialize
                    initResponse.InsertUInt8(0);
                    initResponse.InsertUInt32(InternetProtocolUtils.GetIPAddressAsUInt(endpoint.Address.MapToIPv4()));
                    initResponse.InsertUInt16((ushort)endpoint.Port);
                    initResponse.InsertUInt16(0);
                    initResponse.InsertUInt32(InternetProtocolUtils.GetIPAddressAsUInt(EdenServerConfiguration.ProxyServerAddress));
                    initResponse.InsertUInt16(EdenServerConfiguration.ProxyServerPort);
                    initResponse.InsertUInt16(0);
                    initResponse.InsertUInt32(ProductId);
                    initResponse.InsertRawBytes(Version, (ushort)Version.Length);
                    initResponse.InsertUInt8(0);
                    initResponse.InsertUInt8(0);
                    initResponse.InsertUInt8(0);
                    initResponse.InsertUInt32(Questions.Item1);
                    initResponse.InsertUInt32(Questions.Item2);
                    initResponse.InsertUInt32(Questions.Item3);
                    initResponse.InsertRawBytes(UrlBytes, (ushort)UrlBytes.Length);
                    initResponse.InsertEnd();

                    ResultBuffer = initResponse.Data;
                    try
                    {
                        return server.listener?.Send(ResultBuffer, ResultBuffer.Length, endpoint) != -1;
                    }
                    catch
                    {
                    }
                    break;
                case 2: // Client to Host - Init Answer
                    UrlBytes = new byte[512];
                    EdStore init2Response = new EdStore(null, 1400);

                    Array.Copy(Encoding.ASCII.GetBytes(Url), UrlBytes, Url.Length);

                    ProductId = store.ExtractUInt32();
                    Version = store.ExtractRawBytes(65);
                    string VersionStr = Version == null ? string.Empty : Encoding.ASCII.GetString(Version.Trim());
                    store.ExtractUInt8();
                    store.ExtractUInt8();
                    store.ExtractUInt8();
                    Question1 = store.ExtractUInt32();
                    Question2 = store.ExtractUInt32();
                    Question3 = store.ExtractUInt32();
                    Answer1 = store.ExtractUInt32();
                    Answer2 = store.ExtractUInt32();
                    Answer3 = store.ExtractUInt32();
                    Key = store.ExtractRawBytes(20);
                    Username = store.ExtractRawBytes(22);
                    string UsernameStr = Username == null ? string.Empty : Encoding.ASCII.GetString(Username.Trim());
                    store.ExtractUInt8(); // Unk
                    store.ExtractUInt8(); // Unk

                    client = new ClientObject(server.listener, EdenServerConfiguration.EnableEncryption)
                    {
                        Question1 = Question1, Question2 = Question2, Question3 = Question3,
                        Answer1 = Answer1, Answer2 = Answer2, Answer3 = Answer3,
                        Key = Key,
                        Username = UsernameStr,
                        Url = UrlBytes,
                        IP = endpoint.Address.MapToIPv4(),
                        Port = (ushort)endpoint.Port,
                        Version = VersionStr,
                    };

                    init2Response.InsertStart((ushort)ProxyCrcList.FROM_PROXY_HEADER);
                    init2Response.InsertUInt8(3); // Host to Client - Init Answer
                    init2Response.InsertUInt8(0);
                    init2Response.InsertUInt32(client.Id);
                    init2Response.InsertUInt32(InternetProtocolUtils.GetIPAddressAsUInt(EdenServerConfiguration.ORBServerAddress));

                    if (ChallengeHandler.GenerateClientChallenge(VersionStr, client))
                    {
                        bool IsProxyBusy = false;

                        if (server.ClientStore == null || !server.ClientStore.AddClient(client))
                        {
                            LoggerAccessor.LogWarn($"[EDEN_PROXY_SERVER] - ToProxy - Failure adding ClientObject on IpEndPoint:{endpoint}.");
                            IsProxyBusy = true;
                        }
                        else
                            LoggerAccessor.LogInfo($"[EDEN_PROXY_SERVER] - ToProxy - Adding ClientObject for User:{client.Username} on IpEndPoint:{endpoint}.");

                        if (string.IsNullOrEmpty(VersionStr) || !allowedVersions.ContainsKey(VersionStr))
                        {
                            LoggerAccessor.LogWarn($"[EDEN_PROXY_SERVER] - ToProxy - TDU Version:{VersionStr} is not supported yet, please report to GITHUB!");

                            init2Response.InsertUInt16(0);
                            init2Response.InsertUInt8(0);
                            init2Response.InsertUInt8((byte)ProxyErrorCodes.WRONG_VERSION);
                        }
                        else
                        {
                            uint fishKey = client.Answer1 ^ client.Answer2 ^ client.Answer3 ^ client.Question1;

                            // Check config for PS3/360 Consoles encryption mode (CELL and XENON are big endian).
                            if (allowedVersions[VersionStr])
                                client.CPUEndianness = Endianness.LittleEndian;
                            else if (EdenServerConfiguration.BigEndianEncryption)
                                client.CPUEndianness = Endianness.BigEndian;
#if DEBUG
                            LoggerAccessor.LogInfo($"[EDEN_PROXY_SERVER] - ToProxy - User:{client.Username} has blowfish Key:0x{fishKey:X8} set.");
#endif
                            client.fish.SetKey(Encoding.ASCII.GetBytes(fishKey.ToString("X8")), client.CPUEndianness);

                            init2Response.InsertUInt16(EdenServerConfiguration.ORBServerPort);
                            init2Response.InsertUInt8((byte)(client.BCipher ? 0x1 : 0x0));
                            init2Response.InsertUInt8((byte)(IsProxyBusy ? ProxyErrorCodes.PROXY_BUSY : 0x0));
                        }
                    }
                    else
                    {
                        init2Response.InsertUInt16(0);
                        init2Response.InsertUInt8(0);
                        init2Response.InsertUInt8((byte)ProxyErrorCodes.UNK_1);
                    }

                    init2Response.InsertRawBytes(UrlBytes, (ushort)UrlBytes.Length);

                    ResultBuffer = init2Response.TrimmedData;
                    try
                    {
                        return server.listener?.Send(ResultBuffer, ResultBuffer.Length, endpoint) != -1;
                    }
                    catch
                    {
                    }
                    break;
                case 4: // Client To ORB
                    TargetIp = store.ExtractUInt32();
                    TargetPort = store.ExtractUInt16();
                    store.ExtractUInt16();
                    ClientId = store.ExtractUInt32();
                    PayloadSize = store.ExtractUInt16();
                    store.ExtractUInt16();
                    Payload = store.ExtractBlowfishBytes(PayloadSize);

                    client = server.ClientStore?.GetClientById(ClientId);

                    if (client == null)
                        LoggerAccessor.LogWarn($"[EDEN_PROXY_SERVER] - ToProxy - IpEndPoint:{endpoint} requested non-existant ClientObject with Id:{ClientId}, aborting request...");
                    else
                    {
#if DEBUG
                        LoggerAccessor.LogInfo($"[EDEN_PROXY_SERVER] - ToProxy - User:{client.Username} requested Client To ORB method at:{DateTime.Now}.");
#endif
                        client.lastRequestTime = DateTimeUtils.GetHighPrecisionUtcTime();

                        if (Payload == null)
                        {
                            EdStore validateResponse = new EdStore(null, 1400);

                            validateResponse.InsertStart((ushort)ProxyCrcList.FROM_PROXY_HEADER);
                            validateResponse.InsertUInt8(5); // ORB To Client
                            validateResponse.InsertUInt8(0);
                            validateResponse.InsertUInt32(TargetIp);
                            validateResponse.InsertUInt16(TargetPort);
                            validateResponse.InsertUInt16(0);
                            validateResponse.InsertUInt32(ClientId);
                            validateResponse.InsertUInt16(0);
                            validateResponse.InsertUInt16(0);
                            validateResponse.InsertEnd();

                            ResultBuffer = validateResponse.TrimmedData;
                        }
                        else if (double.TryParse(Encoding.UTF8.GetString(Payload), out _))
                        {
                            EdStore validateResponse = new EdStore(null, 1400);

                            validateResponse.InsertStart((ushort)ProxyCrcList.FROM_PROXY_HEADER);
                            validateResponse.InsertUInt8(5); // ORB To Client
                            validateResponse.InsertUInt8(0);
                            validateResponse.InsertUInt32(TargetIp);
                            validateResponse.InsertUInt16(TargetPort);
                            validateResponse.InsertUInt16(0);
                            validateResponse.InsertUInt32(ClientId);
                            validateResponse.InsertUInt16(PayloadSize);
                            validateResponse.InsertUInt16(0);
                            validateResponse.InsertRawBytes(Payload, PayloadSize);
                            validateResponse.InsertEnd();

                            ResultBuffer = validateResponse.TrimmedData;
                        }
                        else
                        {
                            if (client.BCipher)
                                client.DecipherData(Payload);
#if DEBUG
                            LoggerAccessor.LogInfo($"[EDEN_PROXY_SERVER] - ToProxy - User:{client.Username} Client To ORB Payload : {{{Payload.ToHexString().Replace("\n", string.Empty)}}}");
#endif
                            EdStore orbRequest = new EdStore(Payload, Payload.Length);
                            ushort orbcrc = orbRequest.ExtractStart();

                            ushort orbMagicCrc;
                            ushort orbReqPayloadSize;
                            byte[] orbPayload;
                            ClientTask orbTask;

                            switch (orbcrc)
                            {
                                case (ushort)ProxyCrcList.CRC_R_CALLACTION:

                                    orbTask = client.AddTask(TargetIp, TargetPort);

                                    orbTask.SequenceId = orbRequest.ExtractUInt32();
                                    orbTask.TimeOut = orbRequest.ExtractUInt32();
                                    orbTask.RetryCount = ClientObject.DefaultRetryCount;
                                    orbReqPayloadSize = orbRequest.ExtractUInt16();
                                    orbPayload = orbRequest.ExtractRawBytes(orbReqPayloadSize);
                                    orbMagicCrc = EndianAwareConverter.ToUInt16(orbPayload, Endianness.BigEndian, 0);
#if DEBUG
                                    LoggerAccessor.LogInfo($"[EDEN_PROXY_SERVER] - ToProxy - User:{client.Username} Requested ORB Magic {orbMagicCrc:X4} : {{{orbPayload.ToHexString().Replace("\n", string.Empty)}}}");
#else
                                    LoggerAccessor.LogInfo($"[EDEN_PROXY_SERVER] - ToProxy - User:{client.Username} Requested ORB Magic {orbMagicCrc:X4}");
#endif
                                    if (!ORBCrcToClass.TryGetValue(orbMagicCrc, out Type? c))
                                    {
                                        LoggerAccessor.LogError($"[EDEN_PROXY_SERVER] - ToProxy - User:{client.Username} Requested an unexpected ORB message Type {orbMagicCrc:X4} : SizeOfPacket:{orbPayload.Length}");
                                        return false;
                                    }

                                    orbTask.Request = new EdStore(orbPayload, orbPayload.Length);

                                    AbstractProxyMessage? msg = null;

                                    try
                                    {
                                        if (c != null)
                                            msg = (AbstractProxyMessage?)Activator.CreateInstance(c);
                                    }
                                    catch
                                    {
                                    }

                                    ResultBuffer = msg?.Process(endpoint, new IPEndPoint(TargetIp, TargetPort), orbTask, orbTask.Request.ExtractUInt16());

                                    break;
                                case (ushort)ProxyCrcList.COREREQUEST_CONTAINER:

                                    orbTask = client.AddTask(TargetIp, TargetPort);

                                    orbTask.TimeOut = ClientObject.DefaultTimeOut;
                                    orbTask.RetryCount = ClientObject.DefaultRetryCount;
                                    orbReqPayloadSize = orbRequest.ExtractUInt16();
                                    orbPayload = orbRequest.ExtractRawBytes(orbReqPayloadSize);
                                    orbMagicCrc = EndianAwareConverter.ToUInt16(orbPayload, Endianness.BigEndian, 0);
#if DEBUG
                                    LoggerAccessor.LogInfo($"[EDEN_PROXY_SERVER] - ToProxy - User:{client.Username} Requested ORB Magic {orbMagicCrc:X4} : {{{orbPayload.ToHexString().Replace("\n", string.Empty)}}}");
#else
                                    LoggerAccessor.LogInfo($"[EDEN_PROXY_SERVER] - ToProxy - User:{client.Username} Requested ORB Magic {orbMagicCrc:X4}");
#endif
                                    if (!ORBCrcToClass.TryGetValue(orbMagicCrc, out Type? unreliable_c))
                                    {
                                        LoggerAccessor.LogError($"[EDEN_PROXY_SERVER] - ToProxy - User:{client.Username} Requested an unexpected ORB message Type {orbMagicCrc:X4} : SizeOfPacket:{orbPayload.Length}");
                                        return false;
                                    }

                                    orbTask.Request = new EdStore(orbPayload, orbPayload.Length);

                                    AbstractProxyMessage? unreliable_msg = null;

                                    try
                                    {
                                        if (unreliable_c != null)
                                            unreliable_msg = (AbstractProxyMessage?)Activator.CreateInstance(unreliable_c);
                                    }
                                    catch
                                    {
                                    }

                                    ResultBuffer = unreliable_msg?.Process(endpoint, new IPEndPoint(TargetIp, TargetPort), orbTask, orbTask.Request.ExtractUInt16());

                                    break;
                                case (ushort)ProxyCrcList.CLIENT_ORB_KEEP_ALIVE:

                                    orbTask = client.AddTask(TargetIp, TargetPort);

                                    orbTask.TimeOut = ClientObject.DefaultTimeOut;
                                    orbTask.RetryCount = ClientObject.DefaultRetryCount;

                                    orbTask.Request = orbRequest;

                                    ResultBuffer = new OrbKeepAlive().Process(endpoint, new IPEndPoint(TargetIp, TargetPort), orbTask, orbcrc);

                                    break;
                                case edStoreBank.CRC_R_GET_EDNETBUFFER:

                                    orbTask = client.AddTask(TargetIp, TargetPort);

                                    orbTask.TimeOut = ClientObject.DefaultTimeOut;
                                    orbTask.RetryCount = ClientObject.DefaultRetryCount;

                                    orbTask.Request = orbRequest;

                                    ResultBuffer = new GetEdnetBuffer().Process(endpoint, new IPEndPoint(TargetIp, TargetPort), orbTask, orbcrc);

                                    break;
                                case edStoreBank.CRC_E_GET_EDNETBUFFER_ACK:
                                    LoggerAccessor.LogInfo($"[EDEN_PROXY_SERVER] - ToProxy - EdNetBuffer with id:{orbRequest.ExtractUInt32()} Acknowledged");
                                    break;
                                default:
                                    LoggerAccessor.LogError($"[EDEN_PROXY_SERVER] - ToProxy - User:{client.Username} Requested an unexpected ORB CRC {orbcrc:X4}");
                                    break;
                            }
                        }

                        if (ResultBuffer != null)
                        {
                            try
                            {
                                return server.listener?.Send(ResultBuffer, ResultBuffer.Length, endpoint) != -1;
                            }
                            catch
                            {
                            }
                        }
                    }
                    break;
                default:
                    LoggerAccessor.LogWarn($"[ToProxy] - Unknown Command:{Command} Requested, aborting request...");
                    break;
            }

            return false;
        }
    }
}
