using System.Collections.Generic;
using static WebAPIService.GameServices.PSHOME.NDREAMS.Xi1.StatusBuilder;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Xi1
{
    internal static class EncodedStatus
    {
        public static readonly Dictionary<StatusDay, string> DayCodes = new()
        {
            { StatusDay.Day1, "7BA2-1315-0453-6196" },
            { StatusDay.Day2, "95F3-5231-38C1-4DCE" },
            { StatusDay.Day3, "0726-0B48-3A99-2383" },
            { StatusDay.Day4, "E7BE-31D5-7A61-CCC2" },
            { StatusDay.Day5, "B0BB-BF9E-569B-85F7" },
            { StatusDay.Day6, "1CCB-76A7-C377-D6BF" },
            { StatusDay.Day7, "A377-D88F-AADE-ADA3" },
            { StatusDay.Day8, "FE0C-6A60-6CBA-9DA0" },
            { StatusDay.Day9, "32BF-4691-0D58-F0A6" },
            { StatusDay.Day10, "1E90-084E-0045-3E26" },
            { StatusDay.Day11, "43E1-9E92-816B-E6A3" },
            { StatusDay.Day12, "DE2D-D076-DBF4-5BA9" },
            { StatusDay.Day13, "AC78-EAD2-E93D-8EBA" },
            { StatusDay.Day14, "9C26-4E6F-3AED-FF98" },
            { StatusDay.Day15, "212D-8887-25E4-3297" },
            { StatusDay.Day16, "2865-2511-EE9E-E368" },
            { StatusDay.Day17, "D1C5-75B8-5AF7-1460" },
            { StatusDay.Day18, "CE1E-041E-D276-5162" },
            { StatusDay.Day19, "8EBE-FE1B-A927-904A" },
            { StatusDay.Day20, "CD4C-94FE-92B8-B796" },
            { StatusDay.Day21, "028C-67B2-140D-3AC4" },
            { StatusDay.Day22, "6E96-1210-E735-AA5D" },
            { StatusDay.Day23, "EB45-DFAF-03AA-9454" },
            { StatusDay.Day24, "8612-7A42-02B2-1C21" },
            { StatusDay.Day25, "5C5F-5266-37D3-50C8" },
            { StatusDay.Day26, "1187-EA62-C108-2696" },
            { StatusDay.Day27, "E338-9C99-A3EF-92C3" },
            { StatusDay.Day28, "620F-F8C8-B463-1791" },
            { StatusDay.Day29, "BC0A-59EC-0C85-E611" },
            { StatusDay.Day30, "CE50-9A75-FD90-A84D" },
            { StatusDay.Day31, "4AEE-FC2C-CF0A-9C21" },
        };

        public static readonly Dictionary<StatusMonth, string> MonthCodes = new()
        {
            { StatusMonth.January, "0BFB-AA51-4F0E-633C" },
            { StatusMonth.February, "4156-2004-DFBE-4310" },
            { StatusMonth.March, "DD4B-4CE0-B500-DEAD" },
            { StatusMonth.April, "393E-9DC6-639D-CE8C" },
            { StatusMonth.May, "4D7E-3622-BFF3-1396" },
            { StatusMonth.June, "A4AB-F1DA-5AAF-A408" },
            { StatusMonth.July, "E75E-89DA-DF1B-05EC" },
            { StatusMonth.August, "9FAB-79FB-D02B-3825" },
            { StatusMonth.September, "B622-0E3A-6BA0-A8B9" },
            { StatusMonth.October, "2817-8811-F9F4-EB6F" },
            { StatusMonth.November, "0567-33F4-4E99-289A" },
            { StatusMonth.December, "6BFE-FDDA-544D-E769" }
        };

        public static readonly Dictionary<int, string> Alpha1Doors = new()
        {
            { 1,   "63EF-59F6-FA7F-A9A2" },
            { 2,   "6BD5-6F19-6AE6-B25A" },
            { 3, "C3C0-EA9A-138D-5B01" }
        };

        public static readonly Dictionary<int, string> Alpha2Puzzles = new()
        {
            { 1,   "D005-533A-544C-7813" },
            { 2,   "79AA-6BDD-D68A-6BD3" },
            { 3, "B695-0960-BB8D-48C4" }
        };

        public static readonly Dictionary<int, string> Alpha3Doors = new()
        {
            { 1,   "DED5-9BA2-8C37-2AEC" },
            { 2,   "E83A-6F5F-FC68-8CC4" },
            { 3, "770F-102A-F5FE-BF14" },
            { 4,  "3E89-B262-BC16-12CE" }
        };

        public static readonly Dictionary<TD32Missions, string> TD32 = new()
        {
            { TD32Missions.M1,  "5F83-4338-4969-FEB3" },
            { TD32Missions.M2,  "5702-1924-CA00-CCF1" },
            { TD32Missions.M3,  "60D7-BB9C-ED92-E057" },
            { TD32Missions.M4,  "380C-06E2-A6DB-CDDB" },
            { TD32Missions.M5,  "D97B-ECF3-4C75-96A0" },
            { TD32Missions.M6,  "9FB4-A924-A4FD-9AB3" },
            { TD32Missions.M7,  "4821-5193-76A8-77ED" },
            { TD32Missions.M8,  "24B6-CB03-97EC-76BC" },
            { TD32Missions.M9,  "33FF-9354-7D29-C8DE" },
            { TD32Missions.M10, "C3E5-D843-AE65-4A79" },
            { TD32Missions.M11, "6DCF-AF9C-8D44-49DA" },
            { TD32Missions.M12, "7A90-4C6D-C2DA-2D2C" },
            { TD32Missions.M13, "583C-5022-4EC2-AB7B" },
            { TD32Missions.M14, "5119-18C1-9417-6A1A" },
            { TD32Missions.M15, "590F-F149-B1AD-2656" },
            { TD32Missions.M16, "C906-D6B3-CD43-1616" },
            { TD32Missions.M17, "E137-5F75-9137-EBAC" },
            { TD32Missions.M18, "BCAE-F632-9D9D-01CC" },
            { TD32Missions.M19, "2BDA-6A28-B643-66B4" },
            { TD32Missions.M20, "10EA-9AA1-4F6A-05E4" },
        };

        public static readonly Dictionary<FragmentMissions, string> Fragment = new()
        {
            { FragmentMissions.F1,  "F9E0-532F-9F53-4CBD" },
            { FragmentMissions.F2,  "9C77-4D56-5AA1-FFE0" },
            { FragmentMissions.F3,  "C95D-BF8F-846A-2DD5" },
            { FragmentMissions.F4,  "74EC-70EC-AA75-0EC1" },
            { FragmentMissions.F5,  "784D-D378-3417-F73F" },
            { FragmentMissions.F6,  "BBA9-472E-06A5-CBBC" },
            { FragmentMissions.F7,  "4262-4E83-DA29-6CD2" },
            { FragmentMissions.F8,  "E600-4D81-E939-0DE2" },
            { FragmentMissions.F9,  "BDAF-39EC-D78E-4C69" },
            { FragmentMissions.F10, "D20F-22F3-4E57-C4E8" },
            { FragmentMissions.F11, "A1BB-D10D-DE6E-A770" },
            { FragmentMissions.F12, "BFC8-4965-DC09-305B" },
            { FragmentMissions.F13, "CF38-B5F7-F0FE-7129" },
            { FragmentMissions.F14, "E264-482D-45EE-78CF" },
            { FragmentMissions.F15, "BF92-A546-9250-123C" },
            { FragmentMissions.F16, "665E-A58B-7016-200B" },
            { FragmentMissions.F17, "77C6-3423-BC12-A17A" },
            { FragmentMissions.F18, "3565-4ED5-8FE7-2938" },
            { FragmentMissions.F19, "2C5D-D69E-7068-90A9" },
            { FragmentMissions.F20, "0BCC-164B-F2C7-C3F6" },
            { FragmentMissions.F21, "FBA4-E78D-1614-890A" },
            { FragmentMissions.F22, "4B3D-BE62-E497-8DA5" },
            { FragmentMissions.F23, "2A8A-F44B-AF08-1F44" },
            { FragmentMissions.F24, "7714-117B-E83D-8FCF" }
        };

        public static readonly Dictionary<VideosUnlocked, string> Videos = new()
        {
            { VideosUnlocked.V0, "CCDC-1E7B-51A3-A26F" },
            { VideosUnlocked.V1, "2586-4B4B-7214-F843" },
            { VideosUnlocked.V2, "E996-0BA1-075E-6CA5" },
            { VideosUnlocked.V3, "3402-5170-A25F-AD53" },
            { VideosUnlocked.V4, "B8A4-E9F9-C7B5-1A28" },
            { VideosUnlocked.V5, "5F43-C479-1804-234F" },
            { VideosUnlocked.V6, "C2E0-D321-2CB6-C69C" },
            { VideosUnlocked.V7, "4613-875F-8E38-5B7E" },
            { VideosUnlocked.V8, "4089-F3AD-50AB-612A" },
            { VideosUnlocked.V9, "DB71-1F94-05B0-3C30" },
            { VideosUnlocked.V10,"765C-0A81-9D4D-5714" }
        };

        public static readonly Dictionary<RecapVideo, string> Recap = new()
        {
            { RecapVideo.R1,  "0BFB-AA51-4F0E-633C" },
            { RecapVideo.R2,  "4156-2004-DFBE-4310" },
            { RecapVideo.R3,  "DD4B-4CE0-B500-DEAD" },
            { RecapVideo.R4,  "393E-9DC6-639D-CE8C" },
            { RecapVideo.R5,  "4D7E-3622-BFF3-1396" },
            { RecapVideo.R6,  "A4AB-F1DA-5AAF-A408" },
            { RecapVideo.R7,  "E75E-89DA-DF1B-05EC" },
            { RecapVideo.R8,  "9FAB-79FB-D02B-3825" },
            { RecapVideo.R9,  "B622-0E3A-6BA0-A8B9" },
            { RecapVideo.R10, "2817-8811-F9F4-EB6F" },
            { RecapVideo.R11, "0567-33F4-4E99-289A" },
            { RecapVideo.R12, "6BFE-FDDA-544D-E769" },
            { RecapVideo.R13, "6129-FAAA-5AFE-E768" }
        };
    }
}
