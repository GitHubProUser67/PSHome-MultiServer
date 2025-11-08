using System.Collections.Generic;

namespace WebAPIService.GameServices.PSHOME.TSS
{
    public static class LocalisationsFactory
    {
        public static Localisations Create() => new Localisations
        {
            Refs = new List<RefEntry>
            {
                new() { Language="*-ID", Value="en-SG" },
                new() { Language="*-MY", Value="en-SG" },
                new() { Language="*-TH", Value="en-SG" },
                new() { Language="en-HK", Value="en-SG" },
                new() { Language="*-HK", Value="zh-HK" },
                new() { Language="en-TW", Value="en-SG" },
                new() { Language="*-TW", Value="zh-TW" },
                new() { Language="*-SG", Value="en-SG" },
                new() { Language="en-US", Value="en-US" },
                new() { Language="en-CA", Value="en-US" },
                new() { Language="en-*", Value="en-GB" },
                new() { Language="fr-*", Value="fr-FR" },
                new() { Language="it-*", Value="it-IT" },
                new() { Language="de-*", Value="de-DE" },
                new() { Language="es-*", Value="es-ES" },
                new() { Language="ja-*", Value="ja-JP" },
                new() { Language="ko-*", Value="ko-KR" }
            }
        };
    }
}
