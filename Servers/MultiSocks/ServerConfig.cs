using Newtonsoft.Json;
using System.Security.Authentication;

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
        [JsonProperty("target_hostname")]
        public string? TargetHostname { get; set; }

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
        public string? Game { get; set; }
        [JsonProperty("storage_encryption_key")]
        public uint StorageEncryptionKey { get; set; } = 0;
        [JsonProperty("ssl_protocols")]
#pragma warning disable
        public int SSLProtocols { get; set; } = (int)(SslProtocols.Default | SslProtocols.Tls11 | SslProtocols.Tls12);
#pragma warning restore
        [JsonProperty("write_client_report_to_file")]
        public bool WriteClientReportToFile { get; set; } = true;
        public List<string>? Components { get; set; }
    }

}
