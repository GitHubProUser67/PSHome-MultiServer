using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using EdNetService.CRC;
using EdNetService.Crypto;
using EndianTools;
using MultiServerLibrary.Extension;

namespace EdNetService.Models
{
    [Flags]
    public enum ClientState
    {
        Default = 0,
        ReadyToSend = 1,
        Sending = 2,
        Sent = 3,
        Error = 4,
        Destroyed = 5,
        TimedOut = 6
    }

    [Flags]
    public enum ClientMode
    {
        None = 0,
        Server = 1,
        ProxyServer = 2,
        ProxyServerRaw = 3,
        Client = 4
    }

    public class ClientTask
    {
        public uint TargetIp;
        public ushort TargetPort;
        public uint SequenceId = 0;
        public uint ReliableId = 0;
        public uint ReliableBufferSize = 0;

        public uint TimeOut;
        public uint RetryCount;

        public ClientObject Client;

        protected DateTime _lastBufferEmited;

        protected EdStore _request = null;
        protected EdStore _response = null;

        protected RequestMode _mode = RequestMode.Reliable;
        protected ClientState _state = ClientState.Default;

        protected IPEndPoint _target = new IPEndPoint(IPAddress.Any, byte.MinValue);

        protected ClientMode _clientMode;

        public ClientTask(ClientObject client, uint targetIp, ushort targetPort)
        {
            Client = client;
            TargetIp = targetIp;
            TargetPort = targetPort;
        }

        public EdStore Request
        {
            get
            {
                return _request;
            }
            set
            {
                _request = value;
            }
        }

        public EdStore Response
        {
            get
            {
                return _response;
            }
            set
            {
                _response = value;
                _state = ClientState.Default;
            }
        }

        public RequestMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }

