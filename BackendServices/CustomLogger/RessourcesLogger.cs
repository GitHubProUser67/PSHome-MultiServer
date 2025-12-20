using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CustomLogger
{
    // Windows Only
    public class RessourcesLoggerWin32
    {
        private static TimeSpan _lastCpuTime = TimeSpan.Zero;
        private static DateTime _lastCpuCheck = DateTime.UtcNow;
        private static readonly object _cpuLock = new object();
        private static int _cachedCpu = 0;
        private static readonly int _cpuCoreCount = Environment.ProcessorCount;

        public static Task StartPerfWatcher()
        {
            while (true)
            {
                Thread.Sleep(5 * 60 * 1000);

                LoggerAccessor.LogInfo($"[RessourcesLogger] - Current percentage Used Physical Ram: {100 - (((decimal)PerformanceInfoWin32.GetPhysicalAvailableMemoryInMiB() / (decimal)PerformanceInfoWin32.GetTotalMemoryInMiB()) * 100)}");
            }
        }

        public static double GetCurrentCpuUsage()
        {
            lock (_cpuLock)
            {
                var now = DateTime.UtcNow;
                var currentCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
                var elapsed = (now - _lastCpuCheck).TotalMilliseconds;

                if (elapsed < 500)
                    return _cachedCpu;

                var cpuUsedMs = (currentCpuTime - _lastCpuTime).TotalMilliseconds;
                var cpuPercentage = (int)((cpuUsedMs / elapsed) * 100 / _cpuCoreCount);

                _lastCpuTime = currentCpuTime;
                _lastCpuCheck = now;

                _cachedCpu = Math.Max(0, Math.Min(cpuPercentage, 100));
                return _cachedCpu;
            }
        }
    }

    public static class PerformanceInfoWin32

    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static long GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
                return Convert.ToInt64(pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576);

            return -1;
        }

        public static long GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
                return Convert.ToInt64(pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576);

            return -1;
        }
    }
}
