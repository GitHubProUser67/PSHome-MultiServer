using static WebAPIService.GameServices.PSHOME.NDREAMS.Xi1.StatusBuilder;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Xi1
{
    public class StatusData
    {
        public StatusDay Day { get; set; } = StatusDay.Day1;
        public StatusMonth Month { get; set; } = StatusMonth.January;

        public bool HubOpen { get; set; } = true;
        public bool Alpha1Open { get; set; } = true;
        public bool Alpha1EnthOpen { get; set; } = true;
        public int Alpha1Doors { get; set; } = 3;

        public bool MaintenanceOpen { get; set; } = true;
        public bool Alpha2Open { get; set; } = true;
        public int Alpha2Puzzles { get; set; } = 3;

        public bool PartyOpen { get; set; } = true;
        public bool PartyOver { get; set; } = false;

        public bool Alpha3Open { get; set; } = true;
        public int Alpha3Doors { get; set; } = 4;

        public bool FinalDoor { get; set; } = true;
        public bool ArgComplete { get; set; } = true;

        public TD32Missions TD32Missions { get; set; } = TD32Missions.M20;

        public bool WebVeilCorp { get; set; } = true;
        public bool WebJessDesktop { get; set; } = true;

        public VideosUnlocked VideosUnlocked { get; set; } = VideosUnlocked.V10;

        public bool InnerBeauty { get; set; } = true;

        public FragmentMissions FragmentMissions { get; set; } = FragmentMissions.F24;

        public RecapVideo RecapVideoNum { get; set; } = RecapVideo.R13;
    }

}
