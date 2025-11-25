// From: https://github.com/hallofmeat/Skateboard3Server/blob/master/src/Skateboard3Server.Blaze/Tickets/Ps3TicketParser.cs
using CastleLibrary.Sony.XI5.Reader;
using System;

namespace CastleLibrary.Sony.XI5.Types.Parsers
{
    internal static class TicketParser40
    {
        internal static void ParseTicket(XI5Ticket ticket, TicketReader reader)
        {
            ticket.UnkHeader = reader.ReadUInt16();

            ticket.SerialId = reader.ReadTicketStringData(TicketDataType.Binary);

            ticket.IssuerId = reader.ReadTicketUInt32Data();

            ticket.IssuedDate = reader.ReadTicketTimestampData();
            ticket.ExpiryDate = reader.ReadTicketTimestampData();

            ticket.UserId = reader.ReadTicketUInt64Data();
            ticket.Username = reader.ReadTicketStringData();

            ticket.Country = reader.ReadTicketStringData(TicketDataType.Binary);
            ticket.Domain = reader.ReadTicketStringData();

            ticket.ServiceId = reader.ReadTicketStringData(TicketDataType.Binary);
            ticket.TitleId = XI5Ticket.ServiceIdRegex.Matches(ticket.ServiceId)[0].ToString();

            TicketDataSection header = reader.ReadTicketSectionHeader();
            if (header.Type != TicketDataSectionType.DateOfBirth)
            {
                throw new FormatException($"[XI5Ticket] - Expected section to be {nameof(TicketDataSectionType.DateOfBirth)}, " +
                    $"was really {header.Type} ({(int)header.Type})");
            }

            ticket.Dob = reader.ReadUInt32();

            ticket.StatusHeader = reader.ReadUInt32();

            ticket.Age = reader.ReadUInt16();
            ticket.Status = reader.ReadUInt16();

            header = reader.ReadTicketSectionHeader();
            if (header.Type != TicketDataSectionType.Age)
            {
                throw new FormatException($"[XI5Ticket] - Expected section to be {nameof(TicketDataSectionType.Age)}, " +
                                          $"was really {header.Type} ({(int)header.Type})");
            }

            reader.SkipTicketEmptyData();

            //unknown
            ticket.Unk = reader.ReadUInt32();

            //unknown String (8)
            ticket.Unk0 = reader.ReadTicketBinaryData();

            //unknown String (64)
            ticket.Unk1 = reader.ReadTicketBinaryData();
        }
    }
}
