using System.Numerics;

namespace WebAPIService.GameServices.OHS
{
    public static class GlobalConstants
    {
        // cp_urls values
        public static class CpUrls
        {
            public static string SodiumBlimp = "{SCEA='Lockwood_Showcase_6663_C388/A/CPALOCKSHOW00.xml',SCEE='Lockwood_Showcase_6663_C388/E/CPELockwood00.xml',SCEJ='Sodium_Main_1864_A357/J/CPJSodium00.xml',SCEAsia='Sodium_Main_1864_A357/H/CPHSodium00.xml'}";
        }

        // vickie_version
        public static int VickieVersion = 7;

        // e3_global_data
        public static class E3GlobalData
        {
            public static string DustScene = "{ [\"unlocks\"] = { [\"opendate\"] = { [\"unlocked\"] = \"20130101120000\" } }, { [\"closedate\"] = { [\"unlocked\"] = \"21130101120000\" } } }";
        }

        // cp_global_data
        public static class CpGlobalData
        {
            public static string DustScene = "{ [\"unlocks\"] = { [\"opendate\"] = { [\"unlocked\"] = \"20130101120000\" } } }";
        }

        // voucher_global_data
        public static class VoucherGlobalData
        {
            public static string DustScene = "{" +
                " [\"vouchers\"] = { " +
                " [\"weekend1\"] = { [\"start\"] = \"20130101120000\" }, { [\"stop\"] = \"20130107120000\" }, { [\"SCEEopen\"] = \"20120628110000\" }, { [\"SCEEclose\"] = \"20120702113000\" }, { [\"SCEAopen\"] = \"20120628110000\" }, { [\"SCEAclose\"] = \"20120702113000\" } }," +
                " { [\"weekend2\"] = { [\"start\"] = \"20130108120000\" }, { [\"stop\"] = \"20130115120000\" }, { [\"SCEEopen\"] = \"20120712110000\" }, { [\"SCEEclose\"] = \"20120716113000\" }, { [\"SCEAopen\"] = \"20120712110000\" }, { [\"SCEAclose\"] = \"20120716113000\" } }," +
                " { [\"weekend3\"] = { [\"start\"] = \"20130116120000\" }, { [\"stop\"] = \"20130123120000\" }, { [\"SCEEopen\"] = \"20120716113000\" }, { [\"SCEEclose\"] = \"20120730113000\" }, { [\"SCEAopen\"] = \"20120716113000\" }, { [\"SCEAclose\"] = \"20120730113000\" } }," +
                " { [\"weekend4\"] = { [\"start\"] = \"20130124120000\" }, { [\"stop\"] = \"21130123120000\" }, { [\"SCEEopen\"] = \"20120809110000\" }, { [\"SCEEclose\"] = \"20120813113000\" }, { [\"SCEAopen\"] = \"20120809110000\" }, { [\"SCEAclose\"] = \"20120813113000\" } } }";
        }

        // global_data values
        public static class GlobalData
        {
            public static string DustSlay = "{ [\"unlocks\"] = { [\"week1\"] = { [\"unlocked\"] = \"20241112120000\" } } }";
            public static string Uncharted3 = "{ [\"unlocks\"] = \"WAVE3\",[\"community_score\"] = 1,[\"challenges\"] = { [\"accuracy\"] = 1 } }";
            public static string Halloween2012 = "{ [\"unlocks\"] = { [\"dance\"] = { [\"open\"] = \"20230926113000\", [\"closed\"] = \"20990926163000\" }, [\"limbo\"] = { [\"open\"] = \"20230926113000\", [\"closed\"] = \"20990926163000\" }, [\"hemlock\"] = { [\"open\"] = \"20230926113000\", [\"closed\"] = \"20990926163000\" }, [\"wolfsbane\"] = { [\"open\"] = \"20230926113000\", [\"closed\"] = \"20990926163000\" } } }";
            public static string DeadIsland = "{ [\"difficulty\"] = { [\"easy\"] = { [\"enemyDamage\"] = 0.4, [\"weaponDamage\"] = 1 }, [\"medium\"] = { [\"enemyDamage\"] = 0.8, [\"weaponDamage\"] = 1 }, [\"hard\"] = { [\"enemyDamage\"] = 1, [\"weaponDamage\"] = 0.85 } }, [\"unlocks\"] = { [\"wave_2\"] = { [\"unlocked\"] = \"2011-08-25T00:00:00\", [\"date\"] = \"25-08-2011\", [\"override\"] = false }, [\"wave_3\"] = { [\"unlocked\"] = \"2011-09-01T00:00:00\", [\"date\"] = \"01-09-2011\", [\"override\"] = false }, [\"receipe3\"] = { [\"unlocked\"] = \"2011-08-25T00:00:00\", [\"date\"] = \"25-08-2011\", [\"override\"] = false }, [\"receipe5\"] = { [\"unlocked\"] = \"2011-09-01T00:00:00\", [\"date\"] = \"01-09-2011\", [\"override\"] = false } }, [\"minDropInterval\"] = 10, [\"maxDropInterval\"] = 15, [\"maxDrops\"] = 4, [\"enableCheats\"] = false }";
            public static string SFxT = "{ [\"unlocks\"] = { [\"week1\"] = { [\"unlocked\"] = \"20250124000000\" }, [\"week2\"] = { [\"unlocked\"] = \"20250125000000\" }, [\"week3\"] = { [\"unlocked\"] = \"20250126000000\" } } }";
        }

        // unlock_data
        public static string Killzone3UnlockData = "{ [\"wave_1\"] = { [\"unlocked\"] = \"1999:10:10\", [\"override\"] = true }, [\"wave_2\"] = { [\"unlocked\"] = \"1999:10:10\", [\"override\"] = true }, [\"wave_3\"] = { [\"unlocked\"] = \"1999:10:10\", [\"override\"] = true } }";

        // max plaza reward
        public static string MaxSceaPlazaReward = "{ [\"maxSceaPlazaReward\"] = 5 }";

        // Desert Quench salaries
        public static int BartenderSalary = 2000;
        public static int CustomerSalary = 100;

        // Dragon Statue egg count
        public static BigInteger DragonStatueMax = 999999999999;
    }
}
