using System.Collections.Generic;

namespace WebAPIService.GameServices.PSHOME.TSS
{
    public static class RegionMapFactory
    {
        public static RegionMap Create() => new RegionMap
        {
            MapEntries = new List<MapEntry>
            {
                new() { Code="ja-JP", Loc=0, Value="Asia-Japan" },
                new() { Code="en-US", Loc=0, Value="US" },
                new() { Code="en-CA", Loc=0, Value="US" },
                new() { Code="fr-CA", Loc=0, Value="US" },
                new() { Code="en-GB", Loc=0, Value="EU-GB" },
                new() { Code="en-IE", Loc=0, Value="EU-GB" },
                new() { Code="fr-BE", Loc=0, Value="EU-France" },
                new() { Code="nl-BE", Loc=0, Value="EU-GB" },
                new() { Code="fr-LU", Loc=0, Value="EU-France" },
                new() { Code="de-LU", Loc=0, Value="EU-Germany" },
                new() { Code="nl-NL", Loc=0, Value="EU-GB" },
                new() { Code="fr-FR", Loc=0, Value="EU-France" },
                new() { Code="de-DE", Loc=0, Value="EU-Germany" },
                new() { Code="de-AT", Loc=0, Value="EU-Germany" },
                new() { Code="de-CH", Loc=0, Value="EU-Germany" },
                new() { Code="fr-CH", Loc=0, Value="EU-France" },
                new() { Code="it-CH", Loc=0, Value="EU-Italy" },
                new() { Code="it-IT", Loc=0, Value="EU-Italy" },
                new() { Code="pt-PT", Loc=0, Value="EU-GB" },
                new() { Code="en-DK", Loc=0, Value="EU-GB" },
                new() { Code="da-DK", Loc=0, Value="EU-GB" },
                new() { Code="en-FI", Loc=0, Value="EU-GB" },
                new() { Code="fi-FI", Loc=0, Value="EU-GB" },
                new() { Code="en-NO", Loc=0, Value="EU-GB" },
                new() { Code="no-NO", Loc=0, Value="EU-GB" },
                new() { Code="en-SE", Loc=0, Value="EU-GB" },
                new() { Code="sv-SE", Loc=0, Value="EU-GB" },
                new() { Code="en-AU", Loc=0, Value="EU-GB" },
                new() { Code="en-NZ", Loc=0, Value="EU-GB" },
                new() { Code="es-ES", Loc=0, Value="EU-Spain" },
                new() { Code="ru-RU", Loc=0, Value="EU-GB" },
                new() { Code="en-AE", Loc=0, Value="EU-GB" },
                new() { Code="en-ZA", Loc=0, Value="EU-GB" },
                new() { Code="en-CZ", Loc=0, Value="EU-GB" },
                new() { Code="en-SA", Loc=0, Value="EU-GB" },
                new() { Code="en-PL", Loc=0, Value="EU-GB" },
                new() { Code="pl-PL", Loc=0, Value="EU-GB" },
                new() { Code="en-GR", Loc=0, Value="EU-GB" },
                new() { Code="en-HK", Loc=0, Value="Asia-HongKong" },
                new() { Code="zh-HK", Loc=0, Value="Asia-HongKong" },
                new() { Code="en-TW", Loc=0, Value="Asia-Taiwan" },
                new() { Code="zh-TW", Loc=0, Value="Asia-Taiwan" },
                new() { Code="en-SG", Loc=0, Value="Asia-Singapore" },
                new() { Code="ko-KR", Loc=0, Value="Asia-Korea" },
                new() { Code="en-ID", Loc=0, Value="Asia-Indonesia" },
                new() { Code="en-MY", Loc=0, Value="Asia-Malaysia" },
                new() { Code="en-TH", Loc=0, Value="Asia-Thailand" }
            }
        };
    }
}
