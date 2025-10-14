using Newtonsoft.Json;

namespace MultiSocks
{
    public class RoomConfig
    {
        public string Name { get; set; } = string.Empty;

        [JsonProperty("is_global")]
        public bool IsGlobal { get; set; } = false;
    }

    public class ServerConfig
    {
        public string Type { get; set; } = string.Empty;   // Aries, Blaze2, Blaze3
        public string Subtype { get; set; } = string.Empty; // Redirector, Main, Matchmaker, etc.
        public ushort Port { get; set; }

        [JsonProperty("target_ip")]
        public string? TargetIP { get; set; }

        [JsonProperty("target_port")]
        public ushort? TargetPort { get; set; }

        [JsonProperty("listen_ip")]
        public string? ListenIP { get; set; }
        public string? Project { get; set; }
        public string? SKU { get; set; }
        public bool Secure { get; set; } = false;
        public string CN { get; set; } = string.Empty;

        [JsonProperty("weak_chain_signed_rsa_key")]
        public bool WeakChainSignedRSAKey { get; set; } = false;

        [JsonProperty("rooms_to_add")]
        public List<RoomConfig>? RoomsToAdd { get; set; }

        // Blaze-specific
        [JsonProperty("ssl_domain")]
        public string? SSLDomain { get; set; }
        public string? Game { get; set; }
        public List<string>? Components { get; set; }
    }

}
