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

using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace DNSLibrary
{
    public class UdpClientService
    {
        private ConcurrentQueue<UdpClient> UdpClientQueue = new ConcurrentQueue<UdpClient>();
        private IPAddress DNSServerIP = GetFirstAvailableDNSServer();
        private int MaxConcurrentListeners;

        public UdpClientService(int MaxConcurrentListeners = 10)
        {
            this.MaxConcurrentListeners = MaxConcurrentListeners;
            AddToClientQueue();
        }

        public UdpClient Dequeue()
        {
            UdpClient client = null;

            do
            {
                if (UdpClientQueue.TryDequeue(out UdpClient selectedClient))
                    client = selectedClient;
                else
                    Thread.Sleep(1); // sleep for a milisecond
            } while (client == null);

            return client;
        }

        public void ReturnToQueue(UdpClient udpClient)
        {
            UdpClientQueue.Enqueue(udpClient);
        }

        private static IPAddress GetFirstAvailableDNSServer()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(i => i.OperationalStatus == OperationalStatus.Up)
                .First().GetIPProperties().DnsAddresses.First();
        }

        private void AddToClientQueue()
        {
            if (UdpClientQueue.Count < MaxConcurrentListeners)
            {
                for (byte i = 0; i < MaxConcurrentListeners; i++)
                {
                    UdpClientQueue.Enqueue(new UdpClient(DNSServerIP.ToString(), 53));
                }
            }
        }
    }
}
