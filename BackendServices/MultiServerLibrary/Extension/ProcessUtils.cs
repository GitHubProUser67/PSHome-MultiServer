using CustomLogger;
using System;
using System.Diagnostics;
using System.Threading;

namespace MultiServerLibrary.Extension
{
    public static class ProcessUtils
    {
        public const int CustomServersLoopWaitTimeMs = 500; // Defines for how many ms we are awaiting a task result in the async loops.

        /// <summary>
        /// Check a process for idle state (long period of no CPU load) and kill if it's idle.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="averageLoad">Average CPU load by the process.</param>
        public static bool PreventProcessIdle(ref Process process, ref float averageLoad)
        {
            averageLoad = (float)(averageLoad + GetUsage(process)) / 2;

            if (Math.Round(averageLoad, 6) <= 0)
            {
                //the process is counting crows. Fire!
                try
                {
                    process.Kill();

                    LoggerAccessor.LogWarn("[PreventProcessIdle] - Idle process {0} killed.", process.ProcessName);

                    return true;
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[PreventProcessIdle] - Failed to kill idle process. Exception: {ex}");
                }
            }

            return false;
        }

        /// <summary>
        /// Get CPU load for process.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns>CPU usage in percents.</returns>
        private static double GetUsage(Process process)
        {
            //thx to: https://stackoverflow.com/a/49064915/7600726
            //see also https://www.mono-project.com/archived/mono_performance_counters/

            if (process.HasExited) return double.MinValue;

            // Preparing variable for application instance name
            string name = string.Empty;
#pragma warning disable
            foreach (string instance in new PerformanceCounterCategory("Process").GetInstanceNames())
            {
                if (process.HasExited) return double.MinValue;
                if (instance.StartsWith(process.ProcessName))
                {
                    using (PerformanceCounter processId = new PerformanceCounter("Process", "ID Process", instance, true))
                    {
                        if (process.Id == (int)processId.RawValue)
                        {
                            name = instance;
                            break;
                        }
                    }
                }
            }

            PerformanceCounter cpu = new PerformanceCounter("Process", "% Processor Time", name, true);

            // Getting first initial values
            cpu.NextValue();

            // Creating delay to get correct values of CPU usage during next query
            Thread.Sleep(500);
            if (process.HasExited) return double.MinValue;
            return Math.Round(cpu.NextValue() / Environment.ProcessorCount, 2);
#pragma warning restore
        }
    }
}
