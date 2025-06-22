namespace XI5.Verification.Keys
{
    /// <summary>
    /// A public signing key used when PS3/PSVita clients connect via official PSN.
    /// Since each game series has a different set of curves, you must provide them yourself in a new class.
    /// </summary>
    public abstract class PsnSigningKey : ITicketSigningKey
    {
        public string HashAlgorithm => "SHA-1";
        public string CurveTable => "secp192r1";
        public TicketSignatureMessageType MessageType => TicketSignatureMessageType.Ticket;
        public abstract string PublicKeyX { get; }
        public abstract string PublicKeyY { get; }
    }
}
