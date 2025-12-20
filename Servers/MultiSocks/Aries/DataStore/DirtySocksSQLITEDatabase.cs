using CustomLogger;
using Newtonsoft.Json;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace MultiSocks.Aries.DataStore
{
    public class DirtySocksSQLiteDatabase : IDatabase
    {
        public int AutoInc = 0;
        public List<DbAccount> Accounts = new();
        public HashSet<string> Personas = new();
        public HashSet<string> Friends = new();
        public HashSet<string> Rivals = new();

        public DirtySocksSQLiteDatabase()
        {
            LoggerAccessor.LogInfo("Loading DirtySocks Database...");

            try
            {
                if (File.Exists(MultiSocksServerConfiguration.DirtySocksDatabasePath))
                    Load();
                else
                {
                    LoggerAccessor.LogWarn("Database file not existant. Starting with a blank state.");
                    InitializeDatabase();
                    Save();
                }
            }
            catch (Exception)
            {
                LoggerAccessor.LogWarn($"Error loading database! Starting with a blank state.");
                InitializeDatabase();
                Save();
            }
        }

        private void InitializeDatabase()
        {
            using var connection = new SQLiteConnection($"Data Source={MultiSocksServerConfiguration.DirtySocksDatabasePath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS AccountsJson (
                                Id INTEGER PRIMARY KEY,
                                Json TEXT NOT NULL
                            );";
            cmd.ExecuteNonQuery();

            CreateNew(new DbAccount()
            {
                Username = "brobot24",
                TOS = "1",
                SHARE = "0",
                MAIL = "dummy@ea.com",
                Password = string.Empty,
            });
        }

        private void Load()
        {
            InitializeDatabase();

            using var connection = new SQLiteConnection($"Data Source={MultiSocksServerConfiguration.DirtySocksDatabasePath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Json FROM AccountsJson";
            using var reader = cmd.ExecuteReader();

            Accounts.Clear();
            Personas.Clear();
            Friends.Clear();
            Rivals.Clear();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var json = reader.GetString(1);

                var acct = JsonConvert.DeserializeObject<DbAccount>(json);
                if (acct != null)
                {
                    Accounts.Add(acct);

                    foreach (var p in acct.Personas) Personas.Add(p);
                    foreach (var f in acct.Friends) Friends.Add(f);
                    foreach (var r in acct.Rivals) Rivals.Add(r);
                }
            }

            if (Accounts.Count > 0)
                AutoInc = Accounts.Max(x => x.ID) + 1;
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
            Directory.CreateDirectory(Path.GetDirectoryName(MultiSocksServerConfiguration.DirtySocksDatabasePath));

            using var connection = new SQLiteConnection($"Data Source={MultiSocksServerConfiguration.DirtySocksDatabasePath}");
            connection.Open();

            using var transaction = connection.BeginTransaction();

            var upsertCmd = connection.CreateCommand();
            upsertCmd.CommandText = @"INSERT OR REPLACE INTO AccountsJson (Id, Json)
                                  VALUES ($id, $json)";

            lock (Accounts)
            {
                foreach (var acct in Accounts)
                {
                    var json = JsonConvert.SerializeObject(acct);
                    upsertCmd.Parameters.Clear();
                    upsertCmd.Parameters.AddWithValue("$id", acct.ID);
                    upsertCmd.Parameters.AddWithValue("$json", json);
                    upsertCmd.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }
    }
}
