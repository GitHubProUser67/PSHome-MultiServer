using Newtonsoft.Json;
using System.Text.RegularExpressions;
using CustomLogger;

namespace MultiSocks.Aries.DataStore
{
    public class DirtySocksJSONDatabase : IDatabase
    {
        public int AutoInc = 1;
        public List<DbAccount> Accounts = new();
        public HashSet<string> Personas = new();
        public HashSet<string> Friends = new();
        public HashSet<string> Rivals = new();

        public DirtySocksJSONDatabase()
        {
            //load from disk.
            LoggerAccessor.LogInfo("Loading DirtySocks Database...");

            try
            {
                if (File.Exists(MultiSocksServerConfiguration.DirtySocksDatabasePath))
                {
                    using (StreamReader io = new(File.Open(MultiSocksServerConfiguration.DirtySocksDatabasePath, FileMode.Open, FileAccess.Read, FileShare.None)))
                    {
                        var extractedact = JsonConvert.DeserializeObject<List<DbAccount>>(io.ReadToEnd());
                        if (extractedact != null)
                            Accounts = extractedact;
                        io.Close();
                    }
                    Personas.Clear();
                    Friends.Clear();
                    Rivals.Clear();
                    if (Accounts != null)
                    {
                        foreach (var user in Accounts)
                        {
                            foreach (var persona in user.Personas)
                            {
                                Personas.Add(persona);
                            }
                            foreach (var persona in user.Friends)
                            {
                                Friends.Add(persona);
                            }
                            foreach (var persona in user.Rivals)
                            {
                                Rivals.Add(persona);
                            }
                        }
                        if (Accounts.Count > 0)
                            AutoInc = Accounts.Max(x => x.ID) + 1;
                    }
                }
                else
                {
                    LoggerAccessor.LogWarn("Database file not existant. Starting with a blank slate.");
                    Save();
                }
            }
            catch (Exception)
            {
                LoggerAccessor.LogWarn("Error loading database! Starting with a blank slate.");
                Save();
            }
        }

        public bool CreateNew(DbAccount info)
        {
            if (info.Username == null) return false;
            info.Personas.Add(info.Username); // Burnout Paradise seems to want at least one entry which is username itself.
            lock (Accounts)
            {
                if (GetByName(info.Username) != null) return false; //already exists
                info.ID = AutoInc++;
                Accounts.Add(info);
                Save();
            }
            return true;
        }

        public DbAccount? GetByName(string? username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            lock (Accounts)
            {
                return Accounts.FirstOrDefault(x => x.Username == username);
            }
        }

        public int AddPersona(int id, string persona)
        {
            Regex regex = new(@"[a-zA-Z0-9\s]");
            if (!regex.IsMatch(persona)) return -1;
            var index = 0;
            lock (Accounts)
            {
                var acct = Accounts.FirstOrDefault(x => x.ID == id);
                if (acct == null || acct.Personas.Count == 4) return -1;
                if (Personas.Contains(persona)) return -2;
                Personas.Add(persona);
                acct.Personas.Add(persona);
                index = acct.Personas.Count;
                Save();
            }
            return index;
        }

        public int DeletePersona(int id, string persona)
        {
            var index = 0;
            lock (Accounts)
            {
                var acct = Accounts.FirstOrDefault(x => x.ID == id);
                if (acct == null) return -1;
                index = acct.Personas.IndexOf(persona);
                if (index != -1)
                {
                    Personas.Remove(persona);
                    acct.Personas.Remove(persona);
                    Save();
                }
            }
            return index;
        }

        public int AddFriend(int id, string Friend)
        {
            Regex regex = new(@"[a-zA-Z0-9\s]");
            if (!regex.IsMatch(Friend)) return -1;
            var index = 0;
            lock (Accounts)
            {
                var acct = Accounts.FirstOrDefault(x => x.ID == id);
                if (acct == null) return -1;
                if (Friends.Contains(Friend)) return -2;
                Friends.Add(Friend);
                acct.Friends.Add(Friend);
                index = acct.Friends.Count;
                Save();
            }
            return index;
        }

        public int DeleteFriend(int id, string Friend)
        {
            var index = 0;
            lock (Accounts)
            {
                var acct = Accounts.FirstOrDefault(x => x.ID == id);
                if (acct == null) return -1;
                index = acct.Friends.IndexOf(Friend);
                if (index != -1)
                {
                    Friends.Remove(Friend);
                    acct.Friends.Remove(Friend);
                    Save();
                }
            }
            return index;
        }

        public int AddRival(int id, string Rival)
        {
            Regex regex = new(@"[a-zA-Z0-9\s]");
            if (!regex.IsMatch(Rival)) return -1;
            var index = 0;
            lock (Accounts)
            {
                var acct = Accounts.FirstOrDefault(x => x.ID == id);
                if (acct == null) return -1;
                if (Rivals.Contains(Rival)) return -2;
                Rivals.Add(Rival);
                acct.Rivals.Add(Rival);
                index = acct.Rivals.Count;
                Save();
            }
            return index;
        }

        public int DeleteRival(int id, string Rival)
        {
            var index = 0;
            lock (Accounts)
            {
                var acct = Accounts.FirstOrDefault(x => x.ID == id);
                if (acct == null) return -1;
                index = acct.Rivals.IndexOf(Rival);
                if (index != -1)
                {
                    Rivals.Remove(Rival);
                    acct.Rivals.Remove(Rival);
                    Save();
                }
            }
            return index;
        }

        private void Save()
        {
            string data;
            lock (Accounts)
            {
                data = JsonConvert.SerializeObject(Accounts);
            }
            Directory.CreateDirectory(Path.GetDirectoryName(MultiSocksServerConfiguration.DirtySocksDatabasePath));
            using (FileStream file = File.Open(MultiSocksServerConfiguration.DirtySocksDatabasePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter io = new(file))
                {
                    io.Write(data);
                    io.Flush();
                }
            }
        }
    }
}
