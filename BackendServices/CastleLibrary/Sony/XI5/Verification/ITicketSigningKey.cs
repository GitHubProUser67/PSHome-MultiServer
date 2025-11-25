namespace CastleLibrary.Sony.XI5.Verification
{
    /// <summary>
    /// Defines the parameters for a key used for verification.
    /// </summary>
    public interface ITicketSigningKey
    {
        string HashAlgorithm { get; }
        string CurveTable { get; }
        TicketSignatureMessageType MessageType { get; }
        string PublicKeyX { get; }
        string PublicKeyY { get; }
    }
}
