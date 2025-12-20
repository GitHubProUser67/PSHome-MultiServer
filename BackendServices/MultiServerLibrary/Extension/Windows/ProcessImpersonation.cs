using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace MultiServerLibrary.Extension.Microsoft
{
    public delegate object Invoker();

    public class Win32API
    {
        public enum SECURITY_IMPERSONATION_LEVEL : int
        {
            /// <summary>
            /// The server process cannot obtain identification information about the client, 
            /// and it cannot impersonate the client. It is defined with no value given, and thus, 
            /// by ANSI C rules, defaults to a value of zero. 
            /// </summary>
            SecurityAnonymous = 0,

            /// <summary>
            /// The server process can obtain information about the client, such as security identifiers and privileges, 
            /// but it cannot impersonate the client. This is useful for servers that export their own objects, 
            /// for example, database products that export tables and views. 
            /// Using the retrieved client-security information, the server can make access-validation decisions without 
            /// being able to use other services that are using the client's security context. 
            /// </summary>
            SecurityIdentification = 1,

            /// <summary>
            /// The server process can impersonate the client's security context on its local system. 
            /// The server cannot impersonate the client on remote systems. 
            /// </summary>
            SecurityImpersonation = 2,

            /// <summary>
            /// The server process can impersonate the client's security context on remote systems. 
            /// NOTE: Windows NT:  This impersonation level is not supported.
            /// </summary>
            SecurityDelegation = 3,
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int CloseHandle(IntPtr handle);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSQueryUserToken(uint sessionId, out IntPtr Token);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int OpenProcessToken(IntPtr ProcessHandle,
            int DesiredAccess, ref IntPtr TokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DuplicateToken(IntPtr ExistingTokenHandle, int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);

        public static readonly bool IsWindows = Environment.OSVersion.Platform == PlatformID.Win32NT
            || Environment.OSVersion.Platform == PlatformID.Win32S || Environment.OSVersion.Platform == PlatformID.Win32Windows;

        /// <summary>
        /// Know if we are the true administrator of the Windows system.
        /// <para>Savoir si est réellement l'administrateur Windows.</para>
        /// </summary>
        /// <returns>A boolean.</returns>
#pragma warning disable
        public static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
#pragma warning restore

        public static bool StartAsAdmin(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            try
            {
                new Process()
                {
                    StartInfo =
                    {
                        FileName = filePath,
                        UseShellExecute = true,
                        Verb = "runas"
                    }
                }.Start();
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static IntPtr GetActiveUserToken()
        {
            uint result = WTSGetActiveConsoleSessionId();
            if (result != 0xFFFFFFFF)
            {
                IntPtr uToken;
                if (WTSQueryUserToken(result, out uToken))
                {
                    return uToken;
                }
                int error = Marshal.GetLastWin32Error();
            }
            return WindowsIdentity.GetCurrent().Token;
        }
    }

    public class ProcessImpersonation : IDisposable
    {
        #region win32 api
        protected const int TOKEN_DUPLICATE = 2;
        protected const int TOKEN_QUERY = 0X00000008;
        protected const int TOKEN_IMPERSONATE = 0X00000004;
        #endregion

        #region Fields
        protected IntPtr expProcToken = IntPtr.Zero;
        #endregion

        #region Constructor/Destructor
        public ProcessImpersonation()
        {
            Initialize();
        }
        ~ProcessImpersonation()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (expProcToken != IntPtr.Zero)
            {
                Win32API.CloseHandle(expProcToken);
                expProcToken = IntPtr.Zero;
            }
        }
        #endregion

        #region Protected methods
        protected virtual void Initialize()
        {
            IntPtr prToken = IntPtr.Zero;
            try
            {
                using (Process curprocess = Process.GetCurrentProcess())
                {
                    IntPtr prHandel = IntPtr.Zero;
                    foreach (Process proc in Process.GetProcessesByName("explorer"))
                    {
                        if (proc.SessionId == curprocess.SessionId)
                        {
                            prHandel = proc.Handle;
                            break;
                        }
                    }
                    if (prHandel == IntPtr.Zero)
                        prHandel = curprocess.Handle;
                    if (Win32API.OpenProcessToken(prHandel, TOKEN_QUERY | TOKEN_IMPERSONATE | TOKEN_DUPLICATE, ref prToken) != 0)
                        Win32API.DuplicateToken(prToken, (int)Win32API.SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, ref expProcToken);
                }
            }
            catch { }
            finally
            {
                if (prToken != IntPtr.Zero)
                {
                    Win32API.CloseHandle(prToken);
                    prToken = IntPtr.Zero;
                }
            }
        }
        #endregion

        #region Public methods
        public void Impersonate(Invoker impersonatedMthd)
        {
            if (expProcToken != IntPtr.Zero)
            {
                using (WindowsIdentity dupnewId = new WindowsIdentity(expProcToken))
                {
#if NETFRAMEWORK
                    using (WindowsImpersonationContext impersonatedUser = dupnewId.Impersonate())
                    {
                        try
                        {
                            impersonatedMthd();
                        }
                        finally
                        {
                            impersonatedUser.Undo();
                        }
                    }
#else
                    WindowsIdentity.RunImpersonated(dupnewId.AccessToken, () =>
                    {
                        impersonatedMthd();
                    });
#endif
                }
            }
        }
        #endregion
    }

    public class HighProcessImpersonation : ProcessImpersonation
    {
        protected override void Initialize()
        {
            bool result = false;
            IntPtr prToken = IntPtr.Zero;
            try
            {
                uint accSessionId = Win32API.WTSGetActiveConsoleSessionId();
                IntPtr prHandel = IntPtr.Zero;
                foreach (Process proc in Process.GetProcessesByName("explorer"))
                {
                    if (proc.SessionId == accSessionId)
                    {
                        prHandel = proc.Handle;
                        break;
                    }
                }
                if (Win32API.OpenProcessToken(prHandel, TOKEN_QUERY | TOKEN_IMPERSONATE | TOKEN_DUPLICATE, ref prToken) != 0)
                    result = Win32API.DuplicateToken(prToken, (int)Win32API.SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, ref expProcToken);
            }
            catch { }
            finally
            {
                if (prToken != IntPtr.Zero)
                {
                    Win32API.CloseHandle(prToken);
                    prToken = IntPtr.Zero;
                }
            }
            if (!result)
            {
                if (expProcToken != IntPtr.Zero)
                {
                    Win32API.CloseHandle(expProcToken);
                    expProcToken = IntPtr.Zero;
                }
                base.Initialize();
            }
        }
    }
}
