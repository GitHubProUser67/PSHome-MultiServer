using System.Collections.Generic;

namespace WebAPIService.GameServices.PSHOME.TSS
{
    public static class RegionInfoFactory
    {
        public static RegionInfo Create() => new RegionInfo
        {
            InstanceTypes = new InstanceTypes
            {
                Types = new List<NamedType>
                {
                    new() { Name="EU" },
                    new() { Name="US" },
                    new() { Name="Japan" },
                    new() { Name="Asia" }
                }
            },
            RegionTypes = new RegionTypes
            {
                Types = new List<RegionType>
                {
                    new() { Name="EU-GB", Territory="SCEE", Instance="EU", Value="en-GB" },
                    new() { Name="EU-France", Territory="SCEE", Instance="EU", Value="fr-FR" },
                    new() { Name="EU-Italy", Territory="SCEE", Instance="EU", Value="it-IT" },
                    new() { Name="EU-Germany", Territory="SCEE", Instance="EU", Value="de-DE" },
                    new() { Name="EU-Spain", Territory="SCEE", Instance="EU", Value="es-ES" },
                    new() { Name="US", Territory="SCEA", Instance="US", Value="en-US" },
                    new() { Name="Asia-Japan", Territory="SCEJ", Instance="Japan", Value="ja-JP" },
                    new() { Name="Asia-Korea", Territory="SCEAsia", Instance="Asia", Value="ko-KR" },
                    new() { Name="Asia-Taiwan", Territory="SCEAsia", Instance="Asia", Value="zh-TW" },
                    new() { Name="Asia-HongKong", Territory="SCEAsia", Instance="Asia", Value="zh-HK" },
                    new() { Name="Asia-Singapore", Territory="SCEAsia", Instance="Asia", Value="en-SG" },
                    new() { Name="Asia-Indonesia", Territory="SCEAsia", Instance="Asia", Value="en-ID" },
                    new() { Name="Asia-Malaysia", Territory="SCEAsia", Instance="Asia", Value="en-MY" },
                    new() { Name="Asia-Thailand", Territory="SCEAsia", Instance="Asia", Value="en-TH" }
                }
            },
            RegionMap = RegionMapFactory.Create(),
            Localisations = LocalisationsFactory.Create()
        };
    }

}
