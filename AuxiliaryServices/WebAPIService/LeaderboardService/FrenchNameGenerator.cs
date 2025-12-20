using MultiServerLibrary.Extension;
using System;
using System.Collections.Generic;

namespace WebAPIService.LeaderboardService
{
    public static class FrenchNameGenerator
    {
        private static readonly string[] Words =
        {
                // --- Classic Cheeses ---
                "Fromage", "Camembert", "Roquefort", "Brie", "Comte", "Munster",
                "Reblochon", "SaintNectaire", "Chevre", "Tomme", "Bleu",
                "Cantal", "Fourme", "PontLeveque", "Maroilles", "Raclette",
                "OssauIraty", "Abondance", "Livarot", "epoisses", "Chabichou",

                // --- Asterix & Obelix World ---
                "Asterix", "Obelix", "Idefix", "Panoramix", "Abraracourcix",
                "Assurancetourix", "Ordralfabetix", "Cetautomatix", "Goudurix",
                "Falbala", "Jolitorax", "Cleopatre", "Cesarix",

                // --- Pâtisserie / Desserts ---
                "Croissant", "PainAuChocolat", "eclair", "Macaron", "TarteTatin",
                "MilleFeuille", "Opera", "ParisBrest", "Madeleine", "Financier",
                "Brioche", "KouignAmann", "Chouquette", "Canele", "GaletteDesRois",

                // --- Funny / Slang / Random French words ---
                "ZutAlors", "SacreBleu", "Ohlala", "Bof", "Frometon",
                "Chouchou", "Pepito", "Doudou", "Minou", "Croquette",
                "Cocorico", "PouleMouillee", "Carambar", "Frangipane",
                "Biscotte", "Rigolo", "Pamplemousse", "Fripouille",
                "Chiffonnade", "Quenelle", "Tartiflette", "Bouillabaisse",

                // --- Rugby Legends (France + a few iconic internationals) ---
                "SergeBlanco", "PhilippeSella", "ThierryDusautoir", "FabienPelous",
                "SebastienChabal", "FredericMichalak", "RomainNtamack", "AntoineDupont",
                "DamienTraille", "RaphaelIbanez", "YannickJauzion", "JeanPierreRives",
                "OlivierMagne", "ImanolHarinordoquy", "ChristopheDominici",

                // a few international names for spice
                "JonahLomu", "RichieMcCaw", "DanCarter", "MartinJohnson", "BrianODriscoll",
                "HenryPotDeBeurre", "ChuckNorris", "TheRock", "SarahConnor", "GaudefroiDeMontmirail",
                "JacquouilleLaFripouille"
        };

        private static readonly int wordsLength = Words.Length;

        private static readonly Random Rng = new Random();

        public static string GetRandomWord()
        {
            if (DateTimeUtils.IsAprilFoolsDay())
                return Words[(int)(Math.Abs(DateTime.Now.Ticks
                       ^ Environment.TickCount
                       ^ Guid.NewGuid().GetHashCode()) % wordsLength)];
            return Words[Rng.Next(wordsLength)];
        }

        public static IEnumerable<string> GetRandomWords(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return GetRandomWord();
            }
        }
    }

}
