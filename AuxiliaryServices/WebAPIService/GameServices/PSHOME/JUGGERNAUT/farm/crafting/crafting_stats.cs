namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.crafting
{
    public class crafting_stats
    {
        public static string ProcessGetStats(string apiPath)
        {
            return File.Exists($"{apiPath}/juggernaut/farm/crafting_stats.xml")
                ? File.ReadAllText($"{apiPath}/juggernaut/farm/crafting_stats.xml")
                : @"<xml>
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
