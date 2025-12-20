using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.DDL.Models.EventStatsProtocol;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.v2Services
{
    /// <summary>
	/// Events stats service
	/// </summary>
	[RMCService((ushort)RMCProtocolId.EventStatsProtocol)]
    public class EventStatsProtocol : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult SendGameStats(GameStats gameStats)
        {
            UNIMPLEMENTED();

            return Result(new { retVal = true });
        }

        [RMCMethod(2)]
        public RMCResult UNK2(uint unk1, uint unk2, uint unk3)
        {
            UNIMPLEMENTED();

            return Result(new { gameStats = new List<GameStats> { }, unk = (uint)0 });
        }

        [RMCMethod(3)]
        public RMCResult UNK3()
        {
            UNIMPLEMENTED();

            return Result(new { unk = (uint)0, unk1 = (uint)0 });
        }

        [RMCMethod(4)]
        public RMCResult UNK4(string unk)
        {
            UNIMPLEMENTED();

            return Result(new { gameStats = new List<GameStats> { }, unk = (uint)0, unk2 = new List<uint> { } });
        }

        [RMCMethod(5)]
        public RMCResult UNK5()
        {
            UNIMPLEMENTED();

            return Error(0);
        }

        [RMCMethod(6)]
        public RMCResult SendEventStats(EventStats eventStats)
        {
            UNIMPLEMENTED();

            return Result(new { retVal = true });
        }

        [RMCMethod(7)]
        public RMCResult UNK7(ulong unk1, uint unk2, uint unk3, uint unk4)
        {
            UNIMPLEMENTED();

            return Result(new { eventStats = new List<EventStats> { }, unk = (uint)0 });
        }

        [RMCMethod(8)]
        public RMCResult UNK8()
        {
            UNIMPLEMENTED();

            return Result(new { unk = (uint)0, unk1 = (uint)0 });
        }

        [RMCMethod(9)]
        public RMCResult UNK9()
        {
            UNIMPLEMENTED();

            return Result(new { eventStats = new List<EventStats> { }, unk = (uint)0 });
        }

        [RMCMethod(10)]
        public RMCResult UNK10(List<ulong> unk1, string unk2)
        {
            UNIMPLEMENTED();

            return Result(new { eventStats = new List<EventStats> { }, unk = (uint)0 });
        }

        [RMCMethod(11)]
        public RMCResult UNK11()
        {
            UNIMPLEMENTED();

            return Error(0);
        }
    }
}
