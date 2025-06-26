using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XI5.Reader;
using XI5.Types;
using XI5.Types.Parsers;
using XI5.Verification;
using CustomLogger;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Parameters;

namespace XI5
{
    // https://www.psdevwiki.com/ps3/X-I-5-Ticket
    // https://github.com/RipleyTom/rpcn/blob/master/src/server/client/ticket.rs
    // https://github.com/LittleBigRefresh/NPTicket/tree/main

    public partial class XI5Ticket
    {
        // constructor
        public XI5Ticket() { }

        // fields
        public TicketVersion Version { get; set; }
        public string SerialId { get; set; }
        public uint IssuerId { get; set; }

        public DateTimeOffset IssuedDate { get; set; }
        public DateTimeOffset ExpiryDate { get; set; }

        public ulong UserId { get; set; }
        public string Username { get; set; }

        public string Country { get; set; }
        public string Domain { get; set; }
        
        public string ServiceId { get; set; }
        public string TitleId { get; set; }

        public uint Status { get; set; }
        public ushort TicketLength { get; set; }
        public TicketDataSection BodySection { get; set; }

        public string SignatureIdentifier { get; set; }
        public byte[] SignatureData { get; set; }
        public byte[] HashedMessage { get; set; } = Array.Empty<byte>();
        public byte[] Message { get; set; } = Array.Empty<byte>();
        public string HashName { get; set; } = string.Empty;
        public string CurveName { get; set; } = string.Empty;
        public BigInteger R { get; set; } = BigInteger.Zero;
        public BigInteger S { get; set; } = BigInteger.Zero;
        public bool Valid { get; protected set; }
#if NET7_0_OR_GREATER
        internal static readonly Regex ServiceIdRegex = GeneratedRegex();
#else
        internal static readonly Regex ServiceIdRegex = new Regex("(?<=-)[A-Z0-9]{9}(?=_)", RegexOptions.Compiled);
#endif
        public static XI5Ticket ReadFromBytes(byte[] ticketData)
        {
            using (MemoryStream ms = new MemoryStream(ticketData))
                return ReadFromStream(ms);
        }

        private static XI5Ticket ReadFromStream(Stream ticketStream)
        {
            // ticket version (2 bytes), header (4 bytes), ticket length (2 bytes) = 8 bytes
            const int headerLength = sizeof(byte) + sizeof(byte) + sizeof(uint) + sizeof(ushort);

            byte[] ticketData;
            if (ticketStream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> buffer))
                ticketData = buffer.Array.Take((int)ms.Length).ToArray();
            else
            {
                using (MemoryStream tempMs = new MemoryStream())
                {
                    ticketStream.CopyTo(tempMs);
                    ticketData = tempMs.ToArray();
                }

                // reset stream position
                ticketStream.Position = 0;
            }

            XI5Ticket ticket = new XI5Ticket();

