namespace NthDeveloper.TelnetServer
{
    /// <summary>
    /// Telnet service interface
    /// </summary>
    public interface ITelnetService
    {
        /// <summary>
        /// Is running. True if the service is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Returns a copy of the settings that is passed to <see cref="Start" /> method.
        /// </summary>
        TelnetServiceSettings Settings { get; }

        /// <summary>
        /// Starts the Telnet service.
        /// </summary>
        /// <param name="settings">Service settings</param>
        /// <returns></returns>
        bool Start(TelnetServiceSettings settings);

        /// <summary>
        /// Stops the service.
        /// </summary>
        void Stop();
    }
}