using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.PS3DriverServices
{
    /// <summary>
    /// Ubi news service
    /// </summary>
    [RMCService((ushort)RMCProtocolId.UbiNewsService)]
    public class UbiNewsService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult GetChannel()
        {
            if (Context != null)
                return Result(new NewsChannel
                {
                    m_ID = 1,
                    m_ownerPID = Context.Client.sPID,
                    m_locale = "en-US",
                    m_name = "Name",
                    m_description = "Description",
                    m_subscribable = false,
                    m_type = "UbiNews",
                    m_creationTime = DateTime.MinValue,
                    m_expirationTime = DateTime.MinValue,
                });
            else
                return Error(0);
        }
    }
}
