using CastleLibrary.Sony.XI5.Reader;
using CastleLibrary.Sony.XI5.Types;
using CastleLibrary.Sony.XI5.Types.Parsers;
using CastleLibrary.Sony.XI5.Verification;
using CustomLogger;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CastleLibrary.Sony.XI5
{
    // https://www.psdevwiki.com/ps3/X-I-5-Ticket
    // https://github.com/RipleyTom/rpcn/blob/master/src/server/client/ticket.rs
    // https://github.com/LittleBigRefresh/NPTicket/tree/main

    public partial class XI5Ticket
    {
        public const string RPCNSigner = "RPCN";

        // constructor
        public XI5Ticket() { }

        // fields
        public TicketVersion Version { get; set; }
        public ushort UnkHeader { get; set; }
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

        public uint StatusHeader { get; set; }
        public ushort Age { get; set; }
        public ushort Status { get; set; }
        public uint StatusDuration { get; set; }
        public uint Dob { get; set; }
        public uint Unk { get; set; }
        public byte[] Unk0 { get; set; }
        public byte[] Unk1 { get; set; }
        public uint TicketLength { get; set; }
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
        public bool IsSignedByRPCN 
        {
            get
            {
                return RPCNSigner.Equals(SignatureIdentifier, StringComparison.OrdinalIgnoreCase);
            }
        }

#if NET7_0_OR_GREATER
        internal static readonly Regex ServiceIdRegex = GeneratedRegex();
#else
        internal static readonly Regex ServiceIdRegex = new Regex("(?<=-)[A-Z0-9]{9}(?=_)", RegexOptions.Compiled);
#endif
        public static XI5Ticket ReadFromBytes(byte[] ticketData, PayloadVersionsEnum enabled = PayloadVersionsEnum.V20 | PayloadVersionsEnum.V21 | PayloadVersionsEnum.V30, string serviceIdCriteria = null)
        {
            using (MemoryStream ms = new MemoryStream(ticketData))
                return ReadFromStream(ms, enabled, serviceIdCriteria);
        }

        public static XI5Ticket ReadFromStream(Stream ticketStream, PayloadVersionsEnum enabled = PayloadVersionsEnum.V20 | PayloadVersionsEnum.V21 | PayloadVersionsEnum.V30, string serviceIdCriteria = null)
        {
            // ticket version (2 bytes), header (4 bytes), ticket length (2 bytes) = 8 bytes
            const byte headerLength = 8;
            // ticket version (2 bytes), header (4 bytes), ticket length (4 bytes) = 10 bytes
            const byte headerLengthVer40 = 10;

            bool rpcn = false;
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

                // Determine payload version enum from the ticket version
                static bool TryGetPayloadVersion(TicketVersion ver, out PayloadVersionsEnum payloadVersion)
                {
                    payloadVersion = ver switch
                    {
                        { Major: 2, Minor: 0 } => PayloadVersionsEnum.V20,
                        { Major: 2, Minor: 1 } => PayloadVersionsEnum.V21,
                        { Major: 3, Minor: 0 } => PayloadVersionsEnum.V30,
                        { Major: 4, Minor: 0 } => PayloadVersionsEnum.V40,
                        _ => default
                    };

                    return payloadVersion != default;
                }

                // Try resolve version
                if (!TryGetPayloadVersion(ticket.Version, out PayloadVersionsEnum verEnum))
                    throw new FormatException($"[XI5Ticket] - Unknown/unhandled ticket version {ticket.Version}.");

                // Assert version is enabled
                if (!CheckVersion(enabled, verEnum))
                    throw new UnauthorizedAccessException($"[XI5Ticket] - ticket version {ticket.Version} while being banned.");

                bool isVer40 = verEnum == PayloadVersionsEnum.V40;

                uint ticketLength = ticket.TicketLength = reader.ReadTicketHeader(isVer40);

                long bodyStart = reader.BaseStream.Position;

                long actualLength = ticketStream.Length - (isVer40 ? headerLengthVer40 : headerLength);
                if (ticketLength > actualLength)
                    throw new FormatException($"[XI5Ticket] - Expected ticket length to be at least {ticketLength} bytes, but was {actualLength} bytes.");
                else if (ticketLength < actualLength)
                {
                    byte[] trimmedTicket = new byte[ticketLength + (isVer40 ? headerLengthVer40 : headerLength)];
                    Array.Copy(ticketData, 0, trimmedTicket, 0, trimmedTicket.Length);
                    return ReadFromBytes(trimmedTicket);
                }

                ticket.BodySection = reader.ReadTicketSectionHeader();
                if (ticket.BodySection.Type != TicketDataSectionType.Body)
                    throw new FormatException($"[XI5Ticket] - Expected first section to be {nameof(TicketDataSectionType.Body)}, but was {ticket.BodySection.Type} ({(int)ticket.BodySection.Type}).");

                switch (verEnum)
                {
                    case PayloadVersionsEnum.V20:
                        TicketParser20.ParseTicket(ticket, reader);
                        break;
                    case PayloadVersionsEnum.V21:
                        TicketParser21.ParseTicket(ticket, reader);
                        break;
                    case PayloadVersionsEnum.V30:
                        TicketParser30.ParseTicket(ticket, reader);
                        break;
                    case PayloadVersionsEnum.V40:
                        TicketParser40.ParseTicket(ticket, reader);
                        break;
                }

                var footer = reader.ReadTicketSectionHeader();
                if (footer.Type != TicketDataSectionType.Footer)
                {
                    LoggerAccessor.LogError($"[XI5Ticket] - Expected last section to be {nameof(TicketDataSectionType.Footer)}, but was {footer.Type} ({(int)footer.Type}).");
                    return null;
                }

                ticket.SignatureIdentifier = reader.ReadTicketStringData(TicketDataType.Binary);
                ticket.SignatureData = reader.ReadTicketBinaryData();

                rpcn = ticket.IsSignedByRPCN;

                if (rpcn)
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
                else if (ticket.SignatureData.Length == 56)
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
                else if (isVer40) // TODO, figuring out the 4.0 hash algorithm.
                {
                    // unhandled!!!
                }
                else
                    throw new FormatException($"[XI5Ticket] - Unknown Signature data.");
            }

            DateTimeOffset currentTime = DateTimeOffset.UtcNow;
            bool isValidTimestamp = ticket.IssuedDate <= currentTime && ticket.ExpiryDate >= currentTime;
            bool isValidServiceId = string.IsNullOrEmpty(serviceIdCriteria) || ticket.ServiceId.Contains(serviceIdCriteria);

            List<ITicketSigningKey> signingKeys = null;

            // only verify if we need to
            if (!string.IsNullOrEmpty(ticket.HashName))
                signingKeys = SigningKeyResolver.GetSigningKeys(rpcn, ticket.SignatureIdentifier, ticket.TitleId);

            // verify ticket signature or skip them depending the compiler options and/or the current ticket version
            if (signingKeys == null)
                ticket.Valid = isValidTimestamp && isValidServiceId;
            else
                ticket.Valid = signingKeys.Any(key =>
                   new TicketVerifier(ticketData, ticket, key).IsTicketValid()) && isValidTimestamp && isValidServiceId;

            if (!isValidTimestamp)
            {
                LoggerAccessor.LogError($"[XI5Ticket] - Timestamp of the ticket data was invalid, likely an exploit. (IssuedDate:{ticket.IssuedDate} ExpiryDate:{ticket.ExpiryDate} CurrentTime:{currentTime})");
                return ticket;
            }
            else if (!isValidServiceId)
            {
                LoggerAccessor.LogError($"[XI5Ticket] - ServiceId of the ticket data was invalid, likely an exploit. (ServiceId:{ticket.ServiceId} ExpectedCriteria:{serviceIdCriteria})");
                return ticket;
            }
#if DEBUG
            if (!ticket.Valid)
            {
                LoggerAccessor.LogWarn($"[XI5Ticket] - Invalid ticket data sent at:{DateTime.Now} with TitleId:{ticket.TitleId} with payload:{{{BytesToHex(ticketData)}}}");

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

        private static bool CheckVersion(PayloadVersionsEnum enabled, PayloadVersionsEnum version)
        {
            return (enabled & version) == version;
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
            sb.AppendLine($"UnkHeader: {UnkHeader}");
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
            sb.AppendLine($"StatusHeader: {StatusHeader}");
            sb.AppendLine($"Age: {Age}");
            sb.AppendLine($"Status: {Status}");
            sb.AppendLine($"StatusDuration: {StatusDuration}");
            sb.AppendLine($"Dob: {Dob}");
            sb.AppendLine($"Unk: {Unk}");
            sb.AppendLine($"Unk0: {(Unk0 != null ? BitConverter.ToString(Unk0).Replace("-", string.Empty) : "null")}");
            sb.AppendLine($"Unk1: {(Unk1 != null ? BitConverter.ToString(Unk1).Replace("-", string.Empty) : "null")}");
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