using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.PCDriverServices
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
