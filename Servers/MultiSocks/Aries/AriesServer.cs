using CustomLogger;
using MultiSocks.Aries.DataStore;

namespace MultiSocks.Aries
{
    public class AriesServer : IDisposable
    {
        public static IDatabase? Database = null;
        private readonly AbstractAriesServer? RedirectorARMYOFTWO2010_PS3;
        private readonly AbstractAriesServer? RedirectorSSX3_NTSC_A;
        private readonly AbstractAriesServer? RedirectorSSX3_PAL;
        private readonly AbstractAriesServer? RedirectorTSBO_NTSC_A;
        private readonly AbstractAriesServer? RedirectorTSBO_PAL;
        private readonly AbstractAriesServer? RedirectorFightNight_NTSC;
        private readonly AbstractAriesServer? RedirectorFightNightR2_NTSC;
        private readonly AbstractAriesServer? RedirectorFifa06_NTSC;
        private readonly AbstractAriesServer? RedirectorNFLStreet_NTSC;
        private readonly AbstractAriesServer? RedirectorNFLStreet2_NTSC;
        private readonly AbstractAriesServer? RedirectorNFLStreet3_NTSC;
        private readonly AbstractAriesServer? Redirector007EverythingOrNothing_NTSC;
        private readonly AbstractAriesServer? RedirectorNCAAMM06_NTSC;
        private readonly AbstractAriesServer? RedirectorMaddenNFL06_NTSC;
        private readonly AbstractAriesServer? RedirectorBurnout3Takedown_NTSC;
        private readonly AbstractAriesServer? RedirectorBurnoutRevenge_NTSC;
        private readonly AbstractAriesServer? RedirectorBOP_PS3;
        private readonly AbstractAriesServer? RedirectorBOPULTIMATEBOX_PS3;
        private readonly AbstractAriesServer? RedirectorBOPULTIMATEBOX_PC;
        private readonly AbstractAriesServer? RedirectorLordOfTheRingsTheReturnOfTheKing_NTSC;
        private readonly AbstractAriesServer? RedirectoNFSMWA124_PAL;
        private readonly AbstractAriesServer? RedirectorNascarThunder04_NTSC;
        private readonly AbstractAriesServer? RedirectorMarvelNemesis06_NTSC;
        private readonly AbstractAriesServer? RedirectorNASCAR08_PS3;
        private readonly AbstractAriesServer? RedirectorNASCAR09_PS3;
        private readonly AbstractAriesServer? RedirectorNASCAR09_SECURE_PS3;
        private readonly AbstractAriesServer? RedirectorHASBROFAMILYGAMENIGHT_PS3;
        private readonly AbstractAriesServer? RedirectorMIRRORSEDGE_PS3;

        private readonly AbstractAriesServer? Burnout3Takedown_NTSCMatchmaker;
        private readonly AbstractAriesServer? BurnoutRevenge_NTSCMatchmaker;
        private readonly AbstractAriesServer? BurnoutParadisePS3Matchmaker;
        private readonly AbstractAriesServer? BurnoutParadisePS3UltimateBoxMatchmaker;
        private readonly AbstractAriesServer? BurnoutParadisePCUltimateBoxMatchmaker;
        private readonly AbstractAriesServer? EverythingOrNothing007_NTSC_Matchmaker;
        private readonly AbstractAriesServer? LordOfTheRingsTheReturnOfTheKing_NTSC_Matchmaker;
        private readonly AbstractAriesServer? NascarThunder04_NTSC_Matchmaker;
        private readonly AbstractAriesServer? NCAAMM06_NTSC_Matchmaker;
        private readonly AbstractAriesServer? HASBROFAMILYGAMENIGHTPS3Matchmaker;
        private readonly AbstractAriesServer? NFLStreet_NTSCMatchmaker;
        private readonly AbstractAriesServer? NFLStreet2_NTSCMatchmaker;
        private readonly AbstractAriesServer? NFLStreet3_NTSCMatchmaker;
        private readonly AbstractAriesServer? FightNight_NTSCMatchmaker;
        private readonly AbstractAriesServer? FightNightR2_NTSCMatchmaker;
        private readonly AbstractAriesServer? NASCAR09_PS3_Matchmaker;
        private readonly AbstractAriesServer? Fifa06_NTSC_Matchmaker;
        private readonly AbstractAriesServer? MaddenNFL06_NTSC_Matchmaker;
        private readonly AbstractAriesServer? NFSMWA124_PAL_Matchmaker;
        private readonly AbstractAriesServer? MarvelNemesis_NTSC_Matchmaker;
        private readonly AbstractAriesServer? SimsMatchmaker;
        private readonly AbstractAriesServer? SSX3Matchmaker;
        private readonly AbstractAriesServer? PS2EAMessenger;
        private readonly AbstractAriesServer? PS3EAMessenger;
        private bool disposedValue;

