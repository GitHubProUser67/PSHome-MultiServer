using CustomLogger;
using MultiServerLibrary.GeoLocalization;
using System.Data.SQLite;
using System.Data;
using System.Net;

namespace EdenServer.Database
{
    public class LoginDatabase : IDisposable
    {
        public static LoginDatabase? _instance;

        private SQLiteConnection? _db;

        private SQLiteCommand? _getUsersByName;
        private SQLiteCommand? _updateUser;
        private SQLiteCommand? _createUser;
        private SQLiteCommand? _countUsers;
        private SQLiteCommand? _logUser;

        private readonly object _dbLock = new object();

        public static void Initialize(string databasePath)
        {
            _instance = new LoginDatabase();

            databasePath = Path.GetFullPath(databasePath);

            Directory.CreateDirectory(Path.GetDirectoryName(databasePath));

            if (!File.Exists(databasePath))
                SQLiteConnection.CreateFile(databasePath);

            if (File.Exists(databasePath))
            {
                SQLiteConnectionStringBuilder connBuilder = new SQLiteConnectionStringBuilder()
                {
                    DataSource = databasePath,
                    Version = 3,
                    PageSize = 4096,
                    CacheSize = 10000,
                    JournalMode = SQLiteJournalModeEnum.Wal,
                    LegacyFormat = false,
                    DefaultTimeout = 500
                };

                _instance._db = new SQLiteConnection(connBuilder.ToString());
                _instance._db.Open();

                if (_instance._db.State == ConnectionState.Open)
                {
                    bool read = false;
                    using (SQLiteCommand queryTables = new SQLiteCommand("SELECT * FROM sqlite_master WHERE type='table' AND name='users'", _instance._db))
                    {
                        using (SQLiteDataReader reader = queryTables.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                read = true;
                                break;
                            }
                        }
                    }

                    if (!read)
                    {
                        LoggerAccessor.LogWarn("[LoginDatabase] - No database found, creating now");
                        using (SQLiteCommand createTables = new SQLiteCommand(@"CREATE TABLE users (
                            id INTEGER PRIMARY KEY,
                            name TEXT NOT NULL,
                            password TEXT NOT NULL,
                            userid INTEGER NOT NULL,
                            XUID INTEGER NOT NULL,
                            unk2 INTEGER NOT NULL,
                            gamekey TEXT NOT NULL,
                            megapackkey TEXT NULL,
                            country TEXT NOT NULL,
                            lastip TEXT NOT NULL
                        )", _instance._db))
                        {
                            createTables.ExecuteNonQuery();
                        }
                        LoggerAccessor.LogInfo("[LoginDatabase] - Using " + databasePath);
                        _instance.PrepareStatements();
                        return;
                    }
                    else
                    {
                        LoggerAccessor.LogInfo("[LoginDatabase] - Using " + databasePath);
                        _instance.PrepareStatements();
                        return;
                    }
                }
            }

            LoggerAccessor.LogError("[LoginDatabase] - Error creating database");
            _instance.Dispose();
            _instance = null;
        }

        private void PrepareStatements()
        {
            _getUsersByName = new SQLiteCommand("SELECT id, password, userid, XUID, unk2, gamekey, megapackkey FROM users WHERE name=@name COLLATE NOCASE", _db);
            _getUsersByName.Parameters.Add("@name", DbType.String);

            _updateUser = new SQLiteCommand("UPDATE users SET password=@pass, country=@country WHERE name=@name COLLATE NOCASE", _db);
            _updateUser.Parameters.Add("@pass", DbType.String);
            _updateUser.Parameters.Add("@country", DbType.String);
            _updateUser.Parameters.Add("@name", DbType.String);

            _createUser = new SQLiteCommand("INSERT INTO users (name, password, userid, XUID, unk2, gamekey, megapackkey, country, lastip) VALUES ( @name, @pass, @userid, @XUID, @unk2, @gamekey, @megapackkey, @country, @ip)", _db);
            _createUser.Parameters.Add("@name", DbType.String);
            _createUser.Parameters.Add("@pass", DbType.String);
            _createUser.Parameters.Add("@userid", DbType.UInt32);
            _createUser.Parameters.Add("@XUID", DbType.UInt64);
            _createUser.Parameters.Add("@unk2", DbType.UInt16);
            _createUser.Parameters.Add("@gamekey", DbType.String);
            _createUser.Parameters.Add("@megapackkey", DbType.String);
            _createUser.Parameters.Add("@country", DbType.String);
            _createUser.Parameters.Add("@ip", DbType.String);

            _countUsers = new SQLiteCommand("SELECT COUNT(*) FROM users WHERE name=@name COLLATE NOCASE", _db);
            _countUsers.Parameters.Add("@name", DbType.String);

            _logUser = new SQLiteCommand("UPDATE users SET lastip=@ip, country=@country WHERE name=@name COLLATE NOCASE", _db);
            _logUser.Parameters.Add("@ip", DbType.String);
            _logUser.Parameters.Add("@country", DbType.String);
            _logUser.Parameters.Add("@name", DbType.String);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_getUsersByName != null)
                    {
                        _getUsersByName.Dispose();
                        _getUsersByName = null;
                    }
                    if (_updateUser != null)
                    {
                        _updateUser.Dispose();
                        _updateUser = null;
                    }
                    if (_createUser != null)
                    {
                        _createUser.Dispose();
                        _createUser = null;
                    }
                    if (_countUsers != null)
                    {
                        _countUsers.Dispose();
                        _countUsers = null;
                    }
                    if (_logUser != null)
                    {
                        _logUser.Dispose();
                        _logUser = null;
                    }
                    if (_db != null)
                    {
                        _db.Close();
                        _db.Dispose();
                        _db = null;
                    }
                    _instance = null;

