using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.DDL.Models;

namespace QuazalServer.RDVServices.GameServices.PS3DriverServices
{
    [RMCService((ushort)RMCProtocolId.NotificationEventManager)]
    public class NotificationEventManager : RMCServiceBase
    {
        [RMCMethod(1)]
        public static void Notify(NotificationEvent notification)
        {
            // Dummy event
        }
    }
}
