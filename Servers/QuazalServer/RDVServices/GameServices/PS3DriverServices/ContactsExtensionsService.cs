using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.PS3DriverServices
{
    [RMCService((ushort)RMCProtocolId.ContactsExtensionsService)]
    public class ContactsExtensionsService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult SetContacts()
        {

            UNIMPLEMENTED();
            return Error(0);
        }

    }
}
