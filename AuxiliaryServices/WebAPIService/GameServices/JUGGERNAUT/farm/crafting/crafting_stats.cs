using System.IO;
namespace WebAPIService.GameServices.JUGGERNAUT.farm.crafting
{
    public class crafting_stats
    {
        public static string ProcessGetStats(string apiPath)
        {
            if (File.Exists($"{apiPath}/juggernaut/farm/crafting_stats.xml"))
                return File.ReadAllText($"{apiPath}/juggernaut/farm/crafting_stats.xml");

            return @"<xml>
                <element>
                    <value>1</value>
                    <value>100</value>
                    <value>50</value>
                    <value>200</value>
                </element>
                <element>
                    <value>2</value>
                    <value>150</value>
                    <value>75</value>
                    <value>300</value>
                </element>
            </xml>";
        }
    }
}
