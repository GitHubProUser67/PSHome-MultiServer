using System.Collections.Generic;
namespace WebAPIService.GameServices.NDREAMS.Aurora
{
    public static class AuroraXPTable
    {
        private static readonly Dictionary<ushort, int> LevelsNxp = new Dictionary<ushort, int>()
        {
            {1, 0},
            {2, 1371},
            {3, 1740},
            {4, 2209},
            {5, 2805},
            {6, 3561},
            {7, 4520},
            {8, 5738},
            {9, 7284},
            {10, 9247},
            {11, 11739},
            {12, 14902},
            {13, 18918},
            {14, 24016},
            {15, 30488},
            {16, 38703},
            {17, 49132},
            {18, 62372},
            {19, 79179},
            {20, 100515},
            {21, 127600},
            {22, 161984},
            {23, 205634},
            {24, 261045},
            {25, 331388},
            {26, 420686},
            {27, 534047},
            {28, 677955},
            {29, 680394},
            {30, 682843},
            {31, 685299},
            {32, 687765},
            {33, 690240},
            {34, 692723},
            {35, 695216},
            {36, 697717},
            {37, 700228},
            {38, 702747},
            {39, 705276},
            {40, 707814},
            {41, 710360},
            {42, 712916},
            {43, 715482},
            {44, 718056},
            {45, 720640},
            {46, 723233},
            {47, 725835},
            {48, 728447},
            {49, 731068},
            {50, 733698},
            {51, 736338},
            {52, 738987},
            {53, 741646},
            {54, 744315},
            {55, 746993},
            {56, 749681},
            {57, 752378},
            {58, 755085},
            {59, 757802},
            {60, 760529},
            {61, 763265},
            {62, 766012},
            {63, 768768},
            {64, 771534},
            {65, 774310},
            {66, 777096},
            {67, 779892},
            {68, 782699},
            {69, 785515},
            {70, 788341},
            {71, 791178},
            {72, 794024},
            {73, 796881},
            {74, 799749},
            {75, 802626},
            {76, 805514},
            {77, 808413},
            {78, 811321},
            {79, 814241},
            {80, 817170},
            {81, 820111},
            {82, 823061},
            {83, 826023},
            {84, 828995},
            {85, 831978},
            {86, 834971},
            {87, 837976},
            {88, 840991},
            {89, 844017},
            {90, 847054},
            {91, 850102},
            {92, 853160},
            {93, 856230},
            {94, 859311},
            {95, 862403},
            {96, 865506},
            {97, 868620},
            {98, 871746},
            {99, 874882},
            {100, 878030},
            {101, 881189},
            {102, 884360},
            {103, 887542},
            {104, 890736},
            {105, 893941},
            {106, 897157},
            {107, 900385},
            {108, 903625},
            {109, 906876},
            {110, 910139},
            {111, 913414},
            {112, 916701},
            {113, 919999},
            {114, 923309},
            {115, 926631},
            {116, 929966},
            {117, 933312},
            {118, 936670},
            {119, 940040},
            {120, 943423},
            {121, 946817},
            {122, 950224},
            {123, 953643},
            {124, 957074},
            {125, 960518},
            {126, 963974},
            {127, 967442},
            {128, 970923},
            {129, 974417},
            {130, 977923},
            {131, 981442},
            {132, 984973},
            {133, 988517},
            {134, 992074},
            {135, 995643},
            {136, 999226},
            {137, 1002821},
            {138, 1006430},
            {139, 1010051},
            {140, 1013685},
            {141, 1017332},
            {142, 1020993},
            {143, 1024667},
            {144, 1028353},
            {145, 1032054},
            {146, 1035767},
            {147, 1039494},
            {148, 1043234},
            {149, 1046988},
            {150, 1050755}
        };

        private static readonly Dictionary<int, string> XpRewards = new Dictionary<int, string>()
        {
            {1371, "Aurora Energy goggles - bronze"},
            {2805, "Aurora Teleporter active item"},
            {9247, "nDreams Logo t-shirt"},
            {14902, "Aurora Ship outfit"},
            {100515, "Aurora t-shirt"},
            {331388, "Aurora Energy goggles - silver"},
            {682843, "Twitcher wall hanging"},
            {695216, "Twitcher outfit"},
            {707814, "Aurora Island ornament"},
            {720640, "Aurora Ship wall hanging"},
            {733698, "Mini Lucille active item"},
            {746993, "Aurora Portrait wall hanging"},
            {760529, "Aurora Picnic flooring"},
            {774310, "Landshark Coffee Bar table"},
            {788341, "Sky Skiff Seat chair"},
            {802626, "Jane's Love Seat chair"},
            {817170, "Aurora Dancefloor flooring"},
            {831978, "Captain Barber's Propeller wall hanging, The Face Melter chair, and Clux-Fapacitor chair"},
            {847054, "Lucille ornament, and Lilith ornament"},
            {862403, "Sir Lord Winterbottom III inventory item"},
            {878030, "Aurora Champion Apartment (personal space)"},
            {893941, "Mood light"},
            {910139, "Aurora Blimp LMO inventory item"},
            {926631, "Bladestream Corp: V1000Z LMO inventory item"},
            {943423, "Aurora Showcase Apartment (personal space)"},
            {960518, "Beautiful Shadow Companion inventory item"},
            {977923, "nDreams Dance Pack"},
            {995643, "nDreams Sci-Fi chair"},
            {1013685, "Pop-Up Chair LMO inventory item"},
            {1032054, "Aurora Dance Pack"},
            {1050755, "Aurora Chinatown Apartment (personal space)"}
        };

        public static ushort FindClosestPreviousLevel(int xp)
        {
            ushort closestLevel = 1;
            foreach (KeyValuePair<ushort, int> pair in LevelsNxp)
            {
                if (pair.Value <= xp && pair.Key >= closestLevel)
                    closestLevel = pair.Key;
            }
            return closestLevel;
        }

        public static int FindClosestHigherXP(int xp)
        {
            int closestHigherXP = 1050755;
            foreach (KeyValuePair<ushort, int> exp in LevelsNxp)
            {
                if (exp.Value >= xp && exp.Value <= closestHigherXP)
                    closestHigherXP = exp.Value;
            }

            return closestHigherXP;
        }

        public static int FindClosestLowerXP(int xp)
        {
            int closestLowerXP = 0;
            foreach (KeyValuePair<ushort, int> exp in LevelsNxp)
            {
                if (exp.Value <= xp && exp.Value >= closestLowerXP)
                    closestLowerXP = exp.Value;
            }

            return closestLowerXP;
        }

        public static int GetClosestLowerRewardXP(int xp)
        {
            if (XpRewards.TryGetValue(xp, out _))
                return xp;

            int closestLowerXP = 0;
            foreach (int key in XpRewards.Keys)
            {
                if (key <= xp && key >= closestLowerXP)
                    closestLowerXP = key;
            }

            return closestLowerXP;
        }

        public static int GetClosestHigherRewardXP(int xp)
        {
            if (XpRewards.TryGetValue(xp, out _))
                return xp;

            int closestHigherXP = 1050755;
            foreach (int key in XpRewards.Keys)
            {
                if (key >= xp && key <= closestHigherXP)
                    closestHigherXP = key;
            }

            return closestHigherXP;
        }
    }
}
