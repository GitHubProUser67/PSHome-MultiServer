namespace CastleLibrary.Sony.XI5.Verification.Keys
{
    /// <summary>
    /// The public signing key used when RPCS3 clients connect via RPCN.
    /// </summary>
    public class RpcnSigningKey : ITicketSigningKey
    {
        public static readonly RpcnSigningKey Instance = new RpcnSigningKey();
        private RpcnSigningKey() { }

        public string HashAlgorithm => "SHA-224";
        public string CurveTable => "secp224k1";
        public TicketSignatureMessageType MessageType => TicketSignatureMessageType.Body;
        public string PublicKeyX => "b07bc0f0addb97657e9f389039e8d2b9c97dc2a31d3042e7d0479b93";
        public string PublicKeyY => "d81c42b0abdf6c42191a31e31f93342f8f033bd529c2c57fdb5a0a7d";
    }
}