        public IPEndPoint Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }

        public ClientMode ClientMode
        {
            get
            {
                return _clientMode;
            }
            set
            {
                _clientMode = value;
                _state |= ClientState.ReadyToSend;
            }
        }

        public ClientState State
        {
            get
            {
                return _state;
            }
        }

        public bool Completed
        {
            get
            {
                return Error || (_state & ClientState.Sent) != ClientState.Default || (_state & ClientState.Destroyed) != ClientState.Default;
            }
        }

        public bool Error
        {
            get
            {
                return (_state & ClientState.Error) != ClientState.Default;
            }
        }

        public void Disconnect()
        {
            _state = ClientState.Destroyed;
        }

        public bool SendRequest(IPEndPoint target)
        {
            Client.LastPacketSent = DateTimeUtils.GetHighPrecisionUtcTime();
            _lastBufferEmited = DateTimeUtils.GetHighPrecisionUtcTime();

            EdStore store = new EdStore(null, 1400);

            _state = ClientState.Sending;

            if (_mode == RequestMode.Reliable)
            {
                store.InsertStart((ushort)ProxyCrcList.CRC_R_CALLACTION);
                store.InsertUInt32(SequenceId);
                store.InsertUInt32(TimeOut);
                store.InsertDataStore(_request);
                store.InsertEnd();
            }
            else
            {
                store.InsertStart((ushort)ProxyCrcList.COREREQUEST_CONTAINER);
                store.InsertDataStore(_request);
                store.InsertEnd();
            }

            return Client.Send(target, store);
        }

        public bool SendResponse(IPEndPoint target)
        {
            Client.LastPacketSent = DateTimeUtils.GetHighPrecisionUtcTime();
            _lastBufferEmited = DateTimeUtils.GetHighPrecisionUtcTime();

            bool result = true;

            _state = ClientState.Sending;

            if (_response != null && _response.CurrentSize != 0)
            {
                EdStore store = new EdStore(null, 1400);
                if (SequenceId != 0U)
                {
                    if (ReliableId != 0U)
                    {
                        store.InsertStart((ushort)ProxyCrcList.NETBUFFER_RESPONSE);
                        store.InsertUInt32(SequenceId);
                        store.InsertUInt32(ReliableId);
                        store.InsertUInt32(ReliableBufferSize);
                    }
                    else
                    {
                        store.InsertStart((ushort)ProxyCrcList.CRC_A_CALLACTION);
                        store.InsertUInt32(SequenceId);
                    }
                }
                else
                    store.InsertStart((ushort)ProxyCrcList.COREREQUEST_CONTAINER);
                if (_response.CurrentSize > 0)
                    store.InsertDataStore(_response);
                store.InsertEnd();
                result = Client.Send(target, store);
            }

            return result;
        }

        public bool SendProxyResponse(IPEndPoint target)
        {
            Client.LastPacketSent = DateTimeUtils.GetHighPrecisionUtcTime();
            _lastBufferEmited = DateTimeUtils.GetHighPrecisionUtcTime();

            bool result = true;

            _state = ClientState.Sending;

            if (_response != null && _response.CurrentSize != 0)
            {
                EdStore fromProxyStore = new EdStore(null, 1400);
                EdStore store = new EdStore(null, 1400);
                fromProxyStore.InsertStart((ushort)ProxyCrcList.FROM_PROXY_HEADER);
                fromProxyStore.InsertUInt8(5); // ORB To Client
                fromProxyStore.InsertUInt8(0);
                fromProxyStore.InsertUInt32(TargetIp);
                fromProxyStore.InsertUInt16(TargetPort);
                fromProxyStore.InsertUInt16(0);
                fromProxyStore.InsertUInt32(Client.Id);
                if (SequenceId != 0U)
                {
                    if (ReliableId != 0U)
                    {
                        store.InsertStart((ushort)ProxyCrcList.NETBUFFER_RESPONSE);
                        store.InsertUInt32(SequenceId);
                        store.InsertUInt32(ReliableId);
                        store.InsertUInt32(ReliableBufferSize);
                    }
                    else
                    {
                        store.InsertStart((ushort)ProxyCrcList.CRC_A_CALLACTION);
                        store.InsertUInt32(SequenceId);
                    }
                }
                else
                    store.InsertStart((ushort)ProxyCrcList.COREREQUEST_CONTAINER);
                if (_response.CurrentSize > 0)
                    store.InsertDataStore(_response);
                store.InsertEnd();
                ushort responseSize = (ushort)store.CurrentSize;
                fromProxyStore.InsertUInt16(responseSize);
                fromProxyStore.InsertUInt16(0);
                if (Client.BCipher)
                {
                    ushort length = (ushort)(responseSize + (Blowfish.BlockSize - (responseSize % Blowfish.BlockSize)));
                    byte[] payloadToEncrypt = new byte[length];
                    Array.Copy(store.Data, 0, payloadToEncrypt, 0, responseSize);
                    Client.EncipherData(payloadToEncrypt);
                    fromProxyStore.InsertRawBytes(payloadToEncrypt, (ushort)payloadToEncrypt.Length);
                }
                else
                    fromProxyStore.InsertRawBytes(store.Data, responseSize);
                fromProxyStore.InsertEnd();
                result = Client.Send(target, fromProxyStore);
            }

            return result;
        }

        public bool SendProxyRawResponse(IPEndPoint target)
        {
            Client.LastPacketSent = DateTimeUtils.GetHighPrecisionUtcTime();
            _lastBufferEmited = DateTimeUtils.GetHighPrecisionUtcTime();

            bool result = true;

            _state = ClientState.Sending;

            if (_response != null && _response.CurrentSize != 0)
            {
                EdStore fromProxyStore = new EdStore(null, 1400);
                fromProxyStore.InsertStart((ushort)ProxyCrcList.FROM_PROXY_HEADER);
                fromProxyStore.InsertUInt8(5); // ORB To Client
                fromProxyStore.InsertUInt8(0);
                fromProxyStore.InsertUInt32(TargetIp);
                fromProxyStore.InsertUInt16(TargetPort);
                fromProxyStore.InsertUInt16(0);
                fromProxyStore.InsertUInt32(Client.Id);
                ushort responseSize = (ushort)_response.CurrentSize;
                fromProxyStore.InsertUInt16(responseSize);
                fromProxyStore.InsertUInt16(0);
                if (Client.BCipher)
                {
                    ushort length = (ushort)(responseSize + (Blowfish.BlockSize - (responseSize % Blowfish.BlockSize)));
                    byte[] payloadToEncrypt = new byte[length];
                    Array.Copy(_response.Data, 0, payloadToEncrypt, 0, responseSize);
                    Client.EncipherData(payloadToEncrypt);
                    fromProxyStore.InsertRawBytes(payloadToEncrypt, (ushort)payloadToEncrypt.Length);
                }
                else
                    fromProxyStore.InsertRawBytes(_response.Data, responseSize);
                fromProxyStore.InsertEnd();
                result = Client.Send(target, fromProxyStore);
            }

            return result;
        }

        public void RefreshTask()
        {
            if (!InternetProtocolUtils.IsZeroIpv4Address(_target.Address) && _target.Port != 0)
            {
                if (((_state & (ClientState.Sending | ClientState.ReadyToSend)) != ClientState.Default) && (DateTimeUtils.GetHighPrecisionUtcTime() - _lastBufferEmited).TotalMilliseconds > TimeOut)
                {
                    if (RetryCount > 0U)
                    {
                        RetryCount -= 1U;
                        if (_clientMode == ClientMode.ProxyServer)
                        {
                            if (SendProxyResponse(_target))
                                _state = ClientState.Sent;
                        }
                        else if (_clientMode == ClientMode.ProxyServerRaw)
                        {
                            if (SendProxyRawResponse(_target))
                                _state = ClientState.Sent;
                        }
                        else if (_clientMode == ClientMode.Server)
                        {
                            if (SendResponse(_target))
                                _state = ClientState.Sent;
                        }
                        else if (_clientMode == ClientMode.Client)
                        {
                            if (SendRequest(_target))
                                _state = ClientState.Sent;
                        }
                        else
                            _state = ClientState.Error;
                    }
                    else
                        _state = ClientState.Error | ClientState.TimedOut;
                }
            }
        }
    }

    public class ClientObject
    {
        private static readonly UniqueIDGenerator _IdCounter = new UniqueIDGenerator();
        private static readonly UniqueIDGenerator _reliableIdCounter = new UniqueIDGenerator();

        public const byte DefaultRetryCount = 3;
        public const uint DefaultTimeOut = 1000U;

        public readonly ConcurrentList<ClientTask> Tasks = new ConcurrentList<ClientTask>();

        public readonly bool BCipher;

        public Endianness CPUEndianness = Endianness.LittleEndian;
        public uint Id;
        public uint Answer1;
        public uint Answer2;
        public uint Answer3;
        public uint Question1;
        public uint Question2;
        public uint Question3;
        public uint PendingFileUserId;
        public uint UserId;
        public ulong SessionId;

        public byte[] Key;
        public byte[] Url;

        public string Username;
        public string Version;

        public IPAddress IP;
        public ushort Port;

        public DateTime lastRequestTime = DateTimeUtils.GetHighPrecisionUtcTime();
        public DateTime LastPacketSent;

        public Blowfish fish = new Blowfish();

        protected UdpClient _client;

        public ClientObject(UdpClient client, bool cipher)
        {
            Id = _IdCounter.CreateUniqueID();
            _client = client;
            BCipher = cipher;
        }

        public ClientTask AddTask(uint TargetIp, ushort TargetPort)
        {
            ClientTask task = new ClientTask(this, TargetIp, TargetPort);
            Tasks.Add(task);
            return task;
        }

        public void RefreshClient()
        {
            List<ClientTask> tasksToRemove = new List<ClientTask>();

            foreach (var task in Tasks)
            {
                task.RefreshTask();
                if (task.Completed)
                    tasksToRemove.Add(task);
            }

            foreach (var task in tasksToRemove)
            {
                Tasks.Remove(task);
            }
        }

        public void DecipherData(byte[] array)
        {
            uint offset = 0;

            while (offset < array.Length / 8)
            {
                fish.Decipher(array, offset * 8U, CPUEndianness);
                offset += 1;
            }
        }

        public void EncipherData(byte[] array)
        {
            uint offset = 0;

            while (offset < array.Length / 8)
            {
                fish.Encipher(array, offset * 8U, CPUEndianness);
                offset += 1;
            }
        }

        public bool Send(IPEndPoint target, EdStore data)
        {
            try
            {
                if (_client.Send(data.Data, (int)data.CurrentSize, target) < 0)
                    return false;
                return true;
            }
            catch
            {
            }
            return false;
        }
    }
}
