using System;
using System.Collections.Generic;

namespace NthDeveloper.TelnetServer
{
    /// <summary>
    /// Interface for Telnet commands
    /// </summary>
    public interface ITelnetCommand
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Help description for this command
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Parameter definitions of this command (only used for displaying parameter help information)
        /// </summary>
        IEnumerable<CommandParameter> Parameters { get; }

        /// <summary>
        /// Executes the command with entered parameters
        /// </summary>
        /// <param name="parameters">Parameters entered by the Telnet user</param>
        /// <returns>Returns <see cref="T:NthDeveloper.TelnetServer.TelnetCommandResult" /> </returns>
        TelnetCommandResult Execute(Dictionary<string, string> parameters);
    }    
}
