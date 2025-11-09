using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace MultiServerLibrary.Extension
{
    public static class TCPUtils
    {
        [DllImport("Iphlpapi.dll", SetLastError = true)]
        private static extern uint GetTcpTable(IntPtr pTcpTable, ref uint dwOutBufLen, bool order);

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
            uint num2 = GetTcpTable(IntPtr.Zero, ref dwOutBufLen, true);
            if (num2 == 0x7a)
            {
                try
                {
                    pTcpTable = Marshal.AllocHGlobal((int)dwOutBufLen);
                    num2 = GetTcpTable(pTcpTable, ref dwOutBufLen, true);
                    if (num2 == 0)
                    {
                        IntPtr handle = pTcpTable;
#pragma warning disable
                        MibTcpTable table = (MibTcpTable)Marshal.PtrToStructure(handle, typeof(MibTcpTable));
                        if (table.numberOfEntries > 0)
                        {
                            ports = new int[table.numberOfEntries];
                            handle = (IntPtr)((long)handle + Marshal.SizeOf(table.numberOfEntries));
                            for (int i = 0; i < table.numberOfEntries; i++)
                            {
                                MibTcpRow row = (MibTcpRow)Marshal.PtrToStructure(handle, typeof(MibTcpRow));
                                ports[i] = row.localPort3 << 0x18 | row.localPort4 << 0x10 | row.localPort1 << 8 | row.localPort2;
                                handle = (IntPtr)((long)handle + Marshal.SizeOf(row));
                            }
                        }
#pragma warning restore
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
        /// Get the next available port on the system (Windows only).
        /// <para>(Seulement sur Windows) Obtiens le prochain port disponible.</para>
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
                        sourceport += 1;
                        attemptcount -= 1;
                        if (sourceport >= 0xffff && attemptcount > 0)
                            sourceport = 1;
                        else if (sourceport >= 0xffff && attemptcount == 0)
                            return -1;
                        return GetNextVacantTCPPort(sourceport, attemptcount);
                    }
                }

                return sourceport;
            }

            throw new PlatformNotSupportedException("[TCPUtils] - GetNextVacantTCPPort is only supported on Windows.");
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
            CustomLogger.LoggerAccessor.LogInfo("[TCPUtils] - Checking Port {0}", port);
#endif
            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            bool isAvailable = !IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(p => p.Port == port);
#if DEBUG
            CustomLogger.LoggerAccessor.LogInfo("[TCPUtils] - Port {0} available = {1}", port, isAvailable);
#endif
            return isAvailable;
        }
    }
}
