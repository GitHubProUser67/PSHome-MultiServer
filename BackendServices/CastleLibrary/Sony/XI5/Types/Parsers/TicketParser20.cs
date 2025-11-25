// From: https://github.com/hallofmeat/Skateboard3Server/blob/master/src/Skateboard3Server.Blaze/Tickets/Ps3TicketParser.cs
using CastleLibrary.Sony.XI5.Reader;

namespace CastleLibrary.Sony.XI5.Types.Parsers
{
    internal static class TicketParser20
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

            ticket.StatusHeader = reader.ReadUInt32();

            ticket.Age = reader.ReadUInt16();
            ticket.Status = reader.ReadUInt16();

            //TODO ???
            //unknown
            reader.SkipTicketEmptyData(2);
        }
    }
}
