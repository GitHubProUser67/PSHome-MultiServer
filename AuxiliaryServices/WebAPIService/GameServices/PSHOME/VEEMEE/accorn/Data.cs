namespace WebAPIService.GameServices.PSHOME.VEEMEE.accorn
{
    public static class Data
    {
        public static string ParkChallenges(string apiPath)
        {
            return File.Exists($"{apiPath}/VEEMEE/Acorn_Medow/challenges.json")
                ? Processor.Sign(File.ReadAllText($"{apiPath}/VEEMEE/Acorn_Medow/challenges.json"))
                : null;
        }

        public static string ParkTasks(string apiPath)
        {
            return File.Exists($"{apiPath}/VEEMEE/Acorn_Medow/tasks.json")
                ? Processor.Sign(File.ReadAllText($"{apiPath}/VEEMEE/Acorn_Medow/tasks.json"))
                : null;
        }
    }
}
