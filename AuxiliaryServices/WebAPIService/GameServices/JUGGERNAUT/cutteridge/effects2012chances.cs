using System.IO;
namespace WebAPIService.GameServices.JUGGERNAUT.cutteridge
{
    public class effects2012chances
    {
        public static string ProcessChances(string apiPath)
        {
            if (File.Exists($"{apiPath}/juggernaut/cutteridge/effects2012chances.xml"))
                return File.ReadAllText($"{apiPath}/juggernaut/cutteridge/effects2012chances.xml");

            return "<scarecrow>500</scarecrow><girlChance>350</girlChance><doorChance>650</doorChance><kitchenChance>900</kitchenChance>";
        }
    }
}