                    if (_instance != null)
                    {
                        _instance.Dispose();
                        _instance = null;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        ~LoginDatabase()
        {
            Dispose(false);
        }

        public static bool IsInitialized()
        {
            return _instance != null && _instance._db != null;
        }

        public static LoginDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new ArgumentNullException("Instance", "Initialize() must be called first");
                }

                return _instance;
            }
        }

        public Dictionary<string, object>? GetData(string username)
        {
            if (_db == null)
                return null;

            if (!UserExists(username))
                return null;

            lock (_dbLock)
            {
                _getUsersByName.Parameters["@name"].Value = username;

                using (SQLiteDataReader reader = _getUsersByName.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // only go once

                        Dictionary<string, object> data = new Dictionary<string, object>();
                        data.Add("id", reader["id"]);
                        data.Add("name", username);
                        data.Add("password", reader["password"]);
                        data.Add("userid", reader["userid"]);
                        data.Add("XUID", reader["XUID"]);
                        data.Add("unk2", reader["unk2"]);
                        data.Add("gamekey", reader["gamekey"]);
                        data.Add("megapackkey", reader["megapackkey"]);

                        return data;
                    }
                }
            }

            return null;
        }

        public void SetData(string name, Dictionary<string, object> data)
        {
            var oldValues = GetData(name);

            if (oldValues == null)
                return;

            lock (_dbLock)
            {
                _updateUser.Parameters["@pass"].Value = data.ContainsKey("password") ? data["password"] : oldValues["password"];
                _updateUser.Parameters["@country"].Value = data.ContainsKey("country") ? data["country"].ToString().ToUpperInvariant() : oldValues["country"];
                _updateUser.Parameters["@name"].Value = name;

                _updateUser.ExecuteNonQuery();
            }
        }

        public bool LogLogin(string name, IPAddress address)
        {
            if (_db == null)
                return false;

            var data = GetData(name);
            if (data == null)
                return false;

            string country = "??";
            if (GeoIP.Instance != null && GeoIP.Instance.Reader != null)
            {
                try
                {
                    country = GeoIP.GetISOCodeFromIP(address).ToUpperInvariant();
                }
                catch
                {

                }
            }

            lock (_dbLock)
            {
                _logUser.Parameters["@country"].Value = country;
                _logUser.Parameters["@ip"].Value = address.ToString();
                _logUser.Parameters["@name"].Value = name;

                _logUser.ExecuteNonQuery();
            }

            return true;
        }

        public void CreateUser(string username, string password, uint userId, ulong XUID, byte unk2, string gameKey, string megapackKey, string country, IPAddress address)
        {
            if (_db == null)
                return;

            if (UserExists(username))
                return;

            lock (_dbLock)
            {
                _createUser.Parameters["@name"].Value = username;
                _createUser.Parameters["@pass"].Value = password;
                _createUser.Parameters["@userid"].Value = userId;
                _createUser.Parameters["@XUID"].Value = XUID;
                _createUser.Parameters["@unk2"].Value = unk2;
                _createUser.Parameters["@gamekey"].Value = gameKey;
                _createUser.Parameters["@megapackkey"].Value = megapackKey;
                _createUser.Parameters["@country"].Value = country.ToUpperInvariant();
                _createUser.Parameters["@ip"].Value = address.ToString();

                _createUser.ExecuteNonQuery();
            }
        }

        public bool UserExists(string username)
        {
            bool existing = false;

            if (_db == null)
                return false;

            lock (_dbLock)
            {
                _countUsers.Parameters["@name"].Value = username;

                using (SQLiteDataReader reader = _countUsers.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // only go once

                        if (reader.FieldCount == 1 && (Int64)reader[0] == 1)
                        {
                            existing = true;
                        }
                    }
                }
            }

            return existing;
        }
    }
}