            using (TicketReader reader = new TicketReader(ticketStream))
            {
                ticket.Version = reader.ReadTicketVersion();
                ticket.TicketLength = reader.ReadTicketHeader();

                long bodyStart = reader.BaseStream.Position;

                long actualLength = ticketStream.Length - headerLength;
                if (ticket.TicketLength > actualLength)
                {
                    LoggerAccessor.LogError($"[XI5Ticket] - Expected ticket length to be at least {ticket.TicketLength} bytes, but was {actualLength} bytes.");
                    return null;
                }
                else if (ticket.TicketLength < actualLength)
                {
                    byte[] trimmedTicket = new byte[ticket.TicketLength + headerLength];
                    Array.Copy(ticketData, 0, trimmedTicket, 0, trimmedTicket.Length);
                    return ReadFromBytes(trimmedTicket);
                }

                ticket.BodySection = reader.ReadTicketSectionHeader();
                if (ticket.BodySection.Type != TicketDataSectionType.Body)
                {
                    LoggerAccessor.LogError($"[XI5Ticket] - Expected first section to be {nameof(TicketDataSectionType.Body)}, but was {ticket.BodySection.Type} ({(int)ticket.BodySection.Type}).");
                    return null;
                }

                // ticket 2.1
                if (ticket.Version.Major == 2 && ticket.Version.Minor == 1)
                    TicketParser21.ParseTicket(ticket, reader);

                // ticket 3.0
                else if (ticket.Version.Major == 3 && ticket.Version.Minor == 0)
                    TicketParser30.ParseTicket(ticket, reader);

                // unhandled ticket version
                else
                    throw new FormatException($"[XI5Ticket] - Unknown/unhandled ticket version {ticket.Version}.");

                var footer = reader.ReadTicketSectionHeader();
                if (footer.Type != TicketDataSectionType.Footer)
                {
                    LoggerAccessor.LogError($"[XI5Ticket] - Expected last section to be {nameof(TicketDataSectionType.Footer)}, but was {footer.Type} ({(int)footer.Type}).");
                    return null;
                }

                ticket.SignatureIdentifier = reader.ReadTicketStringData(TicketDataType.Binary);
                ticket.SignatureData = reader.ReadTicketBinaryData();

                if (ticket.SignatureData.Length == 56)
                {
#if NET6_0_OR_GREATER
                    ticket.Message = ticketData.AsSpan()[..ticketData.AsSpan().IndexOf(ticket.SignatureData)].ToArray();
#else
                    int index = IndexOfSequence(ticketData, ticket.SignatureData);
                    if (index >= 0)
                    {
                        byte[] message = new byte[index];
                        Array.Copy(ticketData, 0, message, 0, index);
                        ticket.Message = message;
                    }
#endif
                    ticket.HashedMessage = NetHasher.DotNetHasher.ComputeSHA1(ticket.Message);
                    ticket.HashName = "SHA1";
                    ticket.CurveName = "secp192r1";
                }
                else
                {
#if NET6_0_OR_GREATER
                    ticket.Message = ticketData.AsSpan().Slice((int)bodyStart, ticket.BodySection.Length + 4).ToArray();
#else
                    ticket.Message = new byte[ticket.BodySection.Length + 4];
                    Array.Copy(ticketData, (int)bodyStart, ticket.Message, 0, ticket.BodySection.Length + 4);
#endif
                    ticket.HashedMessage = NetHasher.DotNetHasher.ComputeSHA224(ticket.Message);
                    ticket.HashName = "SHA224";
                    ticket.CurveName = "secp224k1";
                }
            }

            DateTimeOffset validityCheckTime = DateTimeOffset.UtcNow;
            bool isValidTimestamp = ticket.IssuedDate <= validityCheckTime && ticket.ExpiryDate > validityCheckTime;

            // verify ticket signature
            ticket.Valid = SigningKeyResolver.GetSigningKeys(ticket.SignatureIdentifier, ticket.TitleId).Any(key =>
               new TicketVerifier(ticketData, ticket, key).IsTicketValid()) && isValidTimestamp;

            if (!isValidTimestamp)
            {
                LoggerAccessor.LogError($"[XI5Ticket] - Timestamp of the ticket data was invalid, likely an exploit. (IssuedDate:{ticket.IssuedDate} ExpiryDate:{ticket.ExpiryDate} CurrentTime:{validityCheckTime}");
                return ticket;
            }

            // ticket invalid
#if DEBUG
            if (!ticket.Valid)
            {
                LoggerAccessor.LogWarn($"[XI5Ticket] - Invalid ticket data sent at:{DateTime.Now} with payload:{{{BytesToHex(ticketData)}}}");

                var curveCache = new Dictionary<string, ECDomainParameters>();
                var validPoints = new List<Org.BouncyCastle.Math.EC.ECPoint>();

                ECDomainParameters curve = curveCache.ContainsKey(ticket.CurveName) ? curveCache[ticket.CurveName] : EcdsaFinder.CurveFromName(ticket.CurveName);
                if (!curveCache.ContainsKey(ticket.CurveName))
                    curveCache.Add(ticket.CurveName, curve);

                byte[] sigBackup = ticket.SignatureData;
                Asn1Sequence sig = ParseSignature(ticket);

                if (sig == null || sig.Count != 2)
                {
                    LoggerAccessor.LogWarn($"[XI5Ticket] - Ticket for {ticket.TitleId} has invalid signature!\nsig: {BytesToHex(ticket.SignatureData)}\norig sig: {BytesToHex(sigBackup)}");
                    return ticket;
                }

                ticket.R = ((DerInteger)sig[0]).PositiveValue;
                ticket.S = ((DerInteger)sig[1]).PositiveValue;

                validPoints.AddRange(EcdsaFinder.RecoverPublicKey(curve, ticket));

                LoggerAccessor.LogWarn($"[XI5Ticket] - Valid points: {validPoints.Count}");

                var alreadyChecked = new List<Org.BouncyCastle.Math.EC.ECPoint>();
                foreach (Org.BouncyCastle.Math.EC.ECPoint p in validPoints)
                {
                    if (alreadyChecked.Contains(p)) continue;
                    Org.BouncyCastle.Math.EC.ECPoint normalized = p.Normalize();
                    int count = validPoints.Count(x =>
                        x.Normalize().AffineXCoord.Equals(normalized.AffineXCoord) &&
                        x.Normalize().AffineYCoord.Equals(normalized.AffineYCoord));
                    if (count <= 1 && validPoints.Count > 2) continue;

                    LoggerAccessor.LogWarn("[XI5Ticket] - =====");
                    LoggerAccessor.LogWarn($"[XI5Ticket] - {normalized.AffineXCoord}");
                    LoggerAccessor.LogWarn($"[XI5Ticket] - {normalized.AffineYCoord}");
                    LoggerAccessor.LogWarn($"[XI5Ticket] - n={count}");
                    LoggerAccessor.LogWarn("[XI5Ticket] - =====");
                    alreadyChecked.Add(p);
                }

                if (alreadyChecked.Count == 0)
                    LoggerAccessor.LogWarn("[XI5Ticket] - all points are unique :(");
            }
#endif
            return ticket;
        }

