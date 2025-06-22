using System;
using System.Collections.Generic;

namespace NthDeveloper.TelnetServer
{
    partial class TelnetService
    {
        /// <summary>
        /// Help command. Only for listing available commands.
        /// Real execution is done in TelnetService class.
        /// </summary>
        class HelpCommand : ITelnetCommand
        {
            readonly CommandParameter[] m_Parameters;

            public string CommandName
            {
                get { return "help"; }
            }

            public string Description
            {
                get { return "Displays help information for available commands."; }
            }

            public IEnumerable<CommandParameter> Parameters { get { return m_Parameters; } }

            public HelpCommand()
            {
                m_Parameters = new CommandParameter[]
                {
                    new CommandParameter("<name>", false, "Name of the command.")
                };
            }

            public TelnetCommandResult Execute(Dictionary<string, string> parameters)
            {
                return TelnetCommandResult.Success();
            }
        }

        /// <summary>
        /// Exit command. Only for listing available commands.
        /// Real execution is done in TelnetService class.
        /// </summary>
        class ExitCommand : ITelnetCommand
        {
            public string CommandName
            {
                get { return "exit"; }
            }

            public string Description
            {
                get { return "Closes the terminal connection."; }
            }

            public IEnumerable<CommandParameter> Parameters { get { return null; } }

            public TelnetCommandResult Execute(Dictionary<string, string> parameters)
            {
                return TelnetCommandResult.Success();
            }
        }
    }
}
