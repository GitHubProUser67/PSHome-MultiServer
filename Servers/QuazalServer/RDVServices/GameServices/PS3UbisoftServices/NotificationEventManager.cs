using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.PS3UbisoftServices
{
    [RMCService((ushort)RMCProtocolId.NotificationEventManager)]
    public class NotificationEventManager : RMCServiceBase
    {
        [RMCMethod(1)]
        public void Notify(NotificationEvent notification)
        {
            // Dummy event
        }
    }
}
