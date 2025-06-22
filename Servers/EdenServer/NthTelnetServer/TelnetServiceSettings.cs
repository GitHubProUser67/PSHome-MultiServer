using System;

namespace NthDeveloper.TelnetServer
{
    /// <summary>
    /// Configuration class used when starting the Telnet service.
    /// </summary>
    public class TelnetServiceSettings
    {        
        /// <summary>
        /// Listen on all IP addresses on the host machine.
        /// </summary>
        public bool ListenAllAdapters { get; set; } = true;

        /// <summary>
        /// If ListenAllAdapters is false, holds the IP address that will be listened.
        /// </summary>
        public string? LocalIPAddress { get; set; }

        /// <summary>
        /// TCP port number that will be listened for incoming connections.
        /// </summary>
        public int PortNumber { get; set; } = 23;

        /// <summary>
        /// If true and Password is not empty, users are required to enter password in order to start executing commands.
        /// </summary>
        public bool PasswordIsEnabled { get; set; }

        /// <summary>
        /// If true , text will be printed to console when a telnet command is sent.
        /// </summary>
        public bool DebugInput { get; set; }

        /// <summary>
        /// Password that will be used for authenticating the incoming connections.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// The max number of times a user can enter a wrong password. After the last attempt, the connection will be closed by the server.
        /// </summary>
        public int MaxLoginTryCount { get; set; } = 3;

        /// <summary>
        /// Charset code for encoding and decoding text data.
        /// </summary>
        public int Charset { get; set; } = 20127;//US-ASCII

        /// <summary>
        /// The prompted text on the connected Telnet client console.
        /// </summary>
        public string PromtText { get; set; } = "TelnetService";

        /// <summary>
        /// Creates a shallow copy of this object.
        /// </summary>
        /// <returns></returns>
        public TelnetServiceSettings Clone()
        {
            return (TelnetServiceSettings)this.MemberwiseClone();
        }
    }
}
