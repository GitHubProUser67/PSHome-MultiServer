using CustomLogger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NthDeveloper.TelnetServer
{
    public partial class TelnetService : ITelnetService
    {
        class ConnectedClient
        {
            public ClientInfo? Client;
            public string? CommandBuffer = String.Empty;
            public Encoding? TextEncoder;
            public bool IsLoginRequired;
            public int LoginTryCount;
        }

        class ReceivedCommandItem
        {
            public ConnectedClient? Client { get; set; }
            public string? InputCommand { get; set; }
            public string? CommandName { get; set; }
            public bool IsSucceed { get; set; }
            public string? ResponseMessage { get; set; }
            public Dictionary<string, string>? Parameters { get; set; }
        }

        static readonly Regex TokenizerRegex = new Regex("(\"[^\"]+\")|([^ \":]+)|(:)", RegexOptions.Compiled);       

        bool m_IsRunning;
        ITCPServer m_TCPServer;
        Thread m_thrCommandProcessor;

        Dictionary<string, ITelnetCommand> m_Commands;
        ConcurrentDictionary<string, ConnectedClient> m_Clients;
        ConcurrentQueue<ReceivedCommandItem> m_ReceivedCommands;
        TelnetServiceSettings m_Settings;

        public bool IsRunning
        {
            get { return m_IsRunning; }
        }
        
        public TelnetServiceSettings Settings
        {
            get { return m_Settings.Clone(); }
        }

        public TelnetService(ITCPServer tcpServer, ITelnetCommand[] customCommands)
        {
            m_TCPServer = tcpServer;
            m_Clients = new ConcurrentDictionary<string, ConnectedClient>();
            m_ReceivedCommands = new ConcurrentQueue<ReceivedCommandItem>();
            m_Settings = new TelnetServiceSettings();

            buildCommandsCatalog(customCommands ?? new ITelnetCommand[0]);
        }

        public bool Start(TelnetServiceSettings settings)
        {
            if (m_IsRunning)
                return true;

            m_Settings = settings.Clone();

            int _portNo = m_Settings.PortNumber;
            string? _ipAddress = null;
            if (!m_Settings.ListenAllAdapters && !String.IsNullOrEmpty(m_Settings.LocalIPAddress))
                _ipAddress = m_Settings.LocalIPAddress;

            IPEndPoint _endPoint = new IPEndPoint(!String.IsNullOrEmpty(_ipAddress) ? IPAddress.Parse(_ipAddress) : IPAddress.Any, _portNo);

            clearClientsAndReceivedCommands();

            m_TCPServer.ClientConnected += m_TCPServer_ClientConnected;
            m_TCPServer.ClientDisconnected += m_TCPServer_ClientDisconnected;
            m_TCPServer.DataReceived += m_TCPServer_DataReceived;

            try
            {
                m_TCPServer.StartListening(_endPoint);
                m_IsRunning = true;

                m_thrCommandProcessor = new Thread(commandProcessorThread);
                m_thrCommandProcessor.Name = "TelnetCommandProcessor";
                m_thrCommandProcessor.Start();
            }
            catch (Exception ex)
            {
                //log.Error("TelnetService could not be started.", ex);
                Stop();
            }

            return true;
        }

        public void Stop()
        {
            if (!m_IsRunning)
                return;

            m_IsRunning = false;

            m_TCPServer.ClientConnected -= m_TCPServer_ClientConnected;
            m_TCPServer.ClientDisconnected -= m_TCPServer_ClientDisconnected;
            m_TCPServer.DataReceived -= m_TCPServer_DataReceived;

            m_TCPServer.StopListening();

            clearClientsAndReceivedCommands();
        }

        private void buildCommandsCatalog(ITelnetCommand[] customCommands)
        {
            m_Commands = new Dictionary<string, ITelnetCommand>(customCommands.Length + 2);

            var _builtInCommands = Assembly.GetAssembly(typeof(ITelnetCommand)).GetTypes()
                .Where(x => !x.IsInterface && typeof(ITelnetCommand).IsAssignableFrom(x));

            foreach (Type commandType in _builtInCommands)
            {
                addCommandToCatalog((ITelnetCommand)Activator.CreateInstance(commandType));
            }

            for (int i = 0; i < customCommands.Length; i++)
                addCommandToCatalog(customCommands[i]);
        } 
        
        private void addCommandToCatalog(ITelnetCommand command)
        {
            string _commandLowerCase = command.CommandName.ToLowerInvariant();

            if (!m_Commands.ContainsKey(_commandLowerCase))
                m_Commands.Add(_commandLowerCase, command);
        }       

        private void clearClientsAndReceivedCommands()
        {
            while (!m_ReceivedCommands.IsEmpty)
            {
                ReceivedCommandItem _cmd;
                m_ReceivedCommands.TryDequeue(out _cmd);
            }

            m_Clients.Clear();
        }

        private void commandProcessorThread()
        {
            while (m_IsRunning)
            {
                if (m_ReceivedCommands.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                ReceivedCommandItem _cmd;
                m_ReceivedCommands.TryDequeue(out _cmd);

                if (_cmd == null)
                    continue;

                processCommandItem(_cmd);
            }
        }

        private void processCommandItem(ReceivedCommandItem commandItem)
        {
            bool _parseFailed = false;
            try
            {
                parseCommandLine(commandItem);
            }
            catch (Exception ex)
            {
                commandItem.ResponseMessage = "Command parse error:" + ex.Message;
                commandItem.IsSucceed = false;

                _parseFailed = true;
            }

            try
            {
                if (!_parseFailed)
                    executeCommand(commandItem);
            }
            catch (Exception ex)
            {
                commandItem.ResponseMessage = "Command execution error:" + ex.Message;
                commandItem.IsSucceed = false;
            }

            sendCommandResult(commandItem);
        }

        private void parseCommandLine(ReceivedCommandItem commandItem)
        {
            Stack<string> commandStack = new Stack<string>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            Match regExMatch = TokenizerRegex.Match(commandItem.InputCommand);

            bool parameter = false;

            while (regExMatch.Success)
            {
                string temp = regExMatch.Value.Replace("\"", "").Replace("'", "");
                switch (temp)
                {
                    case ":":
                        if (parameter)
                            throw new ArgumentException("Invalid Parameters");

                        regExMatch = regExMatch.NextMatch();
                        if (regExMatch.Success)
                            parameters.Add(commandStack.Pop(), regExMatch.Value.Replace("\"", "").Replace("'", ""));

                        parameter = true;
                        break;
                    default:
                        commandStack.Push(temp);
                        parameter = false;
                        break;
                }
                regExMatch = regExMatch.NextMatch();
            }

            while (commandStack.Count > 1)
                parameters.Add(commandStack.Pop(), null);

            commandItem.CommandName = commandStack.Pop().ToLowerInvariant();
            commandItem.Parameters = parameters;
        }        

        private void executeCommand(ReceivedCommandItem commandItem)
        {
            if (!m_Commands.ContainsKey(commandItem.CommandName))
            {
                commandItem.IsSucceed = false;
                commandItem.ResponseMessage = "Unknown command.";
                return;
            }

            if (commandItem.CommandName == "help")
            {
                processHelpCommand(commandItem);
            }
            else if (commandItem.CommandName == "exit")
            {
                processExitCommand(commandItem);
            }
            else
            {
                try
                {
                    var _cmd = m_Commands[commandItem.CommandName];
                    var _result = _cmd.Execute(commandItem.Parameters);

                    commandItem.IsSucceed = _result.IsSucceeded;
                    commandItem.ResponseMessage = _result.ResponseText;
                }
                catch (Exception ex)
                {
                    commandItem.IsSucceed = false;
                    commandItem.ResponseMessage = "Error:" + ex.Message;
                    return;
                }
            }
        }

        private void sendCommandResult(ReceivedCommandItem commandItem)
        {
            string _strResult;

            if (commandItem.IsSucceed)
                _strResult = "OK:";
            else
                _strResult = "FAILED:";

            if (!String.IsNullOrEmpty(commandItem.ResponseMessage))
                _strResult += "\r\n" + commandItem.ResponseMessage;

            _strResult += "\r\n" + getPromptText();

            try
            {
                if (commandItem.Client.Client.ClientSocket.Connected)
                    commandItem.Client.Client.ClientSocket.Send(commandItem.Client.TextEncoder.GetBytes(_strResult));
            }
            catch { }
        }

        private void promptLogin(ConnectedClient client)
        {
            client.Client.ClientSocket.Send(client.TextEncoder.GetBytes(" \r\npassword:"));
        }

        private void processHelpCommand(ReceivedCommandItem commandItem)
        {
            if (commandItem.Parameters.Keys.Count == 0)//Display general information for all commands
            {
                StringBuilder _result = new StringBuilder(1024);
                _result.AppendLine("Available commands");

                foreach (var cmdPair in m_Commands)
                {
                    var _cmd = cmdPair.Value;
                    _result.Append(_cmd.CommandName);
                    _result.Append(":\t");
                    _result.AppendLine(_cmd.Description);
                }

                commandItem.IsSucceed = true;
                commandItem.ResponseMessage = _result.ToString();
            }
            else//Dispaly help for a specific command
            {
                string _commanddName = commandItem.Parameters.Keys.ElementAt(0).ToLowerInvariant();
                if (!m_Commands.ContainsKey(_commanddName))
                {
                    commandItem.IsSucceed = false;
                    commandItem.ResponseMessage = String.Format("Command not found '{0}'", _commanddName);
                }
                else
                {
                    var _cmd = m_Commands[_commanddName];
                    StringBuilder _result = new StringBuilder(1024);
                    _result.Append("Command:");
                    _result.AppendLine(_commanddName);
                    _result.AppendLine(_cmd.Description);

                    var _parameters = _cmd.Parameters;

                    if (_parameters != null)
                    {
                        _result.AppendLine("[Parameters]\r\n---------------------------------------------");

                        foreach(var _parameter in _parameters)
                        {
                            if(_parameter.IsRequired)
                                _result.Append("[r]");

                            _result.Append(_parameter.Name+":\t");
                            _result.AppendLine(_parameter.Description);
                        }
                    }
                    else //No paarmeters
                    {
                        _result.AppendLine("This command has no parameters.");
                    }

                    commandItem.IsSucceed = true;
                    commandItem.ResponseMessage = _result.ToString();
                }
            }
        }

        private void processExitCommand(ReceivedCommandItem commandItem)
        {
            try
            {
                commandItem.Client.Client.ClientSocket.Send(commandItem.Client.TextEncoder.GetBytes("OK: Bye!"));
                commandItem.Client.Client.ClientSocket.Close();

                commandItem.IsSucceed = true;
            }
            catch
            {
                commandItem.IsSucceed = false;
            }
        }

        private string getPromptText()
        {
            return m_Settings.PromtText + ">";
        }

        private void m_TCPServer_ClientConnected(ITCPServer server, ClientInfo client)
        {
#if DEBUG
            LoggerAccessor.LogInfo($"[TELNET] - Connection received on port {m_Settings.PortNumber} (Thread {Environment.CurrentManagedThreadId.ToString()})");
#endif
            ConnectedClient _connectedClient = new ConnectedClient();
            _connectedClient.Client = client;
            _connectedClient.CommandBuffer = null;
            _connectedClient.TextEncoder = Encoding.GetEncoding(m_Settings.Charset);
            _connectedClient.IsLoginRequired = m_Settings.PasswordIsEnabled;

            m_Clients.TryAdd(client.ClientID, _connectedClient);

            _connectedClient.Client.ClientSocket.Send(_connectedClient.TextEncoder.GetBytes(" \r\n" + getPromptText()));//Telnet ekranını temizlemek için
            if (_connectedClient.IsLoginRequired)
                promptLogin(_connectedClient);
        }

        private void m_TCPServer_ClientDisconnected(ITCPServer server, ClientInfo client)
        {
            m_Clients.TryRemove(client.ClientID, out _);
        }

        private void m_TCPServer_DataReceived(ITCPServer server, ClientInfo client, byte[] data)
        {
            ConnectedClient _connectedClient;
            if (!m_Clients.TryGetValue(client.ClientID, out _connectedClient))
                return;

            if (data.Length == 0)
                return;

            if (data[0] == 127)//Ignore delete key
                return;

            //Ignore arrow keys
            if (data.Length == 3)
            {
                if (data[0] == 27 && data[1] == 91)
                {
                    byte _thirdByte = data[2];

                    //65,66,67,68:Up,Down,Right,Left
                    if (_thirdByte > 64 && _thirdByte < 69)
                        return;
                }
            }

            int _startIndex = 0;
            do
            {
                if (data[_startIndex] == (byte)255)//Skip Telnet handshaking data
                {
                    _startIndex += 3;
                }
                else
                    break;
            }
            while (_startIndex <= data.Length - 1);

            if (_startIndex > data.Length - 1)
                return;

            string _textData = _connectedClient.TextEncoder.GetString(data, _startIndex, data.Length - _startIndex);

            if (_textData == "\b")//Backspace
            {
                if (_connectedClient.CommandBuffer.Length > 0)
                {
                    _connectedClient.CommandBuffer = _connectedClient.CommandBuffer.Remove(_connectedClient.CommandBuffer.Length - 1);
                }

                _connectedClient.Client.ClientSocket.Send(new byte[] { 0x20, 0x08 });

                return;
            }

            if (m_Settings.DebugInput)
                LoggerAccessor.LogInfo($"[TELNET] - ClientInfo:{_connectedClient.Client} sent Text:{{{_textData}}}");

            // Add to buffer
            _connectedClient.CommandBuffer += _textData;

            int _endOfLineIndex = _connectedClient.CommandBuffer.IndexOf('\r');
            if (_endOfLineIndex > -1)
            {
                do
                {
                    if (_endOfLineIndex > 0)
                    {
                        string _cmd = _connectedClient.CommandBuffer.Substring(0, _endOfLineIndex + 1).Replace("\r", "").Replace("\n", "").Trim();

                        if (!String.IsNullOrEmpty(_cmd))
                        {
                            if (_connectedClient.IsLoginRequired)//Has not logged in yet
                            {
                                _connectedClient.CommandBuffer = String.Empty;

                                if (String.Compare(m_Settings.Password, _cmd) == 0)
                                {
                                    _connectedClient.IsLoginRequired = false;
                                    sendCommandResult(new ReceivedCommandItem() { Client = _connectedClient, CommandName = "login", IsSucceed = true, ResponseMessage = "Logged in." });
                                    return;
                                }
                                else
                                {
                                    _connectedClient.LoginTryCount++;
                                    sendCommandResult(new ReceivedCommandItem() { Client = _connectedClient, CommandName = "login", IsSucceed = false, ResponseMessage = "Invalid password." });

                                    if (_connectedClient.LoginTryCount >= m_Settings.MaxLoginTryCount)
                                    {
                                        _connectedClient.Client.ClientSocket.Close();
                                        return;
                                    }

                                    promptLogin(_connectedClient);
                                    return;
                                }
                            }
                            else
                            {
                                ReceivedCommandItem _cmdInfo = new ReceivedCommandItem();
                                _cmdInfo.Client = _connectedClient;
                                _cmdInfo.InputCommand = _cmd;

                                m_ReceivedCommands.Enqueue(_cmdInfo);
                            }
                        }
                    }

                    _connectedClient.CommandBuffer = _connectedClient.CommandBuffer.Substring(_endOfLineIndex + 1);

                    _endOfLineIndex = _connectedClient.CommandBuffer.IndexOf('\r');
                }
                while (_endOfLineIndex > -1);
            }
        }
    }
}
