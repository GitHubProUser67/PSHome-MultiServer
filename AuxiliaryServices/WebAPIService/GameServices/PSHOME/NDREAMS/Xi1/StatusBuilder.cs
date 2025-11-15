using System;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Xi1
{
    public class StatusBuilder
    {
        public enum StatusDay
        {
            Day1 = 1, Day2, Day3, Day4, Day5, Day6, Day7,
            Day8, Day9, Day10, Day11, Day12, Day13, Day14,
            Day15, Day16, Day17, Day18, Day19, Day20, Day21,
            Day22, Day23, Day24, Day25, Day26, Day27, Day28,
            Day29, Day30, Day31
        }

        public enum StatusMonth
        {
            January = 1, February, March, April, May, June,
            July, August, September, October, November, December
        }

        public enum TD32Missions
        {
            None = 0, M1 = 1, M2, M3, M4, M5, M6, M7, M8, M9, M10,
            M11, M12, M13, M14, M15, M16, M17, M18, M19, M20
        }

        public enum FragmentMissions
        {
            None = 0,
            F1 = 1, F2, F3, F4, F5, F6, F7, F8, F9, F10,
            F11, F12, F13, F14, F15, F16, F17, F18, F19, F20,
            F21, F22, F23, F24
        }

        public enum VideosUnlocked
        {
            V0 = 0, V1, V2, V3, V4, V5, V6, V7, V8, V9, V10
        }

        public enum RecapVideo
        {
            R1 = 1, R2, R3, R4, R5, R6,
            R7, R8, R9, R10, R11, R12, R13
        }

        // TODO, make it more dynamic (simulating the real event per days and months).
        public static string BuildStatusXml()
        {
            StatusData data = new StatusData();

            var currentTime = GetTodayCodes();

            data.Day = currentTime.dayCode;
            data.Month = currentTime.monthCode;

            return @$"<XML>
              <1>{EncodedStatus.DayCodes[data.Day]}</1>
              <2>{EncodedStatus.MonthCodes[data.Month]}</2>
              <3>{(data.HubOpen ? "F08B-B482-1589-46BF" : string.Empty)}</3>
              <4>{(data.Alpha1Open ? "2F14-D907-BBCE-14F1" : string.Empty)}</4>
              <5>{(data.Alpha1EnthOpen ? "6903-1BDD-B7B8-FA13" : string.Empty)}</5>
              <6>{EncodedStatus.Alpha1Doors[data.Alpha1Doors]}</6>
              <7>{(data.MaintenanceOpen ? "BE28-B221-CB8A-E8E2" : string.Empty)}</7>
              <8>{(data.Alpha2Open ? "AB81-1121-FAD2-399A" : string.Empty)}</8>
              <9>{EncodedStatus.Alpha2Puzzles[data.Alpha2Puzzles]}</9>
              <10>{(data.PartyOpen ? "2836-1231-47D1-12C5" : string.Empty)}</10>
              <11>{(data.PartyOver ? "BCAF-AA02-001A-148D" : string.Empty)}</11>
              <12>{(data.Alpha3Open ? "B023-31F3-FF10-CDA1" : string.Empty)}</12>
              <13>{EncodedStatus.Alpha3Doors[data.Alpha3Doors]}</13>
              <14>{(data.FinalDoor ? "B283-192D-22AA-C9CC" : string.Empty)}</14>
              <15>{(data.ArgComplete ? "9B9C-002A-A265-CC33" : string.Empty)}</15>
              <16>{EncodedStatus.TD32[data.TD32Missions]}</16>
              <17>{(data.WebVeilCorp ? "249A-BFD1-20D0-7774" : string.Empty)}</17>
              <18>{(data.WebJessDesktop ? "8282-C7C1-EE2A-A195" : string.Empty)}</18>
              <19>{EncodedStatus.Videos[data.VideosUnlocked]}</19>
              <20>{(data.InnerBeauty ? "C923-F12F-0D21-120E" : string.Empty)}</20>
              <21>{EncodedStatus.Fragment[data.FragmentMissions]}</21>
              <22>{EncodedStatus.Recap[data.RecapVideoNum]}</22>
            </XML>";
        }

        public static (StatusDay dayCode, StatusMonth monthCode) GetTodayCodes()
        {
            DateTime now = DateTime.Now;
            return ((StatusDay)now.Day, (StatusMonth)now.Month);
        }
    }
}
