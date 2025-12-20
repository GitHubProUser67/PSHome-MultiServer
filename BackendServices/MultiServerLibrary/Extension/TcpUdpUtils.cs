using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace MultiServerLibrary.Extension
{
    public static class TcpUdpUtils
    {
        private const string netDll = "Iphlpapi.dll";

        [DllImport(netDll, SetLastError = true)]
        private static extern uint GetTcpTable(IntPtr pTcpTable, ref uint dwOutBufLen, bool order);

        [DllImport(netDll, SetLastError = true)]
        private static extern uint GetUdpTable(IntPtr pUdpTable, ref uint dwOutBufLen, bool order);


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct MibTcpTable
        {
            internal uint numberOfEntries;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct MibTcpRow
        {
            internal uint state;
            internal uint localAddr;
            internal byte localPort1;
            internal byte localPort2;
            internal byte localPort3;
            internal byte localPort4;
            internal uint remoteAddr;
            internal byte remotePort1;
            internal byte remotePort2;
            internal byte remotePort3;
            internal byte remotePort4;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MibUdpTable
        {
            internal uint numberOfEntries;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MibUdpRow
        {
            internal uint localAddr;
            internal byte localPort1;
            internal byte localPort2;
            internal byte localPort3;
            internal byte localPort4;
        }

        /// <summary>
        /// Get the Windows TCP Table.
        /// <para>Obtiens la table TCP de Windows.</para>
        /// </summary>
        /// <returns>A array of int.</returns>
        private static int[] GetWindowsTcpTable()
        {
            int[] ports = Array.Empty<int>();
            uint dwOutBufLen = 0;
            IntPtr pTcpTable = IntPtr.Zero;

            uint result = GetTcpTable(IntPtr.Zero, ref dwOutBufLen, true);

            if (result == 0x7a) // ERROR_INSUFFICIENT_BUFFER
            {
                try
                {
                    pTcpTable = Marshal.AllocHGlobal((int)dwOutBufLen);
                    result = GetTcpTable(pTcpTable, ref dwOutBufLen, true);

                    if (result == 0)
                    {
                        IntPtr handle = pTcpTable;
                        MibTcpTable table = (MibTcpTable)Marshal.PtrToStructure(handle, typeof(MibTcpTable));

                        if (table.numberOfEntries > 0)
                        {
                            ports = new int[table.numberOfEntries];
                            handle = (IntPtr)((long)handle + Marshal.SizeOf(table.numberOfEntries));

                            for (int i = 0; i < table.numberOfEntries; i++)
                            {
                                MibTcpRow row = (MibTcpRow)Marshal.PtrToStructure(handle, typeof(MibTcpRow));
                                ports[i] = row.localPort3 << 24 | row.localPort4 << 16 | row.localPort1 << 8 | row.localPort2;
                                handle = (IntPtr)((long)handle + Marshal.SizeOf(row));
                            }
                        }
                    }
                }
                finally
                {
                    if (pTcpTable != IntPtr.Zero)
                        Marshal.FreeHGlobal(pTcpTable);
                }
            }
            return ports;
        }

        /// <summary>
        /// Get the Windows UDP Table.
        /// <para>Obtiens la table UDP de Windows.</para>
        /// </summary>
        /// <returns>A array of int.</returns>
        private static int[] GetWindowsUdpTable()
        {
            int[] ports = Array.Empty<int>();
            uint dwOutBufLen = 0;
            IntPtr pUdpTable = IntPtr.Zero;

            uint result = GetUdpTable(IntPtr.Zero, ref dwOutBufLen, true);

            if (result == 0x7A) // ERROR_INSUFFICIENT_BUFFER
            {
                try
                {
                    pUdpTable = Marshal.AllocHGlobal((int)dwOutBufLen);
                    result = GetUdpTable(pUdpTable, ref dwOutBufLen, true);

                    if (result == 0)
                    {
                        IntPtr handle = pUdpTable;
                        MibUdpTable table = (MibUdpTable)Marshal.PtrToStructure(handle, typeof(MibUdpTable));

                        if (table.numberOfEntries > 0)
                        {
                            ports = new int[table.numberOfEntries];
                            handle = (IntPtr)((long)handle + Marshal.SizeOf(table.numberOfEntries));

                            for (int i = 0; i < table.numberOfEntries; i++)
                            {
                                MibUdpRow row = (MibUdpRow)Marshal.PtrToStructure(handle, typeof(MibUdpRow));
                                ports[i] = row.localPort3 << 24 | row.localPort4 << 16 | row.localPort1 << 8 | row.localPort2;
                                handle = (IntPtr)((long)handle + Marshal.SizeOf(row));
                            }
                        }
                    }
                }
                finally
                {
                    if (pUdpTable != IntPtr.Zero)
                        Marshal.FreeHGlobal(pUdpTable);
                }
            }

            return ports;
        }


        /// <summary>
        /// Get the next available TCP port on the system (Windows only).
        /// <para>(Seulement sur Windows) Obtiens le prochain port TCP disponible.</para>
        /// </summary>
        /// <param name="sourceport">The initial port to start with.</param>
        /// <param name="attemptcount">Maximum number of tries.</param>
        /// <returns>A int.</returns>
        public static int GetNextVacantTCPPort(int sourceport, uint attemptcount)
        {
            if (Microsoft.Win32API.IsWindows)
            {
                if (attemptcount == 0)
                    throw new ArgumentOutOfRangeException("attemptcount");

                foreach (int port in GetWindowsTcpTable())
                {
                    if (sourceport == port)
                    {
                        sourceport ++;
                        attemptcount --;

                        if (sourceport >= ushort.MaxValue && attemptcount > 0)
                            sourceport = 1;
                        else if (sourceport >= ushort.MaxValue && attemptcount == 0)
                            return -1;

                        return GetNextVacantTCPPort(sourceport, attemptcount);
                    }
                }

                return sourceport;
            }

            throw new PlatformNotSupportedException("[TcpUdpUtils] - GetNextVacantTCPPort is only supported on Windows.");
        }

        /// <summary>
        /// Get the next available UDP port on the system (Windows only).
        /// <para>(Seulement sur Windows) Obtiens le prochain port UDP disponible.</para>
        /// </summary>
        /// <param name="sourceport">The initial port to start with.</param>
        /// <param name="attemptcount">Maximum number of tries.</param>
        /// <returns>A int.</returns>
        public static int GetNextVacantUDPPort(int sourceport, uint attemptcount)
        {
            if (!Microsoft.Win32API.IsWindows)
                throw new PlatformNotSupportedException("[TcpUdpUtils] - GetNextVacantUDPPort is only supported on Windows.");

            if (attemptcount == 0)
                throw new ArgumentOutOfRangeException(nameof(attemptcount));

            foreach (int port in GetWindowsUdpTable())
            {
                if (sourceport == port)
                {
                    sourceport++;
                    attemptcount--;

                    if (sourceport >= ushort.MaxValue && attemptcount > 0)
                        sourceport = 1;
                    else if (sourceport >= ushort.MaxValue)
                        return -1;

                    return GetNextVacantUDPPort(sourceport, attemptcount);
                }
            }

            return sourceport;
        }


        /// <summary>
        /// Know if the given TCP port is available.
        /// <para>Savoir si le port TCP en question est disponible.</para>
        /// </summary>
        /// <param name="port">The port on which we scan.</param>
        /// <returns>A boolean.</returns>
        public static bool IsTCPPortAvailable(int port)
        {
#if DEBUG
            CustomLogger.LoggerAccessor.LogInfo("[TcpUdpUtils] - Checking TCP Port {0}", port);
#endif
            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            bool isAvailable = !IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(p => p.Port == port);
#if DEBUG
            CustomLogger.LoggerAccessor.LogInfo("[TcpUdpUtils] - TCP Port {0} available = {1}", port, isAvailable);
#endif
            return isAvailable;
        }

        /// <summary>
        /// Know if the given UDP port is available.
        /// <para>Savoir si le port UDP en question est disponible.</para>
        /// </summary>
        /// <param name="port">The port on which we scan.</param>
        /// <returns>A boolean.</returns>
        public static bool IsUDPPortAvailable(int port)
        {
#if DEBUG
            CustomLogger.LoggerAccessor.LogInfo("[TcpUdpUtils] - Checking UDP Port {0}", port);
#endif
            // Evaluate current system udp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our UdpClient is occupied, we will set isAvailable to false.
            bool isAvailable = !IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners().Any(p => p.Port == port);
#if DEBUG
            CustomLogger.LoggerAccessor.LogInfo("[TcpUdpUtils] - UDP Port {0} available = {1}", port, isAvailable);
#endif
            return isAvailable;
        }
    }
}
