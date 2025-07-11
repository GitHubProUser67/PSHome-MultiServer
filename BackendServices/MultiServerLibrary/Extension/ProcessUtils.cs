using CustomLogger;
using System;
using System.Diagnostics;
using System.Threading;

namespace MultiServerLibrary.Extension
{
    public static class ProcessUtils
    {
        /// <summary>
        /// Check a process for idle state (long period of no CPU load) and kill if it's idle.
        /// </summary>
        /// <param name="Proc">The process.</param>
        /// <param name="AverageLoad">Average CPU load by the process.</param>
        public static void PreventProcessIdle(ref Process Proc, ref float AverageLoad)
        {
            AverageLoad = (float)(AverageLoad + GetUsage(Proc)) / 2;

            if (!Proc.HasExited)
                if (Math.Round(AverageLoad, 6) <= 0 && !Proc.HasExited)
                {
                    //the process is counting crows. Fire!
                    Proc.Kill();
                    if (Console.CursorLeft > 0)
                        Console.WriteLine();
                    LoggerAccessor.LogWarn("[PreventProcessIdle] - Idle process {0} killed.", Proc.ProcessName);
                }
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
