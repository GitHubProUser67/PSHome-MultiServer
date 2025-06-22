using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace NetworkLibrary.Extension.Microsoft
{
    public class NetworkDrive : IDisposable
    {
        #region Constructor/Destructor
        public NetworkDrive()
        {
        }
        ~NetworkDrive()
        {
            Dispose();
        }
        public void Dispose()
        {
        }
        #endregion

        #region WIN32 API

        [StructLayout(LayoutKind.Sequential)]
        public struct NETRESOURCE
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }

        [Flags]
        public enum ResourceScope : int
        {
            Connected = 0x00000001,
            GlobalNet = 0x00000002,
            Remembered = 0x00000003,
            Recent = 0x00000004,
            Context = 0x00000005,
        }

        public enum ResourceType : uint
        {
            Any = 0x00000000,
            Disk = 0x00000001,
            Print = 0x00000002,
            Reserved = 0x00000008,
            Unknown = 0xFFFFFFFFu
        }

        [Flags]
        public enum ResourceUsage : uint
        {
            Connectable = 0x00000001,
            Container = 0x00000002,
            NoLocalDevice = 0x00000004,
            Sibling = 0x00000008,
            Attached = 0x00000010,
            All = 0x00000013,
            Reserved = 0x80000000u
        }

        public enum ResourceDisplayType :
            int
        {
            Generic = 0x00000000,
            Domain = 0x00000001,
            Server = 0x00000002,
            Share = 0x00000003,
            File = 0x00000004,
            Group = 0x00000005,
            Network = 0x00000006,
            Root = 0x00000007,
            ShareAdmin = 0x00000008,
            Directory = 0x00000009,
            Tree = 0x0000000A,
            NdsContainer = 0x0000000B,
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2A(ref NETRESOURCE pstNetRes, string psPassword, string psUsername, int piFlags);
        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2A(string psName, int piFlags, int pfForce);
        [DllImport("mpr.dll")]
        private static extern int WNetConnectionDialog(int phWnd, int piType);
        [DllImport("mpr.dll")]
        private static extern int WNetDisconnectDialog(int phWnd, int piType);
        [DllImport("mpr.dll")]
        private static extern int WNetRestoreConnectionW(int phWnd, string psLocalDrive);
        [DllImport("mpr.dll")]
        private static extern int WNetGetResourceInformation(ref NETRESOURCE lpNetResource, IntPtr lpBuffer, ref int lpcbBuffer, out IntPtr lplpSystem);
        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int WNetGetConnection([MarshalAs(UnmanagedType.LPTStr)] string localName, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName, ref int length);
        [DllImport("mpr.dll", EntryPoint = "WNetOpenEnumA", CallingConvention = CallingConvention.Winapi)]
        private static extern int WNetOpenEnum(int dwScope, int dwType, int dwUsage, ref NETRESOURCE lpNetResource, ref IntPtr lphEnum);
        [DllImport("mpr.dll", EntryPoint = "WNetCloseEnum", CallingConvention = CallingConvention.Winapi)]
        private static extern int WNetCloseEnum(IntPtr hEnum);
        [DllImport("mpr.dll", EntryPoint = "WNetEnumResourceA", CallingConvention = CallingConvention.Winapi)]
        private static extern int WNetEnumResource(IntPtr hEnum, ref uint lpcCount, IntPtr buffer, ref int lpBufferSize);

        //Standard	
        private const int CONNECT_INTERACTIVE = 0x00000008;
        private const int CONNECT_PROMPT = 0x00000010;
        private const int CONNECT_UPDATE_PROFILE = 0x00000001;
        //NT5 only
        private const int CONNECT_CMD_SAVECRED = 0x00001000;

        enum ErrorCodes
        {
            NO_ERROR = 0,
            ERROR_NO_MORE_ITEMS = 259
        };
        #endregion

        #region Fields
        private bool saveCredentials = false;
        private bool persistent = false;
        private bool force = false;
        private bool promptForCredentials = false;
        private string drive = "s:";
        private string shareName = "\\\\Computer\\C$";
        #endregion

        #region Properties
        /// <summary>
        /// Option to save credentials are reconnection...
        /// </summary>
        public bool SaveCredentials
        {
            get { return saveCredentials; }
            set { saveCredentials = value; }
        }

        /// <summary>
        /// Option to reconnect drive after log off / reboot ...
        /// </summary>
        public bool Persistent
        {
            get { return persistent; }
            set { persistent = value; }
        }

        /// <summary>
        /// Option to force connection if drive is already mapped...
        /// or force disconnection if network path is not responding...
        /// </summary>
        public bool Force
        {
            get { return force; }
            set { force = value; }
        }

        /// <summary>
        /// Option to prompt for user credintals when mapping a drive
        /// </summary>
        public bool PromptForCredentials
        {
            get { return promptForCredentials; }
            set { promptForCredentials = value; }
        }

        /// <summary>
        /// Drive to be used in mapping / unmapping...
        /// </summary>
        public string LocalDrive
        {
            get { return drive; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    drive = value.Substring(0, 1) + ":";
                else
                    drive = null;
            }
        }

        /// <summary>
        /// Share address to map drive to.
        /// </summary>
        public string ShareName
        {
            get { return shareName; }
            set { shareName = value; }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Map network drive
        /// </summary>
        public void MapDrive()
        {
            mapDrive(null, null);
        }

        /// <summary>
        /// Map network drive (using supplied Password)
        /// </summary>
        public void MapDrive(string Password)
        {
            mapDrive(null, Password);
        }

        /// <summary>
        /// Map network drive (using supplied Username and Password)
        /// </summary>
        public void MapDrive(string Username, string Password)
        {
            mapDrive(Username, Password);
        }

        /// <summary>
        /// Unmap network drive
        /// </summary>
        public void UnMapDrive()
        {
            unMapDrive(force);
        }

        /// <summary>
        /// Check / restore persistent network drive
        /// </summary>
        public void RestoreDrives()
        {
            restoreDrive();
        }

        public string GetAvailableDrive()
        {
            string[] alphabet = { "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            List<string> lalphabet = new List<string>(alphabet);
            foreach (DriveInfo drive in GetDrives())
            {
                lalphabet.Remove(drive.Name.Substring(0, 1).ToLower());
            }
            return lalphabet.Count > 0 ? lalphabet[0].ToUpper() + ":" : "";
        }

        public DriveInfo[] GetDrives()
        {
            DriveInfo[] drvs = new DriveInfo[0];
            using (ProcessImpersonation processImp = new ProcessImpersonation())
            {
                processImp.Impersonate(delegate
                {
                    drvs = DriveInfo.GetDrives();
                    return null;
                });
            }
            return drvs;
        }

        public NETRESOURCE GetNetworkResourceInfo(string path)
        {
            NETRESOURCE result = default;
            using (ProcessImpersonation processImp = new ProcessImpersonation())
            {
                processImp.Impersonate(delegate
                {
                    NETRESOURCE stNetRes = new NETRESOURCE();
                    stNetRes.lpRemoteName = path;

                    int lpcbBuffer = Marshal.SizeOf(typeof(NETRESOURCE));
                    IntPtr lpBuffer = Marshal.AllocHGlobal(lpcbBuffer);
                    IntPtr lplpSystem = IntPtr.Zero;

                    int error = WNetGetResourceInformation(ref stNetRes, lpBuffer, ref lpcbBuffer, out lplpSystem);
                    while (error == 234)
                    {
                        lpBuffer = Marshal.ReAllocHGlobal(lpBuffer, new IntPtr(lpcbBuffer));
                        error = WNetGetResourceInformation(ref stNetRes, lpBuffer, ref lpcbBuffer, out lplpSystem);
                        if (error > 0 && error != 234)
                            break;
                    }

                    result = (NETRESOURCE)Marshal.PtrToStructure(lpBuffer, typeof(NETRESOURCE));

                    Marshal.FreeHGlobal(lpBuffer);
                    return null;
                });
            }
            return result;
        }

        public string GetPath(DriveInfo drive)
        {
            //variable to hold the returned list
            StringBuilder path = new StringBuilder(255);

            //check the drive type, if Network then get it's information
            //and add to our list
            using (ProcessImpersonation processImp = new ProcessImpersonation())
            {
                processImp.Impersonate(delegate
                {
                    if (drive.DriveType.Equals(DriveType.Network))
                    {
                        //capacity of the string builder (required for the Win32 API call
                        int len = path.Capacity;

                        //call the Win32 API

                        int error = 0;
                        do
                        {
                            error = WNetGetConnection(drive.Name.Replace("\\", ""), path, ref len);
                        }
                        while (error != 0);
                    }
                    return null;
                });
            }
            return path.ToString();
        }

        public List<NETRESOURCE> GetNetworkDrives(ResourceScope scope, ResourceType type, ResourceUsage usage, ResourceDisplayType displayType)
        {
            List<NETRESOURCE> result = new List<NETRESOURCE>();

            using (ProcessImpersonation processImp = new ProcessImpersonation())
            {
                processImp.Impersonate(delegate
                {
                    EnumerateServers(new NETRESOURCE(), scope, type, usage, displayType, result);
                    return null;
                });
            }

            return result;
        }
        #endregion

        #region Private methods
        private void EnumerateServers(NETRESOURCE pRsrc, ResourceScope scope, ResourceType type, ResourceUsage usage, ResourceDisplayType displayType, List<NETRESOURCE> netresorces)
        {
            int bufferSize = 16384;
            IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
            IntPtr handle = IntPtr.Zero;
            int result;
            uint cEntries = 1;

            try
            {
                result = WNetOpenEnum((int)scope, (int)type, (int)usage, ref pRsrc, ref handle);

                if (result == (int)ErrorCodes.NO_ERROR)
                {
                    do
                    {
                        result = WNetEnumResource(handle, ref cEntries, buffer, ref bufferSize);

                        if (result == (int)ErrorCodes.NO_ERROR)
                        {
                            pRsrc = (NETRESOURCE)Marshal.PtrToStructure(buffer, typeof(NETRESOURCE));

                            if (pRsrc.dwDisplayType == (int)displayType)
                                netresorces.Add(pRsrc);

                            if ((pRsrc.dwUsage & (int)ResourceUsage.Container) == (int)ResourceUsage.Container)
                                EnumerateServers(pRsrc, scope, type, usage, displayType, netresorces);
                        }
                        else if (result != (int)ErrorCodes.ERROR_NO_MORE_ITEMS)
                            break;
                    }
                    while (result != (int)ErrorCodes.ERROR_NO_MORE_ITEMS);

                    WNetCloseEnum(handle);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        // Map network drive
        private void mapDrive(string psUsername, string psPassword)
        {
            //create struct data
            NETRESOURCE stNetRes = new NETRESOURCE();
            stNetRes.dwScope = (int)ResourceScope.GlobalNet;
            stNetRes.dwType = (int)ResourceType.Disk;
            stNetRes.dwDisplayType = (int)ResourceDisplayType.Share;
            stNetRes.dwUsage = (int)ResourceUsage.Connectable;
            stNetRes.lpRemoteName = shareName;
            stNetRes.lpLocalName = drive;
            //prepare params
            int iFlags = 0;
            if (saveCredentials)
                iFlags += CONNECT_CMD_SAVECRED;
            if (persistent)
                iFlags += CONNECT_UPDATE_PROFILE;
            if (promptForCredentials)
                iFlags += CONNECT_INTERACTIVE + CONNECT_PROMPT;
            if (string.IsNullOrEmpty(psUsername))
                psUsername = null;
            if (string.IsNullOrEmpty(psPassword))
                psPassword = null;
            //if force, unmap ready for new connection
            if (force)
            {
                try
                {
                    unMapDrive(true);
                }
                catch { }
            }
            //call and return
            using (ProcessImpersonation processImp = new ProcessImpersonation())
            {
                processImp.Impersonate(delegate
                {
                    int i = WNetAddConnection2A(ref stNetRes, psPassword, psUsername, iFlags);
                    if (i > 0)
                        throw new System.ComponentModel.Win32Exception(i);
                    return null;
                });
            }
        }

        // Unmap network drive
        private void unMapDrive(bool pfForce)
        {
            int iFlags = 0;
            if (persistent)
                iFlags += CONNECT_UPDATE_PROFILE;

            using (ProcessImpersonation processImp = new ProcessImpersonation())
            {
                processImp.Impersonate(delegate
                {
                    //call unmap and return
                    int i = WNetCancelConnection2A(drive, iFlags, Convert.ToInt32(pfForce));
                    if (i > 0)
                        throw new System.ComponentModel.Win32Exception(i);
                    return null;
                });
            }
        }

        // Check / Restore a network drive
        private void restoreDrive()
        {
            using (ProcessImpersonation processImp = new ProcessImpersonation())
            {
                processImp.Impersonate(delegate
                {
                    //call restore and return
                    int i = WNetRestoreConnectionW(0, null);
                    if (i > 0)
                        throw new System.ComponentModel.Win32Exception(i);
                    return null;
                });
            }
        }

        // Display windows dialog
        private void displayDialog(int iHandle, int piDialog)
        {
            int i = -1;

            //show dialog
            using (ProcessImpersonation processImp = new ProcessImpersonation())
            {
                processImp.Impersonate(delegate
                {
                    if (piDialog == 1)
                        i = WNetConnectionDialog(iHandle, (int)ResourceType.Disk);
                    else if (piDialog == 2)
                        i = WNetDisconnectDialog(iHandle, (int)ResourceType.Disk);
                    if (i > 0)
                        throw new System.ComponentModel.Win32Exception(i);
                    return null;
                });
            }
        }
        #endregion
    }
}
