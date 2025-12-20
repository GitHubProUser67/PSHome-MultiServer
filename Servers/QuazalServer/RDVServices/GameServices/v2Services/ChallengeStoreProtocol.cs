using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.v2Services
{
    /// <summary>
	/// Challenge store service
	/// </summary>
	[RMCService((ushort)RMCProtocolId.ChallengeStoreProtocol)]
    public class ChallengeStoreProtocol : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult UNK1(uint unk1, string unk2, string unk3, string unk4, uint unk5, QNetZ.DDL.Buffer unk6)
        {
            UNIMPLEMENTED();

            return Error(0);
        }

        [RMCMethod(2)]
        public RMCResult UNK2(uint unk1, ulong unk2)
        {
            UNIMPLEMENTED();

            return Result(new { retVal = true, buffer = new QNetZ.DDL.Buffer { } });
        }

        [RMCMethod(3)]
        public RMCResult UNK3(uint unk1)
        {
            UNIMPLEMENTED();

            return Result(new { chalFileStats = new List<ChallengeFileStats> { } });
        }

        [RMCMethod(4)]
        public RMCResult UNK4(uint unk1, uint unk2, bool unk3, string unk4)
        {
            UNIMPLEMENTED();

            return Result(new { chalFileStats = new List<ChallengeFileStats> { } });
        }
    }
}
