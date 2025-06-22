using CustomLogger;
using EdNetService.Models;
using NetworkLibrary.Extension;
using System.Collections.Concurrent;
using System.Net;

namespace EdenServer.EdNet
{
    internal class ClientStore
    {
        private readonly ConcurrentDictionary<uint, ClientObject> Clients = new ConcurrentDictionary<uint, ClientObject>();

        private Thread? TickThread;

        private volatile bool threadActive;

        public ClientStore() { }

        private async void RunLoop()
        {
            while (threadActive)
            {
                foreach (var client in Clients.Values)
                {
                    if ((DateTimeUtils.GetHighPrecisionUtcTime() - client.lastRequestTime).TotalSeconds > EdenServerConfiguration.ClientLongTimeoutSeconds)
                    {
                        foreach (var task in client.Tasks)
                        {
                            task.Disconnect();
                        }
                        Clients.TryRemove(client.Id, out _);
                        LoggerAccessor.LogWarn($"[ClientStore] - Client:{client} was removed from the client store.");
                    }
                    else
                        client.RefreshClient();
                }

                await Task.Delay(1).ConfigureAwait(false);
            }

            foreach (var client in Clients.Values)
            {
                foreach (var task in client.Tasks)
                {
                    task.Disconnect();
                }
                Clients.TryRemove(client.Id, out _);
            }
        }

        public void Start()
        {
            if (TickThread != null)
            {
                LoggerAccessor.LogWarn("[ClientStore] - ClientStore already active.");
                return;
            }
            TickThread = new Thread(RunLoop);
            TickThread.Start();
            threadActive = true;
        }

        public void Stop()
        {
            threadActive = false;

            // wait for thread to finish
            if (TickThread != null)
            {
                TickThread.Join();
                TickThread = null;
            }
        }

        public bool AddClient(ClientObject client)
        {
            return Clients.TryAdd(client.Id, client);
        }

        public bool RemoveClient(ClientObject client)
        {
            var keyValuePair = Clients.FirstOrDefault(kvp => kvp.Value == client);

            // If a matching client was found, remove it by its key
            if (!default(KeyValuePair<uint, ClientObject>).Equals(keyValuePair))
                return Clients.TryRemove(keyValuePair.Key, out _);

            // Client not found
            return false;
        }

        public bool RemoveClientByID(uint ID)
        {
            return Clients.TryRemove(ID, out _);
        }

        public ClientObject? GetClientById(uint id)
        {
            if (Clients.ContainsKey(id))
                return Clients[id];

            return null;
        }

        public ClientObject? GetClientByIp(IPAddress IP)
        {
            var keyValuePair = Clients.FirstOrDefault(kvp => kvp.Value.IP == IP);

            // If a matching client was found, remove it by its key
            if (!default(KeyValuePair<uint, ClientObject>).Equals(keyValuePair))
                return keyValuePair.Value;

            // Client not found
            return null;
        }

        public uint GeIdByClient(ClientObject client)
        {
            return Clients.FirstOrDefault(kvp => kvp.Value == client).Key;
        }
    }
}
