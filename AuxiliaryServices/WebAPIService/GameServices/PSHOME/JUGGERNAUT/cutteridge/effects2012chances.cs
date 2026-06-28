namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.cutteridge
{
    public class effects2012chances
    {
        public static string ProcessChances(string apiPath)
        {
            return File.Exists($"{apiPath}/juggernaut/cutteridge/effects2012chances.xml")
                ? File.ReadAllText($"{apiPath}/juggernaut/cutteridge/effects2012chances.xml")
                : "<scarecrow>500</scarecrow><girlChance>350</girlChance><doorChance>650</doorChance><kitchenChance>900</kitchenChance>";
        }
    }
}
