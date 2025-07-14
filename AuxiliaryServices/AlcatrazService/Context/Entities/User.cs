using Newtonsoft.Json;

namespace Alcatraz.Context.Entities
{
	public class User
	{
		public uint Id { get; set; }
		
		public string Username { get; set; }
		public string PlayerNickName { get; set; }
		[JsonIgnore]
		public string Password { get; set; }
        [JsonIgnore]
        public string MACAddress { get; set; }
        public int RewardFlags { get; set; }
        public byte[] PrivateData { get; set; }
        public byte[] PublicData { get; set; }
        public int UbiTokens { get; set; }
        public string UbiData { get; set; }
    }
}
