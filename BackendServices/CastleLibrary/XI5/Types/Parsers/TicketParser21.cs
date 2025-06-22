using XI5.Reader;

namespace XI5.Types.Parsers
{
    internal static class TicketParser21
    {
        internal static void ParseTicket(XI5Ticket ticket, TicketReader reader)
        {
            ticket.SerialId = reader.ReadTicketStringData(TicketDataType.Binary);

            ticket.IssuerId = reader.ReadTicketUInt32Data();

            ticket.IssuedDate = reader.ReadTicketTimestampData();
            ticket.ExpiryDate = reader.ReadTicketTimestampData();

            ticket.UserId = reader.ReadTicketUInt64Data();
            ticket.Username = reader.ReadTicketStringData();

            ticket.Country = reader.ReadTicketStringData(TicketDataType.Binary); // No I am not going to brazil
            ticket.Domain = reader.ReadTicketStringData();

            ticket.ServiceId = reader.ReadTicketStringData(TicketDataType.Binary);
            ticket.TitleId = XI5Ticket.ServiceIdRegex.Matches(ticket.ServiceId)[0].ToString();

            ticket.Status = reader.ReadUInt32();

            // Skip padding section in ticket
            reader.SkipTicketEmptyData(3);
        }
    }
}
