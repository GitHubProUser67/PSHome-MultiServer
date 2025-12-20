using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVO.Games.PS3
{
    public class StarhawkStatBlobs
    {
        //Type1
        public struct PlayerDetails
        {
            public int Rating { get; set; }             // 0
            public int Exp { get; set; }                // 1
            public int Cash { get; set; }               // 2
            public int NumKills { get; set; }           // 3
            public int NumDeaths { get; set; }          // 4
            public int Suicides { get; set; }           // 5
            public int TeamKills { get; set; }          // 6
            public int Headshots { get; set; }          // 7
            public int Assists { get; set; }            // 8
            public int Score { get; set; }              // 9
            public int Wins { get; set; }               // 0xA (10)
            public int Losses { get; set; }             // 0xB (11)
            public int KillPoints { get; set; }         // 0xC (12)
            public int SupportPoints { get; set; }      // 0xD (13)
            public int FlagCaptures { get; set; }       // 0xE (14)
            public int RiftCaptures { get; set; }       // 0xF (15)
            public int BuildingsDestroyed { get; set; } // 0x10 (16)
            public int TimePlayed { get; set; }         // 0x11 (17)
            public int TimeInCampaign { get; set; }     // 0x12 (18)
            public int TimeInMultiplayer { get; set; }  // 0x13 (19)
            public int TimeInCTF { get; set; }          // 0x14 (20)
            public int TimeInTDM { get; set; }          // 0x15 (21)
            public int TimeInDOM { get; set; }          // 0x16 (22)
            public int TimeInPROS { get; set; }         // 0x17 (23)
            public int SecondsAlive { get; set; }       // 0x18 (24)
            public int ComplaintCheating { get; set; }  // 0x19 (25)
            public int ComplaintRude { get; set; }      // 0x1A (26)
            public int ComplaintUnsportsmanlike { get; set; } // 0x1B (27)
            public int PingSpikes { get; set; }         // 0x1C (28)
            public int PingKill { get; set; }           // 0x1D (29)
            public int PingMount { get; set; }          // 0x1E (30)
            public int Draw { get; set; }               // 0x1F (31)
            public int CTFWin { get; set; }             // 0x20 (32)
            public int CTFLoss { get; set; }            // 0x21 (33)
            public int CTFDraw { get; set; }            // 0x22 (34)
            public int ProsWin { get; set; }            // 0x23 (35)
            public int ProsLoss { get; set; }           // 0x24 (36)
            public int TDMWin { get; set; }             // 0x25 (37)
            public int TDMLoss { get; set; }            // 0x26 (38)
            public int DOMWin { get; set; }             // 0x27 (39)
            public int DOMLoss { get; set; }            // 0x28 (40)
            public int NumIntStats_0 { get; set; }      // 0x40 (64)
        }

        //Type2
        public struct PlayerSummary
        {
            public int Rating { get; set; }
            public int Exp { get; set; }
            public int Kills { get; set; }
            public int Deaths { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public int CTFWins { get; set; }
            public int TDMWins { get; set; }
            public int DOMWins { get; set; }
            public int DMWins { get; set; }
            public int PRSWins { get; set; }
            public int BnBPartsPlaced { get; set; }
            public int WheelmanAwards { get; set; }
            public int DogfighterAwards { get; set; }
            public int AntiAirAwards { get; set; }
            public int RepairmanAwards { get; set; }
            public int MechanicalArmyAwards { get; set; }
            public int UpCloseAwards { get; set; }
            public int JeepDriverKillAssists { get; set; }
            public int StarhawksDestroyedDogFighting { get; set; }
            public int StarhawksDestroyedFromGround { get; set; }
            public int VehicleRepairs { get; set; }
            public int AutoTurretKills { get; set; }
            public int MeleeKills { get; set; }
            public int NumIntStats { get; set; }
        }

        //Type3
        public struct TournamentAwardsSummary
        {
            public int BlobSize { get; set; }
            public int NumberOfTournamentsCompleted { get; set; }
            public int TotalTournamentPoints { get; set; }
            public int TotalAwardsReceived { get; set; }
            public int TotalUniqueAwardsReceived { get; set; }
            public int Reserved6 { get; set; }
            public int Reserved7 { get; set; }
            public int Reserved8 { get; set; }
            public int Reserved9 { get; set; }
            public int NumIntStats { get; set; }
        }

        //Type4 ?

        //Type5
        public struct BnBPartsSummary
        {
            public int TotalBnBPartPoints { get; set; }
            public int TotalBnBPartKills { get; set; }
            public int TotalBnBPartDeaths { get; set; }
            public int NumIntStats { get; set; }
        }

        //Type6
        public struct AwardsSummary
        {
            public int TotalAwardsEarnedPoints { get; set; }
            public int NumIntStats { get; set; }
        }
    }
}
