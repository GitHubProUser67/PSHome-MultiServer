using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NthDeveloper.TelnetServer
{
    /// <summary>
    /// Contains information about the result of a command execution.
    /// </summary>
    public struct TelnetCommandResult
    {
        /// <summary>
        /// Whether the command was successfully executed.
        /// </summary>
        public bool IsSucceeded { get; private set; }

        /// <summary>
        /// Response text that will be sent to the client.
        /// </summary>
        public string ResponseText { get; private set; }
        
        public TelnetCommandResult(bool isSucceeded, string responseText)
        {
            this.IsSucceeded = isSucceeded;
            this.ResponseText = responseText;
        }

        /// <summary>
        /// Creates a successful command result.
        /// </summary>
        /// <param name="responseText">Response text that will be sent to the client.</param>
        /// <returns>Successful command result.</returns>
        public static TelnetCommandResult Success(string responseText=null)
        {
            return new TelnetCommandResult(true, responseText);
        }

        /// <summary>
        /// Creates a failed command result.
        /// </summary>
        /// <param name="responseText">Response text that will be sent to the client.</param>
        /// <returns>Failed command result.</returns>
        public static TelnetCommandResult Fail(string responseText=null)
        {
            return new TelnetCommandResult(false, responseText);
        }
    }
}