        private static string BytesToHex(byte[] bytes)
        {
#if NET6_0_OR_GREATER
            return Convert.ToHexString(bytes);
#else
            return string.Concat(bytes.Select(b => b.ToString("X2")));
#endif
        }
#if !NET6_0_OR_GREATER
        private static int IndexOfSequence(byte[] array, byte[] sequence)
        {
            if (sequence.Length == 0)
                return -1;

            for (int i = 0; i <= array.Length - sequence.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < sequence.Length; j++)
                {
                    if (array[i + j] != sequence[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    return i;
            }
            return -1;
        }
#endif
        private static Asn1Sequence ParseSignature(XI5Ticket ticket)
        {
            for (byte i = 0; i < 3; i++)
            {
                try
                {
                    Asn1Object.FromByteArray(ticket.SignatureData);
                    break;
                }
                catch
                {
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    ticket.SignatureData = ticket.SignatureData.SkipLast(1).ToArray();
#else
                    if (ticket.SignatureData.Length > 0)
                        ticket.SignatureData = ticket.SignatureData.Take(ticket.SignatureData.Length - 1).ToArray();
#endif
                }
            }
            return (Asn1Sequence)Asn1Object.FromByteArray(ticket.SignatureData);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Version: {Version}");
            sb.AppendLine($"SerialId: {SerialId}");
            sb.AppendLine($"IssuerId: {IssuerId}");
            sb.AppendLine($"IssuedDate: {IssuedDate}");
            sb.AppendLine($"ExpiryDate: {ExpiryDate}");
            sb.AppendLine($"UserId: {UserId}");
            sb.AppendLine($"Username: {Username}");
            sb.AppendLine($"Country: {Country}");
            sb.AppendLine($"Domain: {Domain}");
            sb.AppendLine($"ServiceId: {ServiceId}");
            sb.AppendLine($"TitleId: {TitleId}");
            sb.AppendLine($"Status: {Status}");
            sb.AppendLine($"TicketLength: {TicketLength}");
            sb.AppendLine($"SignatureIdentifier: {SignatureIdentifier}");
            sb.AppendLine($"SignatureData: {(SignatureData != null ? BitConverter.ToString(SignatureData).Replace("-", string.Empty) : "null")}");
            sb.AppendLine($"Message: {(Message != null ? BitConverter.ToString(Message).Replace("-", string.Empty) : "null")}");
            sb.AppendLine($"HashedMessage: {(HashedMessage != null ? BitConverter.ToString(HashedMessage).Replace("-", string.Empty) : "null")}");
            sb.AppendLine($"HashName: {HashName}");
            sb.AppendLine($"CurveName: {CurveName}");
            sb.AppendLine($"R: {R}");
            sb.AppendLine($"S: {S}");
            sb.AppendLine($"Valid: {Valid}");

            return sb.ToString();
        }
#if NET7_0_OR_GREATER

        [GeneratedRegex("(?<=-)[A-Z0-9]{9}(?=_)", RegexOptions.Compiled)]
        private static partial Regex GeneratedRegex();
#endif
    }
}