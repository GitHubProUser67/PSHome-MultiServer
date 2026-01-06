using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using BlazeCommon;

namespace MultiSocks.Blaze.MassEffect3PS3Components.Model
{
    public static class ME3Player
    {
        public struct NETDATA
        {
            public uint IP;
            public uint PORT;
        }

        private static UniqueIDGenerator counter = new UniqueIDGenerator();

        public static List<ME3PlayerInfo> AllPlayers = new List<ME3PlayerInfo>();

        public static readonly object _sync = new object();

        public class ME3PlayerInfo
        {
            public struct SettingEntry
            {
                public string Key;
                public string Data;
            }

            public int ID;
            public long UserID = 0;
            public long PlayerID = 0;
            public string AuthString;
            public string Auth2String;
            public string Name;
            public string GameState;
            public string CurrentGame;
            public string IP;
            public string PORT;
            public string SERVER_IP;
            public string SERVER_PORT;
            public string pathtoprofile;
            public ProtoFireConnection Client;
            public Stream ClientStream;
            public bool isActive = true;
            public bool Update = false;
            public bool SendOffers = false;
            public bool WaitsForJoining = false;
            public NETDATA EXIP;
            public NETDATA INIP;
            public Stopwatch PingTimer;

            public List<SettingEntry> Settings;
            public string timestring;

            public ME3PlayerInfo(ProtoFireConnection client, Stream clientstream)
            {
                ID = (int)counter.CreateSequentialID();
                Client = client;
                ClientStream = clientstream;
                GameState = "boot";
                PingTimer = new Stopwatch();
                PingTimer.Start();
                IP = ((IPEndPoint)Client.Socket.RemoteEndPoint).Address.ToString();
                PORT = ((IPEndPoint)Client.Socket.RemoteEndPoint).Port.ToString();
                SERVER_IP = ((IPEndPoint)Client.Socket.LocalEndPoint).Address.ToString();
                SERVER_PORT = ((IPEndPoint)Client.Socket.LocalEndPoint).Address.ToString();
                Settings = new List<SettingEntry>();
                timestring = string.Format(@"{0:yyyy-MM-dd_HHmmss}", DateTime.Now);
            }

            public uint GetRemoteIPvalue()
            {
                byte[] byteip = ((IPEndPoint)Client.Socket.RemoteEndPoint).Address.GetAddressBytes();
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(byteip);
                return BitConverter.ToUInt32(byteip, 0);
            }

            public void UpdateSettings(string key, string data)
            {
                lock (_sync)
                {
                    SettingEntry newset = new SettingEntry();
                    newset.Key = key;
                    newset.Data = data;
                    bool found = false;
                    for (int i = 0; i < Settings.Count; i++)
                        if (Settings[i].Key == key)
                        {
                            Settings[i] = newset;
                            found = true;
                            break;
                        }
                    if (!found)
                        Settings.Add(newset);
                    if (!string.IsNullOrEmpty(pathtoprofile))
                    {
                        string[] lines = File.ReadAllLines(pathtoprofile);
                        List<string> result = new List<string>();
                        for (int i = 0; i < 5; i++)
                            result.Add(lines[i]);
                        foreach (SettingEntry set in Settings)
                            result.Add(set.Key + "=" + set.Data);
                        File.WriteAllLines(pathtoprofile, result.ToArray());
                    }
                    Update = true;
                }
            }

            public string GetSettings()
            {
                lock (_sync)
                {
                    string res = "";
                    foreach (SettingEntry set in Settings)
                        res += "  " + set.Key + " = " + set.Data + "\n";
                    return res;
                }
            }

            public void SetJoinWaitState(bool state)
            {
                WaitsForJoining = state;
            }

            public void SetActiveState(bool state)
            {
                isActive = state;
            }
        }
    }
}
