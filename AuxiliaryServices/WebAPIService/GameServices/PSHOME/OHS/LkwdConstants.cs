using System.Collections.Generic;

namespace WebAPIService.GameServices.PSHOME.OHS
{
    public class LkwdConstants
    {
        public static readonly Dictionary<string, ushort> TokensUUIDs = new Dictionary<string, ushort>
        {
            { "3CE5F061-126E455D-81BE66A9-A5FFE0DD", 61},
            { "41572A7D-7472434F-A9CC5E74-1C46EC72", 6240},
            { "558EDF38-5064415B-9D523BB1-E4826C88", 480},
            { "5B9BCC58-E794496A-B10A027C-DAFB1C19", 80 },
            { "DA1418BE-3B5B4493-BF2A2FDC-FF4E7EBD", 2780 },
            { "FAE204D3-40CA42AA-8055A1D3-8721EE08", 1180 }
        };

        public static readonly List<string> LockwoodLifeSceneList = new List<string>
        {
            "Dream_Yacht_Club_946D_7710",
            "Sunset_Lounge_EBEF_AD7C",
            "Dream_Forest_Apt_82AC_F9C3",
            "Ajagaras_Peak_Public_211C_B987",
            "Dream_Yacht_Arc_Club_9700_7EBC",
            "FortBellamyPublic_2A32_1E14",
            "Dream_Hideaway_Club_FB13_AB2E",
            "ArcticDreamYacht_Apt_D1C5_F674",
            "Castle_Strakh_CFDB_36FE",
            "Millionaires_Hub_1508_98C7",
            "Millionaires_Yacht_CC55_902E",
            "Haunted_Castle_E770_BD3B",
            "Dream_Island_Club_E2C0_8735",
            "DreamAdventureApt_C0D0_CDB0",
            "Ajagaras_Peak_Apt_4A94_3E08",
            "Millionaires_Beach_BC6F_6E33",
            "Dream_Forest_Club_6A43_ADD6",
            "Dream_Hideaway_Apt_577C_70EE"
        };

        public static readonly List<string> LockwoodDreamApartmentEntitlements = new List<string>
        {
            "1F06C254-A7E64A30-B6D21BDC-AA465F64", // Dream Yacht
            "2E962339-5B474D6B-B5330518-E9C24241", // Dream Yacht
            "1FA691D0-0F0444E1-8406D8F9-701754E7", // Dream Forest
            "8350F4C2-A07F4B1D-960F693C-4D3DFE72", // Dream Forest
            "287F383E-76304151-A60A94A1-37840CBD", // Dream Adventure
            "30354DAC-F0A44FCC-89C8621E-003A9BDA", // Dream Island
            "33229FDB-120042BB-8B13026E-4CD7EBF0", // Dream Island
            "304C0815-FC6546CD-932AF75D-15373378", // Dream Hideaway
            "65658D4A-1D7D46CD-9B99B056-639DB504", // Dream Hideaway
            "6480F726-67B14578-B068C0B6-977AF7E0", // Dream Yacht Nightmare
            "8D2C0A43-D2F54E94-B0D6C424-63D81900", // Dream Yacht Artic
            "E238192E-7D914A60-9BBC1F8A-58DE38F7" // Dream Yacht Artic
        };
    }
}
