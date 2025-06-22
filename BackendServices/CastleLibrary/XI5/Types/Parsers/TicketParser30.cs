using System;
using XI5.Reader;

namespace XI5.Types.Parsers
{
    internal static class TicketParser30
    {
        internal static void ParseTicket(XI5Ticket ticket, TicketReader reader)
        {
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

            reader.ReadUInt32();                // d.o.b
            reader.SkipTicketEmptyData(2);

            header = reader.ReadTicketSectionHeader();
            if (header.Type != TicketDataSectionType.Age)
            {
                throw new FormatException($"[XI5Ticket] - Expected section to be {nameof(TicketDataSectionType.Age)}, " +
                                          $"was really {header.Type} ({(int)header.Type})");
            }

            reader.SkipTicketEmptyData();
        }
    }
}
