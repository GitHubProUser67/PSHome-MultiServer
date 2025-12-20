using CustomLogger;
using MultiServerLibrary.GeoLocalization;
using System.Data;
using System.Data.SQLite;
using System.Net;

namespace MultiSpy.Data
{
	public class LoginDatabase : IDisposable
	{
		public static LoginDatabase? _instance;

		private SQLiteConnection? _db;

		private SQLiteCommand? _getUsersByName;
		private SQLiteCommand? _getUsersByEmail;
		private SQLiteCommand? _updateUser;
		private SQLiteCommand? _createUser;
		private SQLiteCommand? _countUsers;
		private SQLiteCommand? _logUser;
		private SQLiteCommand? _logUserUpdateCountry;

		// we're not going to have 100 million users using this login database
		private const int UserIdOffset = 200000000;
		private const int ProfileIdOffset = 100000000;

		private readonly object _dbLock = new object();

		public static void Initialize(string databasePath)
		{
			_instance = new LoginDatabase();

			databasePath = Path.GetFullPath(databasePath);

			if (!File.Exists(databasePath)) {
				SQLiteConnection.CreateFile(databasePath);
			}

			if (File.Exists(databasePath)) {
				SQLiteConnectionStringBuilder connBuilder = new SQLiteConnectionStringBuilder() {
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

				if (_instance._db.State == ConnectionState.Open) {
					bool read = false;
					using (SQLiteCommand queryTables = new SQLiteCommand("SELECT * FROM sqlite_master WHERE type='table' AND name='users'", _instance._db)) {
						using (SQLiteDataReader reader = queryTables.ExecuteReader()) {
							while (reader.Read()) {
								read = true;
								break;
							}
						}
					}

					if (!read) {
						LoggerAccessor.LogWarn("[LoginDatabase] - No database found, creating now");
						using (SQLiteCommand createTables = new SQLiteCommand("CREATE TABLE users ( id INTEGER PRIMARY KEY, name TEXT NOT NULL, password TEXT NOT NULL, email TEXT NOT NULL, country TEXT NOT NULL, lastip TEXT NOT NULL, lasttime INTEGER NULL DEFAULT '0', session INTEGER NULL DEFAULT '0' )", _instance._db)) {
							createTables.ExecuteNonQuery();
						}
                        LoggerAccessor.LogInfo("[LoginDatabase] - Using " + databasePath);
						_instance.PrepareStatements();
						return;
					} else {
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
			_getUsersByName = new SQLiteCommand("SELECT id, password, email, country, session FROM users WHERE name=@name COLLATE NOCASE", _db);
			_getUsersByName.Parameters.Add("@name", DbType.String);

			_getUsersByEmail = new SQLiteCommand("SELECT id, name, country, session FROM users WHERE email=@email AND password=@password", _db);
			_getUsersByEmail.Parameters.Add("@email", DbType.String);
			_getUsersByEmail.Parameters.Add("@password", DbType.String);

			_updateUser = new SQLiteCommand("UPDATE users SET password=@pass, email=@email, country=@country, session=@session WHERE name=@name COLLATE NOCASE", _db);
			_updateUser.Parameters.Add("@pass", DbType.String);
			_updateUser.Parameters.Add("@email", DbType.String);
			_updateUser.Parameters.Add("@country", DbType.String);
			_updateUser.Parameters.Add("@session", DbType.Int64);
			_updateUser.Parameters.Add("@name", DbType.String);

			_createUser = new SQLiteCommand("INSERT INTO users (name, password, email, country, lastip) VALUES ( @name, @pass, @email, @country, @ip )", _db);
			_createUser.Parameters.Add("@name", DbType.String);
			_createUser.Parameters.Add("@pass", DbType.String);
			_createUser.Parameters.Add("@email", DbType.String);
			_createUser.Parameters.Add("@country", DbType.String);
			_createUser.Parameters.Add("@ip", DbType.String);

			_countUsers = new SQLiteCommand("SELECT COUNT(*) FROM users WHERE name=@name COLLATE NOCASE", _db);
			_countUsers.Parameters.Add("@name", DbType.String);

			_logUser = new SQLiteCommand("UPDATE users SET lastip=@ip, lasttime=@time WHERE name=@name COLLATE NOCASE", _db);
			_logUser.Parameters.Add("@ip", DbType.String);
			_logUser.Parameters.Add("@time", DbType.Int64);
			_logUser.Parameters.Add("@name", DbType.String);

			_logUserUpdateCountry = new SQLiteCommand("UPDATE users SET country=@country, lastip=@ip, lasttime=@time WHERE name=@name COLLATE NOCASE", _db);
			_logUserUpdateCountry.Parameters.Add("@country", DbType.String);
			_logUserUpdateCountry.Parameters.Add("@ip", DbType.String);
			_logUserUpdateCountry.Parameters.Add("@time", DbType.Int64);
			_logUserUpdateCountry.Parameters.Add("@name", DbType.String);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			try {
				if (disposing) {
					if (_getUsersByName != null) {
						_getUsersByName.Dispose();
						_getUsersByName = null;
					}
					if (_getUsersByEmail != null) {
						_getUsersByEmail.Dispose();
						_getUsersByEmail = null;
					}
					if (_updateUser != null) {
						_updateUser.Dispose();
						_updateUser = null;
					}
					if (_createUser != null) {
						_createUser.Dispose();
						_createUser = null;
					}
					if (_countUsers != null) {
						_countUsers.Dispose();
						_countUsers = null;
					}
					if (_logUser != null) {
						_logUser.Dispose();
						_logUser = null;
					}
					if (_logUserUpdateCountry != null) {
						_logUserUpdateCountry.Dispose();
						_logUserUpdateCountry = null;
					}
					if (_db != null) {
						_db.Close();
						_db.Dispose();
						_db = null;
					}
					_instance = null;

					if (_instance != null) {
						_instance.Dispose();
						_instance = null;
					}
				}
			} catch (Exception) {
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
				if (_instance == null) {
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

			lock (_dbLock) {
				_getUsersByName.Parameters["@name"].Value = username;

				using (SQLiteDataReader reader = _getUsersByName.ExecuteReader()) {
					if (reader.Read()) {
						// only go once

						Dictionary<string, object> data = new Dictionary<string, object>();
						data.Add("id", reader["id"]);
						data.Add("name", username);
						data.Add("passwordenc", reader["password"]);
						data.Add("email", reader["email"]);
						data.Add("country", reader["country"]);
						data.Add("userid", (Int64)reader["id"] + UserIdOffset);
						data.Add("profileid", (Int64)reader["id"] + ProfileIdOffset);
						data.Add("session", reader["session"]);

						return data;
					}
				}
			}

			return null;
		}

		public List<Dictionary<string, object>> GetData(string email, string passwordEncrypted)
		{
			if (_db == null)
				return null;

			List<Dictionary<string, object>> values = new List<Dictionary<string, object>>();

			lock (_dbLock) {
				_getUsersByEmail.Parameters["@email"].Value = email.ToLowerInvariant();
				_getUsersByEmail.Parameters["@password"].Value = passwordEncrypted;

				using (SQLiteDataReader reader = _getUsersByEmail.ExecuteReader()) {
					while (reader.Read()) {
						// loop through all nicks associated with that email/pass combo

						Dictionary<string, object> data = new Dictionary<string, object>();
						data.Add("id", reader["id"]);
						data.Add("name", reader["name"]);
						data.Add("passwordenc", passwordEncrypted);
						data.Add("email", email);
						data.Add("country", reader["country"]);
						data.Add("userid", (Int64)reader["id"] + UserIdOffset);
						data.Add("profileid", (Int64)reader["id"] + ProfileIdOffset);
						data.Add("session", reader["session"]);

						values.Add(data);
					}
				}
			}

			return values;
		}

		public void SetData(string name, Dictionary<string, object> data)
		{
			var oldValues = GetData(name);

			if (oldValues == null)
				return;

			lock (_dbLock) {
				_updateUser.Parameters["@pass"].Value = data.ContainsKey("passwordenc") ? data["passwordenc"] : oldValues["passwordenc"];
				_updateUser.Parameters["@email"].Value = data.ContainsKey("email") ? ((string)data["email"]).ToLowerInvariant() : oldValues["email"];
				_updateUser.Parameters["@country"].Value = data.ContainsKey("country") ? data["country"].ToString().ToUpperInvariant() : oldValues["country"];
				_updateUser.Parameters["@session"].Value = data.ContainsKey("session") ? data["session"] : oldValues["session"];
				_updateUser.Parameters["@name"].Value = name;

				_updateUser.ExecuteNonQuery();
			}
		}

		public void LogLogin(string name, IPAddress address)
		{
			if (_db == null)
				return;

			var data = GetData(name);
			if (data == null)
				return;

			// for some reason, when creating an account, sometimes the country doesn't get set
			// it gets set to ?? which is the default. probably the message didn't make it through or something
			// but anyway, if it doesn't match what's in the db, then we want to update the country field to the user's
			// country as defined by IP address
			// to save on db writes, we do this as part of logging the ip/time

			string country = "??";
			if (GeoIP.Instance != null && GeoIP.Instance.Reader != null) {
				try
				{
                    var isoCode = GeoIP.GetISOCodeFromIP(address);
                    country = string.IsNullOrEmpty(isoCode) ? "??" : isoCode.ToUpperInvariant();
                }
				catch
				{

				}
			}

			if (country != "??" && !data["country"].ToString().Equals(country, StringComparison.InvariantCultureIgnoreCase)) {
				lock (_dbLock) {

					_logUserUpdateCountry.Parameters["@country"].Value = country;
					_logUserUpdateCountry.Parameters["@ip"].Value = address.ToString();
					_logUserUpdateCountry.Parameters["@time"].Value = (DateTime.UtcNow.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
					_logUserUpdateCountry.Parameters["@name"].Value = name;

					_logUserUpdateCountry.ExecuteNonQuery();
				}
			} else {
				lock (_dbLock) {
					_logUser.Parameters["@ip"].Value = address.ToString();
					_logUser.Parameters["@time"].Value = (DateTime.UtcNow.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
					_logUser.Parameters["@name"].Value = name;

					_logUser.ExecuteNonQuery();
				}
			}
		}

		public void CreateUser(string username, string passwordEncrypted, string email, string country, IPAddress address)
		{
			if (_db == null)
				return;

			if (UserExists(username))
				return;

			lock (_dbLock) {
				_createUser.Parameters["@name"].Value = username;
				_createUser.Parameters["@pass"].Value = passwordEncrypted;
				_createUser.Parameters["@email"].Value = email.ToLowerInvariant();
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

			lock (_dbLock) {
				_countUsers.Parameters["@name"].Value = username;

				using (SQLiteDataReader reader = _countUsers.ExecuteReader()) {
					if (reader.Read()) {
						// only go once

						if (reader.FieldCount == 1 && (Int64)reader[0] == 1) {
							existing = true;
						}
					}
				}
			}

			return existing;
		}
	}
}
