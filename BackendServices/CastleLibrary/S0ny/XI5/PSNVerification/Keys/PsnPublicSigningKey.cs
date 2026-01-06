namespace CastleLibrary.S0ny.XI5.PSNVerification.Keys
{
    /// <summary>
    /// used in Clans server.
    /// </summary>
    public abstract class PsnPublicSigningKey : ITicketPublicSigningKey
    {
        public string Curve => "secp256r1";
        public abstract string Xhex { get; }
        public abstract string Yhex { get; }
    }
}
