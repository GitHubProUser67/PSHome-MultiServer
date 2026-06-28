namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.plant
{
    public class plant_stats
    {
        public static string ProcessStats(string apiPath)
        {
            return File.Exists($"{apiPath}/juggernaut/farm/plant_stats.xml")
                ? File.ReadAllText($"{apiPath}/juggernaut/farm/plant_stats.xml")
                : "<xml><test>100.000</test><test1>500.000</test1></xml>";
        }
    }
}
