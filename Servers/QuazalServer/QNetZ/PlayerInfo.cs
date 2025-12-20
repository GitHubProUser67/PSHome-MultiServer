using QuazalServer.QNetZ.DDL;
using System.Linq.Expressions;

namespace QuazalServer.QNetZ
{
	public class PlayerInfo
	{
		public PlayerInfo()
		{
			DataStore = new Dictionary<Type, IPlayerDataStore>();
		}

		public void OnDropped()
		{
			if (CurrentSparkGameId != 0)
			{
				RDVServices.GameServices.PS3SparkServices.SparkProtocolService.RefreshGames(true, this);
            }

			foreach (var ds in DataStore.Values)
				ds.OnDropped();
		}

		public QClient? Client;	// connection info
		public uint PID { get; set; } // printcipal ID
		public uint RVCID { get; set; } // rendez-vous connection ID
		public uint CurrentSparkGameId { get; set; } = 0; // current spark game lobby id
        public uint StationID { get; set; }
        public string AccessKey { get; set; } = string.Empty;
        public string? AccountId { get; set; }
        public string? UbiCountryCode { get; set; }
        public string? UbiLanguageCode { get; set; }
        public string? UbiAcctName { get; set; }
        public string? UbiMail { get; set; }
        public string? UbiPass { get; set; }
        public string? Name { get; set; }

        public StationURL? Url
        {
            get
            {
                if (Client == null)
                    return null;

                return new StationURL(
                    "prudp",
                    Client.Endpoint.Address.ToString(),
                    new Dictionary<string, int>() {
                        { "port", Client.Endpoint.Port },
                        { "RVCID", (int)RVCID },
						//{ "type", 3 }	// TODO: IsPublic and Behind NAT flags
					});
            }
        }

        // game - specific stuff comes here
        public T GetData<T>() where T: class
		{
			IPlayerDataStore? value;

			if (DataStore.TryGetValue(typeof(T), out value))
				return (T)value;

			var createFunc = Expression.Lambda<Func<T>>(
				Expression.New(typeof(T).GetConstructor(new[] { typeof(PlayerInfo) }), new [] { Expression.Constant(this) })
			).Compile();

			DataStore[typeof(T)] = value = (IPlayerDataStore)createFunc();

			return (T)value;
		}

		private Dictionary<Type, IPlayerDataStore> DataStore;
	}

	public interface IPlayerDataStore
	{
		void OnDropped();
	}
}
