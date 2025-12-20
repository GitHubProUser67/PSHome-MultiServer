using CastleLibrary.Sony.XI5.PSNVerification;
using CastleLibrary.Sony.XI5.Types;
using CustomLogger;
using EndianTools;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CastleLibrary.Sony.XI5
{
    // https://www.psdevwiki.com/ps3/X-I-5-Ticket
    // https://github.com/RipleyTom/rpcn/blob/master/src/server/client/ticket.rs
    public class XI5Ticket
    {
        public const string RPCNSigner = "RPCN";

        // ticket version (2 bytes), header (4 bytes), ticket length (2 bytes) = 8 bytes
        private const byte headerLength = 8;
        // ticket version (2 bytes), header (4 bytes), ticket length (4 bytes) = 10 bytes
        private const byte headerLengthVer40 = 10;

        private static readonly ECDsaSigner ECDsaRPCN;

        static XI5Ticket()
        {
            ECDsaRPCN = new ECDsaSigner();

            using (PemReader pr = new PemReader(new StringReader("-----BEGIN PUBLIC KEY-----\r\n" +
                "ME4wEAYHKoZIzj0CAQYFK4EEACADOgAEsHvA8K3bl2V+nziQOejSucl9wqMdMELn\r\n" +
                "0Eebk9gcQrCr32xCGRox4x+TNC+PAzvVKcLFf9taCn0=\r\n" +
                "-----END PUBLIC KEY-----")))
                ECDsaRPCN.Init(false, (ECPublicKeyParameters)pr.ReadObject());
        }

        public TicketVersion TicketVersion { get; private set; }
        public ushort UnkHeader { get; set; }
        public string SerialId { get; set; }
        public uint IssuerId { get; set; }

        public DateTime IssuedDate { get; set; }
        public DateTime ExpiryDate { get; set; }

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
        public byte[] ExtraDataVer40 { get; set; }
        public uint TicketLength { get; set; }

        public string SignatureIdentifier { get; set; }
        public byte[] SignatureData { get; protected set; }
        public byte[] HashedMessage { get; set; } = Array.Empty<byte>();
        public byte[] Message { get; set; } = Array.Empty<byte>();
        public string HashName { get; set; } = string.Empty;
        public string CurveName { get; set; } = string.Empty;
        public BigInteger R { get; set; } = BigInteger.Zero;
        public BigInteger S { get; set; } = BigInteger.Zero;
        public bool Valid { get; protected set; } = false;
        public bool IsSignedByRPCN
        {
            get
            {
                return RPCNSigner.Equals(SignatureIdentifier, StringComparison.OrdinalIgnoreCase);
            }
        }

        private byte[] _fullBodyData;
        private byte[] _footer;
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

            TicketVersion version = new TicketVersion(ticketStream.ReadUShort());
            bool isVer40 = version.Major == 4 && version.Minor == 0;
            uint size = ticketStream.ReadTicketHeader(isVer40);
            long bodyStart = ticketStream.Position;
            long actualLength = ticketStream.Length - (isVer40 ? headerLengthVer40 : headerLength);

            XI5Ticket ticket = new XI5Ticket()
            {
                _fullBodyData = ReadFullBody(ticketStream),
            };

            // Ver40 tickets has a non-conventional structure which can offset the footer a bit, only read it after having parsed the full ticket.
            if (!isVer40)
                ticket._footer = ReadFooter(ticketStream);

            ticket.TicketVersion = version;

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
            if (!TryGetPayloadVersion(ticket.TicketVersion, out PayloadVersionsEnum verEnum))
                throw new FormatException($"[XI5Ticket] - Unknown/unhandled ticket version {ticket.TicketVersion}.");

            // Assert version is enabled
            if (!CheckVersion(enabled, verEnum))
                throw new UnauthorizedAccessException($"[XI5Ticket] - ticket version {ticket.TicketVersion} while being banned.");

            if (size > actualLength)
                throw new FormatException($"[XI5Ticket] - Expected ticket length to be at least {size} bytes, but was {actualLength} bytes.");
            else if (size < actualLength)
            {
                byte[] trimmedTicket = new byte[size + (isVer40 ? headerLengthVer40 : headerLength)];
                Array.Copy(ticketData, 0, trimmedTicket, 0, trimmedTicket.Length);
                return ReadFromBytes(trimmedTicket);
            }

            ticketStream.Seek(bodyStart + 4, SeekOrigin.Begin); // skip existing dt and size

            switch (verEnum)
            {
                case PayloadVersionsEnum.V20:
                    ticket.ParseTicketV2_0(ticketStream);
                    break;
                case PayloadVersionsEnum.V21:
                    ticket.ParseTicketV2_1(ticketStream);
                    break;
                case PayloadVersionsEnum.V30:
                    ticket.ParseTicketV3_0(ticketStream);
                    break;
                case PayloadVersionsEnum.V40:
                    ticket.ParseTicketV4_0(ticketStream);
                    ticket._footer = ReadFooter(ticketStream);
                    break;
            }

            using (MemoryStream footerStream = new MemoryStream(ticket._footer))
            {
                ticket.SignatureIdentifier = ReadBinaryAsString(footerStream);
                ticket.SignatureData = ReadBinary(footerStream);
            }

            rpcn = ticket.IsSignedByRPCN;

            DerSequence seq;
            DateTime currentTime = DateTime.UtcNow;
            bool isValidTimestamp = ticket.IssuedDate <= currentTime && ticket.ExpiryDate >= currentTime;
            bool isValidServiceId = string.IsNullOrEmpty(serviceIdCriteria) || ticket.ServiceId.Contains(serviceIdCriteria);

            if (rpcn)
            {
                seq = new Asn1InputStream(ParseSignature(ticket.SignatureData)).ReadObject() as DerSequence;

                if (seq != null)
                {
#if NET6_0_OR_GREATER
                    ticket.Message = ticketData.AsSpan().Slice((int)bodyStart, ticket._fullBodyData.Length).ToArray();
#else
                        ticket.Message = new byte[ticket._fullBodyData.Length];
                        Array.Copy(ticketData, (int)bodyStart, ticket.Message, 0, ticket._fullBodyData.Length);
#endif
                    ticket.HashedMessage = NetHasher.DotNetHasher.ComputeSHA224(ticket.Message);
                    ticket.HashName = "SHA224";
                    ticket.CurveName = "secp224k1";

                    ticket.Valid = isValidTimestamp && isValidServiceId && ECDsaRPCN.VerifySignature(ticket.HashedMessage, ((DerInteger)seq[0]).Value, ((DerInteger)seq[1]).Value);
                }
            }
            else if (ticket.SignatureData.Length == 56)
            {
                List<ITicketPublicSigningKey> psnSigningKeys = null;

                seq = new Asn1InputStream(ParseSignature(ticket.SignatureData)).ReadObject() as DerSequence;

                if (seq != null)
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

                    psnSigningKeys = SigningKeyResolver.GetSigningKeys(ticket.SignatureIdentifier, ticket.TitleId);

                    // verify ticket signature or skip them depending the compiler options and/or the current ticket version
                    ticket.Valid = isValidTimestamp && isValidServiceId && (psnSigningKeys == null || psnSigningKeys.Any(key =>
                           SigningKeyResolver.VerifyTicketSignature(ticket.HashedMessage, key.PemStr, seq)));
                }
            }
            else if (isVer40 && ticket.SignatureData.Length == 32) // TODO, figuring out the 4.0 hash algorithm.
            {
                // unhandled!!!

                ticket.Valid = isValidTimestamp && isValidServiceId;
            }
            else
                throw new FormatException($"[XI5Ticket] - Unknown Signature data.");

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
                    LoggerAccessor.LogWarn($"[XI5Ticket] - Ticket for {ticket.TitleId} has invalid signature (nsig: {{{BytesToHex(ticket.SignatureData)}}} - orig sig: {{{BytesToHex(sigBackup)}}})");
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

        private void ParseTicketV2_0(Stream bodyStream)
        {
            SerialId = ReadBinaryAsString(bodyStream);
            IssuerId = ReadUInt(bodyStream);
            IssuedDate = ReadTime(bodyStream);
            ExpiryDate = ReadTime(bodyStream);
            UserId = ReadULong(bodyStream);
            Username = ReadString(bodyStream);
            Country = ReadBinaryAsString(bodyStream);
            Domain = ReadString(bodyStream);
            ServiceId = ReadBinaryAsString(bodyStream);
            TitleId = ServiceIdRegex.Matches(ServiceId)[0].ToString();

            StatusHeader = bodyStream.ReadUInt();

            Age = bodyStream.ReadUShort();
            Status = bodyStream.ReadUShort();

            //TODO ???
            //unknown
            ReadEmptyData(bodyStream, 2);
        }

        private void ParseTicketV2_1(Stream bodyStream)
        {
            SerialId = ReadBinaryAsString(bodyStream);
            IssuerId = ReadUInt(bodyStream);
            IssuedDate = ReadTime(bodyStream);
            ExpiryDate = ReadTime(bodyStream);
            UserId = ReadULong(bodyStream);
            Username = ReadString(bodyStream);
            Country = ReadBinaryAsString(bodyStream);
            Domain = ReadString(bodyStream);
            ServiceId = ReadBinaryAsString(bodyStream);
            TitleId = ServiceIdRegex.Matches(ServiceId)[0].ToString();

            StatusHeader = bodyStream.ReadUInt();

            Age = bodyStream.ReadUShort();
            Status = bodyStream.ReadUShort();

            StatusDuration = bodyStream.ReadUInt();
            Dob = bodyStream.ReadUInt();
        }

        private void ParseTicketV3_0(Stream bodyStream)
        {
            SerialId = ReadBinaryAsString(bodyStream);
            IssuerId = ReadUInt(bodyStream);
            IssuedDate = ReadTime(bodyStream);
            ExpiryDate = ReadTime(bodyStream);
            UserId = ReadULong(bodyStream);
            Username = ReadString(bodyStream);
            Country = ReadBinaryAsString(bodyStream);
            Domain = ReadString(bodyStream);
            ServiceId = ReadBinaryAsString(bodyStream);
            TitleId = ServiceIdRegex.Matches(ServiceId)[0].ToString();

            var header = ReadTicketSectionHeader(bodyStream);
            if (header.Item1 != Sectiontype.DateOfBirth)
                throw new FormatException($"[XI5Ticket] - Expected section to be {nameof(Sectiontype.DateOfBirth)}, " +
                        $"was really {header.Item1} ({(int)header.Item1})");

            Dob = bodyStream.ReadUInt();

            StatusHeader = bodyStream.ReadUInt();

            Age = bodyStream.ReadUShort();
            Status = bodyStream.ReadUShort();

            header = ReadTicketSectionHeader(bodyStream);
            if (header.Item1 != Sectiontype.Age)
                throw new FormatException($"[XI5Ticket] - Expected section to be {nameof(Sectiontype.Age)}, " +
                                              $"was really {header.Item1} ({(int)header.Item1})");

            ReadEmptyData(bodyStream);
        }

        private void ParseTicketV4_0(Stream bodyStream)
        {
            UnkHeader = bodyStream.ReadUShort();

            SerialId = ReadBinaryAsString(bodyStream);
            IssuerId = ReadUInt(bodyStream);
            IssuedDate = ReadTime(bodyStream);
            ExpiryDate = ReadTime(bodyStream);
            UserId = ReadULong(bodyStream);
            Username = ReadString(bodyStream);
            Country = ReadBinaryAsString(bodyStream);
            Domain = ReadString(bodyStream);
            ServiceId = ReadBinaryAsString(bodyStream);
            TitleId = ServiceIdRegex.Matches(ServiceId)[0].ToString();

            var header = ReadTicketSectionHeader(bodyStream);
            if (header.Item1 != Sectiontype.DateOfBirth)
                throw new FormatException($"[XI5Ticket] - Expected section to be {nameof(Sectiontype.DateOfBirth)}, " +
                        $"was really {header.Item1} ({(int)header.Item1})");

            Dob = bodyStream.ReadUInt();

            StatusHeader = bodyStream.ReadUInt();

            Age = bodyStream.ReadUShort();
            Status = bodyStream.ReadUShort();

            var headerProps = PeekReadTicketSectionHeaderProperties(bodyStream);
            if (headerProps.Item1 != Sectiontype.Age)
                throw new FormatException($"[XI5Ticket] - Expected section to be {nameof(Sectiontype.Age)}, " +
                                              $"was really {headerProps.Item1} ({(int)headerProps.Item1})");

            ExtraDataVer40 = new byte[bodyStream.Length - bodyStream.Position - 0x30];

            bodyStream.ReadAll(ExtraDataVer40, 0, ExtraDataVer40.Length);
        }

        private static byte[] ReadFullField(Stream stream, Datatype expected)
        {
            Datatype dt = (Datatype)stream.ReadUShort();
            if (dt != expected && dt != Datatype.Empty)
                throw new InvalidDataException($"[XI5Ticket] - Expected datatype: {expected} | Actual datatype: {dt}");

            ushort size = stream.ReadUShort();
            byte[] data = new byte[size+4]; //with datatype and size included
            stream.Seek(-4, SeekOrigin.Current);
            if (!stream.ReadAll(data, 0, data.Length))
                throw new EndOfStreamException($"[XI5Ticket] - Failed to read {size} bytes from stream");
            return data;
        }

        private static byte[] ReadField(Stream stream, Datatype expected)
        {
            Datatype dt = (Datatype)stream.ReadUShort();
            if (dt != expected && dt != Datatype.Empty)
                throw new InvalidDataException($"[XI5Ticket] - Expected datatype: {expected} | Actual datatype: {dt}");

            ushort size = stream.ReadUShort();
            byte[] data = new byte[size];
            if (!stream.ReadAll(data, 0, size))
                throw new EndOfStreamException($"[XI5Ticket] - Failed to read {size} bytes from stream");
            return data;
        }

        private static void ReadEmptyData(Stream stream, int sections = 1)
        {
            for (int i = 0; i < sections; i++)
                ReadField(stream, Datatype.Empty);
        }

        private static byte[] ReadBody(Stream stream) => ReadField(stream, Datatype.Body);
        private static byte[] ReadFullBody(Stream stream) => ReadFullField(stream, Datatype.Body);
        private static byte[] ReadFooter(Stream stream) => ReadField(stream, Datatype.Footer);
        private static byte[] ReadFullFooter(Stream stream) => ReadFullField(stream, Datatype.Footer);
        private static byte[] ReadBinary(Stream stream) => ReadField(stream, Datatype.Binary);
        private static byte[] ReadFullBinary(Stream stream) => ReadFullField(stream, Datatype.Binary);

        private static string ReadBinaryAsString(Stream stream)
        {
            byte[] data = ReadBinary(stream);
            int inx = Array.FindIndex(data, 0, (x) => x == 0); //search for 0
            if (inx >= 0)
                return Encoding.UTF8.GetString(data, 0, inx);
            return Encoding.UTF8.GetString(data);
        }

        private static ushort ReadUShort(Stream stream)
        {
            return EndianAwareConverter.ToUInt16(ReadField(stream, Datatype.Binary), Endianness.BigEndian, 0);
        }

        private static uint ReadUInt(Stream stream)
        {
            return EndianAwareConverter.ToUInt32(ReadField(stream, Datatype.UInt), Endianness.BigEndian, 0);
        }

        private static DateTime ReadTime(Stream stream)
        {
            byte[] data = ReadField(stream, Datatype.Time);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return DateTimeOffset.FromUnixTimeMilliseconds((long)BitConverter.ToUInt64(data, 0)).UtcDateTime;
        }

        private static ulong ReadULong(Stream stream)
        {
            return EndianAwareConverter.ToUInt64(ReadField(stream, Datatype.ULong), Endianness.BigEndian, 0);
        }

        private static string ReadString(Stream stream)
        {
            byte[] data = ReadField(stream, Datatype.String);
            int inx = Array.FindIndex(data, 0, (x) => x == 0); //search for 0
            if (inx >= 0)
                return Encoding.UTF8.GetString(data, 0, inx);
            return Encoding.UTF8.GetString(data);
        }

        private static (Sectiontype, ushort, long) ReadTicketSectionHeader(Stream stream)
        {
            long position = stream.Position;

            byte sectionHeader = (byte)stream.ReadByte();
            if (sectionHeader != 0x30)
                throw new FormatException($"[XI5Ticket] - Expected 0x30 for section header, was {sectionHeader}. Offset is {position + 1}");

            Sectiontype type = (Sectiontype)stream.ReadByte();
            ushort length = stream.ReadUShort();

            return (type, length, position);
        }

        private static (Sectiontype, long) PeekReadTicketSectionHeaderProperties(Stream stream)
        {
            long position = stream.Position;

            byte sectionHeader = (byte)stream.ReadByte();
            if (sectionHeader != 0x30)
                throw new FormatException($"[XI5Ticket] - Expected 0x30 for section header, was {sectionHeader}. Offset is {position + 1}");

            Sectiontype type = (Sectiontype)stream.ReadByte();

            return (type, position);
        }

        private static bool CheckVersion(PayloadVersionsEnum enabled, PayloadVersionsEnum version)
        {
            return (enabled & version) == version;
        }

        /// <summary>
        /// Attempts to repair DER-encoded ECDSA signatures that may contain
        /// up to two trailing non-DER bytes (observed in PlayStation signatures).
        /// </summary>
        public static byte[] ParseSignature(byte[] sig)
        {
            int sigSize = sig.Length;

            byte[] current = new byte[sigSize];
            Array.Copy(sig, 0, current, 0, sigSize);

            // Up to two attempts because PSN may append 1–2 stray bytes
            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    // If parsing succeeds, return this version
                    Asn1Object.FromByteArray(current);
                    return current;
                }
                catch
                {
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    current = current.SkipLast(1).ToArray();
#else
                    if (current.Length > 0)
                        current = current.Take(current.Length - 1).ToArray();
#endif
                }
            }
            // Parsing never succeeded
            throw new InvalidOperationException("[XI5Ticket] - Invalid or unrecoverable DER signature.");
        }

        /// <summary>
        /// Attempts to repair DER-encoded ECDSA signatures that may contain
        /// up to two trailing non-DER bytes (observed in PlayStation signatures).
        /// </summary>
        public static Asn1Sequence ParseSignature(XI5Ticket ticket)
        {
            int sigSize = ticket.SignatureData.Length;

            byte[] current = new byte[sigSize];
            Array.Copy(ticket.SignatureData, 0, current, 0, sigSize);

            // Up to two attempts because PSN may append 1–2 stray bytes
            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    // If parsing succeeds, return this version
                    Asn1Object.FromByteArray(current);
                    return (Asn1Sequence)Asn1Object.FromByteArray(current);
                }
                catch
                {
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    current = current.SkipLast(1).ToArray();
#else
                    if (current.Length > 0)
                        current = current.Take(current.Length - 1).ToArray();
#endif
                }
            }
            // Parsing never succeeded
            throw new InvalidOperationException("[XI5Ticket] - Invalid or unrecoverable DER signature.");
        }

        public static string BytesToHex(byte[] bytes)
        {
#if NET6_0_OR_GREATER
            return Convert.ToHexString(bytes);
#else
            return string.Concat(bytes.Select(b => b.ToString("X2")));
#endif
        }

        public static byte[] HexToBytes(string hex)
        {
#if NET5_0_OR_GREATER
            return Convert.FromHexString(hex);
#else
            if (hex.Length % 2 == 1)
                throw new ArgumentException("[XI5Ticket] - The binary key cannot have an odd number of digits");

            return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
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
#if NET7_0_OR_GREATER

        [GeneratedRegex("(?<=-)[A-Z0-9]{9}(?=_)", RegexOptions.Compiled)]
        private static partial Regex GeneratedRegex();
#endif
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"TicketVersion: {TicketVersion}");
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
            sb.AppendLine($"ExtraDataVer40: {(ExtraDataVer40 != null ? BitConverter.ToString(ExtraDataVer40).Replace("-", string.Empty) : "null")}");
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

        private enum Datatype : ushort
        {
            Empty = 0,
            UInt = 1,
            ULong = 2,
            String = 4,
            Time = 7,
            Binary = 8,
            Body = 0x3000,
            Footer = 0x3002
        }

        private enum Sectiontype : byte
        {
            Body = 0,
            Footer = 2,
            Age = 16,
            DateOfBirth = 17,
        }
    }
}