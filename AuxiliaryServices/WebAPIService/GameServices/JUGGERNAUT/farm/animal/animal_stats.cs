using System.IO;
namespace WebAPIService.GameServices.JUGGERNAUT.farm.animal
{
    public class animal_stats
    {
        public static string ProcessStats(string apiPath)
        {
            if (File.Exists($"{apiPath}/juggernaut/farm/animal_stats.xml"))
                return File.ReadAllText($"{apiPath}/juggernaut/farm/animal_stats.xml");

            return "<xml><test>100.000</test><test1>500.000</test1></xml>";
        }
    }
}
