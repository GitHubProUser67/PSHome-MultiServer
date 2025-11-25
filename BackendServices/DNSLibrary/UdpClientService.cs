/* From: https://github.com/colt-1/dns-over-https/blob/main/Dependencies/UdpClientService.cs

MIT License

Copyright (c) 2021 Spacedog Labs

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace DNSLibrary
{
    public class UdpClientService
    {
        public int SendTimeoutMs { get; }
        public int ReceiveTimeoutMs { get; }

        private int _currentDnsIndex = 0;

        private readonly Random _random = new Random();

        private readonly object _dnsLock = new object(); // lock for rotating providers

        private readonly string[] DNSServers = GetAvailableDNSServers();

        private string CurrentDnsServer
        {
            get
            {
                lock (_dnsLock)
                    return DNSServers[_currentDnsIndex];
            }
        }

        private readonly ConcurrentQueue<UdpClient> UdpClientQueue = new ConcurrentQueue<UdpClient>();

        public UdpClientService(int SendTimeoutMs, int ReceiveTimeoutMs, int MaxConcurrentListeners = 10)
        {
            this.SendTimeoutMs = SendTimeoutMs;
            this.ReceiveTimeoutMs = ReceiveTimeoutMs;
            AddToClientQueue(MaxConcurrentListeners);
        }

        public (bool, UdpClient) Dequeue(int maxRetries = 20)
        {
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                if (UdpClientQueue.TryDequeue(out UdpClient selectedClient))
                    return (true, selectedClient);
                Thread.Sleep(1); // sleep for a milisecond
            }

            return (false, null);
        }

        /// <summary>
        /// Returns the UDP client to the queue.
        /// If renewClient is true, the existing client is disposed and replaced with a new instance.
        /// </summary>
        public void ReturnToQueue(UdpClient udpClient, bool renewClient)
        {
            if (renewClient)
            {
                try
                {
                    udpClient?.Close();
                }
                catch { }

                UdpClientQueue.Enqueue(CreateNewUdpClient());
            }
            else
                UdpClientQueue.Enqueue(udpClient);
        }

        public void RotateDnsServer()
        {
            lock (_dnsLock)
            {
                int nextIndex;
                int sizeOfDNSServers = DNSServers.Length;
                do
                {
                    nextIndex = _random.Next(sizeOfDNSServers);
                } while (nextIndex == _currentDnsIndex && sizeOfDNSServers > 1); // ensure different server if possible

                _currentDnsIndex = nextIndex;

                CustomLogger.LoggerAccessor.LogWarn($"[UdpClientService] - Rotating DNS provider to {DNSServers[_currentDnsIndex]}.");
            }
        }

        // Returns available system DNS entries or defaults to google.
        private static string[] GetAvailableDNSServers()
        {
            string[] dnsServers = NetworkInterface.GetAllNetworkInterfaces()
                .Where(i => i.OperationalStatus == OperationalStatus.Up)
                .SelectMany(i => i.GetIPProperties()?.DnsAddresses ?? Enumerable.Empty<System.Net.IPAddress>())
                .Select(ip => ip.ToString())
                .Distinct() // remove duplicates
                .ToArray();

            if (dnsServers.Length == 0)
            {
                // Fallback to public DNS servers
                dnsServers = new string[]
                {
                    "8.8.8.8", // Google
                    "8.8.4.4", // Google secondary
                    "1.1.1.1", // Cloudflare
                    "1.0.0.1", // Cloudflare secondary
                    "9.9.9.9", // Quad9
                    "208.67.222.222" // OpenDNS
                };
            }

            return dnsServers;
        }


        private void AddToClientQueue(int MaxConcurrentListeners)
        {
            for (byte i = 0; i < MaxConcurrentListeners; i++)
            {
                UdpClientQueue.Enqueue(CreateNewUdpClient());
            }
        }

        private UdpClient CreateNewUdpClient()
        {
            UdpClient client = null;

            try
            {
                client = new UdpClient(CurrentDnsServer, 53);

                client.Client.SendTimeout = SendTimeoutMs;
                client.Client.ReceiveTimeout = ReceiveTimeoutMs;
            }
            catch (SocketException ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[UdpClientService] - A Socket Exception was thrown while creating UDP client, returning null state. (Exception:{ex})");
            }

            return client;
        }
    }
}
