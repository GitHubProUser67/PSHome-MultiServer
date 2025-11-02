namespace WebAPIService.GameServices.HELLFIRE.Entities.HomeTycoon
{
    // Use this class to "hold back" a property (aka make the game not use it yet), we are gonna send empty really.
    internal class TycoonHoldbacks
    {
        public static readonly string[] Buildings = System.Array.Empty<string>();

        public static readonly string[] ExpansionPacks = System.Array.Empty<string>();

        public static readonly string[] Vehicles = System.Array.Empty<string>();
    }
}