        public AriesServer(CancellationToken cancellationToken)
        {
            string ListenIP = MultiSocksServerConfiguration.ServerBindAddress;

            Database = new DirtySocksJSONDatabase();

            #region Redirector
            try
            {
                RedirectorARMYOFTWO2010_PS3 = new RedirectorServer(18141, ListenIP, 18142, "ARMYOFTWO_2010", "PS3", true, "fesl.ea.com");
                LoggerAccessor.LogInfo($"[Redirector] ARMYOFTWO2010_PS3 Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] ARMYOFTWO2010_PS3 Failed to start! Exception: {ex}");
            }
            #region Army Of Two The 40th Day

            #endregion

            #region SSX3 PS2
            try
            {
               RedirectorSSX3_NTSC_A = new RedirectorServer(11000, ListenIP, 11051, "SSX-ER-PS2-2004", "PS2");
               LoggerAccessor.LogInfo($"[Redirector] SSX3_NTSC_A Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] SSX3_NTSC_A Failed to start! Exception: {ex}");
            }

            try
            {
                RedirectorSSX3_PAL = new RedirectorServer(11050, ListenIP, 11051, "SSX-ER-PS2-2004", "PS2");
                LoggerAccessor.LogInfo($"[Redirector] SSX3_PAL Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] SSX3_PAL Failed to start! Exception: {ex}");
            }
            #endregion

            #region The Sims Bustin' Out PS2
            try
            {
                RedirectorTSBO_NTSC_A = new RedirectorServer(11100, ListenIP, 11101, "TSBO", "PS2");
                LoggerAccessor.LogInfo($"[Redirector] TSBO_NTSC_A Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] TSBO_NTSC_A Failed to start! Exception: {ex}");
            }

            try
            {
                RedirectorTSBO_PAL = new RedirectorServer(11140, ListenIP, 11101, "TSBO", "PS2");
                LoggerAccessor.LogInfo($"[Redirector] TSBO_PAL Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] TSBO_PAL Failed to start! Exception: {ex}");
            }
            #endregion

            #region Fight Night 
            try
            {
                RedirectorFightNight_NTSC = new RedirectorServer(11500, ListenIP, 11501, "KKING-PS2-2004", "PS2", false, "ps2kok04.ea.com");
                RedirectorFightNightR2_NTSC = new RedirectorServer(21500, ListenIP, 21501, "KKING-PS2-2005", "PS2", false, "ps2kok05.ea.com");

                LoggerAccessor.LogInfo($"[Redirector] Fight Night NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] Fight Night NTSC Failed to start! Exception: {ex}");
            }
            #endregion

            #region NFL Street PS2
            try
            {
                RedirectorNFLStreet_NTSC = new RedirectorServer(11300, ListenIP, 11301, "NFLSTREET-PS2-2005", "PS2");
                LoggerAccessor.LogInfo($"[Redirector] NFL Street NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] NFL Street NTSC Failed to start! Exception: {ex}");
            }

            try
            {
                RedirectorNFLStreet2_NTSC = new RedirectorServer(21301, ListenIP, 21302, "NFLSTREET-PS2-2005", "PS2");
                LoggerAccessor.LogInfo($"[Redirector] NFL Street 2 NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] NFL Street 2 NTSC Failed to start! Exception: {ex}");
            }

            try
            {
                RedirectorNFLStreet3_NTSC = new RedirectorServer(11700, ListenIP, 11701, "NFLSTREET3-PS2-2007", "PS2");
                LoggerAccessor.LogInfo($"[Redirector] NFL Street 3 NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] NFL Street 3 NTSC Failed to start! Exception: {ex}");
            }

            #endregion

            #region Lord Of the Rings: The Return of the King
            try
            {
                RedirectorLordOfTheRingsTheReturnOfTheKing_NTSC = new RedirectorServer(11200, ListenIP, 11201, "LOTR", "PS2", false, "ps2rotk04.ea.com");
                LoggerAccessor.LogInfo($"[Redirector] LOTR:TROTK PS2 NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] LOTR:TROTK PS2 NTSC Failed to start! Exception: {ex}");
            }
            #endregion

            #region Burnout Paradise PS3
            try
            {
                RedirectorBOP_PS3 = new RedirectorServer(21850, ListenIP, 21851, "BURNOUT5", "PS3", false, "ps3burnout08.ea.com");
                LoggerAccessor.LogInfo($"[Redirector] BOP PS3 Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] BOP PS3 Failed to start! Exception: {ex}");
            }

            try
            {
                RedirectorBOPULTIMATEBOX_PS3 = new RedirectorServer(21870, ListenIP, 21871, "BURNOUT5", "PS3", false, "ps3burnout08.ea.com");
                LoggerAccessor.LogInfo($"[Redirector] BOPULTIMATEBOX PS3 Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] BOPULTIMATEBOX PS3 Failed to start! Exception: {ex}");
            }

            #endregion

            #region Burnout Paradise PC
            try
            {
                RedirectorBOPULTIMATEBOX_PC = new RedirectorServer(21841, ListenIP, 21842, "BURNOUT5", "PC", true, "pcburnout08.ea.com");
                LoggerAccessor.LogInfo($"[Redirector] BOPULTIMATEBOX PC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] BOPULTIMATEBOX PC Failed to start! Exception: {ex}");
            }
            #endregion

            #region Nascar

            try
            {
                RedirectorNascarThunder04_NTSC = new RedirectorServer(10600, ListenIP, 10601, "NASCAR-PS2-2004", "PS2", false, "ps2nascar04.ea.com");
                LoggerAccessor.LogInfo($"[Redirector] NASCAR Thunder 04 Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] NASCAR08 Thunder 04 Failed to start! Exception: {ex}");
            }

            try
            {
                RedirectorNASCAR08_PS3 = new RedirectorServer(20651, ListenIP, 20652, "NASCAR08", "PS3", true, "ps3nascar08.ea.com");
                LoggerAccessor.LogInfo($"[Redirector] NASCAR08 PS3 Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] NASCAR08 PS3 Failed to start! Exception: {ex}");
            }

            try
            {
                RedirectorNASCAR09_SECURE_PS3 = new RedirectorServer(30671, ListenIP, 30672, "NASCAR09", "PS3", true, "ps3nascar09.ea.com");
                LoggerAccessor.LogInfo($"[Redirector] NASCAR09 PS3 Secure Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] NASCAR09 PS3 Secure Failed to start! Exception: {ex}");
            }

            try
            {
                RedirectorNASCAR09_PS3 = new RedirectorServer(30670, ListenIP, 30672, "NASCAR09", "PS3", false);
                LoggerAccessor.LogInfo($"[Redirector] NASCAR09 PS3 Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] NASCAR09 PS3 Failed to start! Exception: {ex}");
            }
            #endregion

            #region Hasbro Family Game Night PS3
            try
            {
                RedirectorHASBROFAMILYGAMENIGHT_PS3 = new RedirectorServer(32950, ListenIP, 32951, "DPR-09", "PS3");
                LoggerAccessor.LogInfo($"[Redirector] HASBROFAMILYGAMENIGHT PS3 Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] HASBROFAMILYGAMENIGHT PS3 Failed to start! Exception: {ex}");
            }
            #endregion

            #region Mirror's Edge PS3
            try
            {
                RedirectorMIRRORSEDGE_PS3 = new RedirectorServer(18700, ListenIP, 18701, "MIRRORS_EDGE", "PS3", true, "fesl.ea.com");
                LoggerAccessor.LogInfo($"[Redirector] MIRRORSEDGE PS3 Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] MIRRORSEDGE PS3 Failed to start! Exception: {ex}");
            }
            #endregion

            #region NCAA March Madness 06
            try
            {
                RedirectorNCAAMM06_NTSC = new RedirectorServer(30700, ListenIP, 30701, "PS2-MM-2006", "PS2", false, "ps2mm06.ea.com");

                LoggerAccessor.LogInfo($"[Redirector] NCAA March Madness 06 NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] NCAA March Madness 06 NTSC Failed to start! Exception: {ex}");
            }
            #endregion

            #region Madden NFL 06
            try
            {
                RedirectorMaddenNFL06_NTSC = new RedirectorServer(30000, ListenIP, 30001, "PS2-MM-2006", "PS2", false, "ps2madden06.ea.com");

                LoggerAccessor.LogInfo($"[Redirector] Madden NFL 06 NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] Madden NFL 06 NTSC Failed to start! Exception: {ex}");
            }
            #endregion

            #region Fifa 06
            try
            {
                RedirectorFifa06_NTSC = new RedirectorServer(30400, ListenIP, 30401, "PS2-MM-2006", "PS2", false, "ps2fifa06.ea.com");

                LoggerAccessor.LogInfo($"[Redirector] Fifa 06 NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] Fifa 06 NTSC Failed to start! Exception: {ex}");
            }
            #endregion

            #region Marvel Nemesis 
            try
            {
                RedirectorMarvelNemesis06_NTSC = new RedirectorServer(31700, ListenIP, 31701, "NFLSTREET-PS2-2005", "PS2");
                LoggerAccessor.LogInfo($"[Redirector] Marvel Nemesis - Rise of the Imperfects NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] Marvel Nemesis - Rise of the Imperfects NTSC Failed to start! Exception: {ex}");
            }

            #endregion

            #region Burnout 3 Takedown
            try
            {
                RedirectorBurnoutRevenge_NTSC = new RedirectorServer(21800, ListenIP, 21801, "BR-PS2-2004", "PS2", false, "ps2burnout05.ea.com");

                LoggerAccessor.LogInfo($"[Redirector] Burnout 3 Takedown NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] Burnout 3 Takedown NTSC Failed to start! Exception: {ex}");
            }
            #endregion

            #region Burnout Revenge
            try
            {
                RedirectorBurnoutRevenge_NTSC = new RedirectorServer(31800, ListenIP, 31801, "BR-PS2-2004", "PS2", false, "ps2burnout06.ea.com");

                LoggerAccessor.LogInfo($"[Redirector] Burnout Revenge NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] Burnout Revenge NTSC Failed to start! Exception: {ex}");
            }
            #endregion

            #region 007: Everything or Nothing
            try
            {
                Redirector007EverythingOrNothing_NTSC = new RedirectorServer(11600, ListenIP, 11601, "PS2-BOND-2004", "PS2", false, "ps2bond04.ea.com");

                LoggerAccessor.LogInfo($"[Redirector] 007 Everything Or Nothing NTSC Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] 007 Everything Or Nothing NTSC Failed to start! Exception: {ex}");
            }
            #endregion

            #region NFS:MW A124
            try
            {
                RedirectoNFSMWA124_PAL = new RedirectorServer(30900, ListenIP, 30901, "nfs-ps2-2006", "PS2", false, "ps2nfs06.ea.com");

                LoggerAccessor.LogInfo($"[Redirector] Need for Speed: Most Wanted Alpha 124 PAL Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Redirector] Need for Speed: Most Wanted Alpha 124 PAL Failed to start! Exception: {ex}");
            }
            #endregion

            #endregion

            #region EA.com Messenger Buddy Server
            try
            {
                PS2EAMessenger = new EAMessengerServer(10899, ListenIP, null, "PS2");
                LoggerAccessor.LogInfo($"[PS2EAMessenger] Buddy Service Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[PS2EAMessenger] Buddy Service Failed to start! Exception: {ex}");
            }

            try
            {
                PS3EAMessenger = new EAMessengerServer(13505, ListenIP, null, "PS3");
                LoggerAccessor.LogInfo($"[PS3EAMessenger] Buddy Service Started!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[PS3EAMessenger] Buddy Service Failed to start! Exception: {ex}");
            }
            #endregion

            #region Matchmaker
            try
            {
                NascarThunder04_NTSC_Matchmaker = new MatchmakerServer(10601, ListenIP, null, "NASCAR-PS2-2004", "PS2");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Nascar Thunder 04 NTSC Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                NFLStreet_NTSCMatchmaker = new MatchmakerServer(11301, ListenIP, null, "NFLSTREET-PS2-2004", "PS2");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[NFL Street NTSC Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                NFLStreet2_NTSCMatchmaker = new MatchmakerServer(21302, ListenIP, null, "NFLSTREET-PS2-2005", "PS2");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[NFL Street NTSC Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                NFLStreet3_NTSCMatchmaker = new MatchmakerServer(11701, ListenIP, null, "NFLSTREET3-PS2-2007", "PS2");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[NFL Street NTSC Matchmaker] Failed to start! Exception: {ex}");
            }

            #region Madden NFL 06
            try
            {
                MaddenNFL06_NTSC_Matchmaker = new MatchmakerServer(30001, ListenIP, null, "MADDEN-06", "PS2", false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Madden NFL 06 Matchmaker] Failed to start! Exception: {ex}");
            }
            #endregion

            try
            {
                FightNight_NTSCMatchmaker = new MatchmakerServer(11501, ListenIP, null, "KKING-PS2-2004", "PS2", false);
                FightNightR2_NTSCMatchmaker = new MatchmakerServer(21501, ListenIP, null, "KKING-PS2-2005", "PS2", false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Fight Night 04 Matchmaker] Failed to start! Exception: {ex}");
            }

            #region NASCAR09 PS3

            try
            {
                NASCAR09_PS3_Matchmaker = new MatchmakerServer(30672, ListenIP, null, "NASCAR09", "PS3", false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[NASCAR 09 Matchmaker] Failed to start! Exception: {ex}");
            }

            #endregion

            try
            {
                Burnout3Takedown_NTSCMatchmaker = new MatchmakerServer(21801, ListenIP, null, "BR-2005-PS2", "PS2", false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Burnout Revenge Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                MarvelNemesis_NTSC_Matchmaker = new MatchmakerServer(31701, ListenIP, new List<Tuple<string, bool>>()
                {
                    new("Earth", true)
                    }, "NASCAR-PS2-2004", "PS2");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Marvel Nemesis NTSC Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                BurnoutRevenge_NTSCMatchmaker = new MatchmakerServer(31801, ListenIP, null, "BR-2005-PS2", "PS2", false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Burnout Revenge Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                Fifa06_NTSC_Matchmaker = new MatchmakerServer(30401, ListenIP, null, "BR-2005-PS2", "PS2", false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Fifa 06 Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                NFSMWA124_PAL_Matchmaker = new MatchmakerServer(30901, ListenIP, null, "nfs-ps2-2006", "PS2", false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Matchmaker] Need for Speed: Most Wanted A124 Failed to start! Exception: {ex}");
            }

            try
            {
                LordOfTheRingsTheReturnOfTheKing_NTSC_Matchmaker = new MatchmakerServer(11201, ListenIP, null, "PS2_ROTK_2004", "PS2", false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Everything Or Nothing 007 NTSC Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                NCAAMM06_NTSC_Matchmaker = new MatchmakerServer(30701, ListenIP, null, "MM05", "PS2");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[NFL Street NTSC Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                HASBROFAMILYGAMENIGHTPS3Matchmaker = new MatchmakerServer(32951, ListenIP, null, "DPR-09", "PS3");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Hasbro Family Game Night NTSC Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                EverythingOrNothing007_NTSC_Matchmaker = new MatchmakerServer(11601, ListenIP, null, "PS2-BOND-2004", "PS2", false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Everything Or Nothing 007 NTSC Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                BurnoutParadisePS3Matchmaker = new MatchmakerServer(21851, ListenIP, null, "BURNOUT5", "PS3");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[BurnoutParadise PS3 Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                BurnoutParadisePS3UltimateBoxMatchmaker = new MatchmakerServer(21871, ListenIP, null, "BURNOUT5", "PS3");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[BurnoutParadise PS3 UltimateBox Matchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                BurnoutParadisePCUltimateBoxMatchmaker = new MatchmakerServer(21842, ListenIP, null, "BURNOUT5", "PC");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[BurnoutParadise PC UltimateBox Matchmaker] Failed to start! Exception: {ex}");
            }


            try
            {
                SimsMatchmaker = new MatchmakerServer(11101, ListenIP, new List<Tuple<string, bool>>()
                {
                    new("Veronaville", true),
                    new("Strangetown", true),
                    new("Pleasantview", true),
                    new("Belladonna Cove", true),
                    new("Riverblossom Hills", true)
                }, "TSBO", "PS2");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SimsMatchmaker] Failed to start! Exception: {ex}");
            }

            try
            {
                SSX3Matchmaker = new MatchmakerServer(11051, ListenIP, null, "SSX-ER-PS2-2004", "PS2");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SimsMatchmaker] Failed to start! Exception: {ex}");
            }

            #endregion

            LoggerAccessor.LogInfo("DirtySocks Servers initiated...");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose all servers

                    // Redirectors
                    RedirectorSSX3_NTSC_A?.Dispose();
                    RedirectorSSX3_PAL?.Dispose();
                    RedirectorTSBO_NTSC_A?.Dispose();
                    RedirectorTSBO_PAL?.Dispose();
                    RedirectorFifa06_NTSC?.Dispose();
                    RedirectorFightNight_NTSC?.Dispose();
                    RedirectorFightNightR2_NTSC?.Dispose();
                    RedirectorNCAAMM06_NTSC?.Dispose();
                    RedirectorMaddenNFL06_NTSC?.Dispose();
                    RedirectorBurnout3Takedown_NTSC?.Dispose();
                    RedirectorBurnoutRevenge_NTSC?.Dispose();
                    RedirectorBOP_PS3?.Dispose();
                    RedirectorBOPULTIMATEBOX_PS3?.Dispose();
                    RedirectorBOPULTIMATEBOX_PC?.Dispose();
                    Redirector007EverythingOrNothing_NTSC?.Dispose();
                    RedirectorMarvelNemesis06_NTSC?.Dispose();
                    RedirectorNascarThunder04_NTSC?.Dispose();
                    RedirectorNASCAR08_PS3?.Dispose();
                    RedirectorNASCAR09_PS3?.Dispose();
                    RedirectoNFSMWA124_PAL?.Dispose();
                    RedirectorHASBROFAMILYGAMENIGHT_PS3?.Dispose();
                    RedirectorLordOfTheRingsTheReturnOfTheKing_NTSC?.Dispose();
                    RedirectorNFLStreet_NTSC?.Dispose();
                    RedirectorNFLStreet2_NTSC?.Dispose();
                    RedirectorNFLStreet3_NTSC?.Dispose();

                    //Matchmakers
                    Burnout3Takedown_NTSCMatchmaker?.Dispose();
                    BurnoutRevenge_NTSCMatchmaker?.Dispose();
                    BurnoutParadisePS3Matchmaker?.Dispose();
                    BurnoutParadisePCUltimateBoxMatchmaker?.Dispose();
                    BurnoutParadisePS3UltimateBoxMatchmaker?.Dispose();
                    Fifa06_NTSC_Matchmaker?.Dispose();
                    FightNight_NTSCMatchmaker?.Dispose();
                    FightNightR2_NTSCMatchmaker?.Dispose();
                    LordOfTheRingsTheReturnOfTheKing_NTSC_Matchmaker?.Dispose();
                    EverythingOrNothing007_NTSC_Matchmaker?.Dispose();
                    NFLStreet_NTSCMatchmaker?.Dispose();
                    NFLStreet2_NTSCMatchmaker?.Dispose();
                    NFLStreet3_NTSCMatchmaker?.Dispose();
                    NascarThunder04_NTSC_Matchmaker?.Dispose();
                    MaddenNFL06_NTSC_Matchmaker?.Dispose();
                    MarvelNemesis_NTSC_Matchmaker?.Dispose();
                    NCAAMM06_NTSC_Matchmaker?.Dispose();
                    NFSMWA124_PAL_Matchmaker?.Dispose();
                    HASBROFAMILYGAMENIGHTPS3Matchmaker?.Dispose();
                    SimsMatchmaker?.Dispose();
                    SSX3Matchmaker?.Dispose();

                    //EA.COM Buddy Server
                    PS2EAMessenger?.Dispose();
                    PS3EAMessenger?.Dispose();

                    //Database
                    Database = null;

                    LoggerAccessor.LogWarn("DirtySocks Servers stopped...");
                }

                // TODO: lib�rer les ressources non manag�es (objets non manag�s) et substituer le finaliseur
                // TODO: affecter aux grands champs une valeur null
                disposedValue = true;
            }
        }

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour lib�rer les ressources non manag�es
        // ~DirtySocksServer()
        // {
        //     // Ne changez pas ce code. Placez le code de nettoyage dans la m�thode 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la m�thode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
