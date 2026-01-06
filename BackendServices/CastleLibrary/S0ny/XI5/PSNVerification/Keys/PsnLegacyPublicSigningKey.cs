namespace CastleLibrary.S0ny.XI5.PSNVerification.Keys
{
    /// <summary>
    /// used when PS3/PSVita clients connects to PSN.
    /// </summary>
    public abstract class PsnLegacyPublicSigningKey : ITicketPublicSigningKey
    {
        public string Curve => "secp192r1";
        public abstract string Xhex { get; }
        public abstract string Yhex { get; }
    }
}
