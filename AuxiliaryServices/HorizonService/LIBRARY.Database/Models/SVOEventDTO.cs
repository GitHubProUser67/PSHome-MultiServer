namespace Horizon.LIBRARY.Database.Models
{
    public class SVOEventDTO
    {
        public int Id { get; set; }
        public int categoryID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public int startYear { get; set; }
        public int startMonth { get; set; }
        public int startDay { get; set; }
        public int endYear { get; set; }
        public int endMonth { get; set; }
        public int endDay { get; set; }
        public bool isActive { get; set; }
        public bool isApproved { get; set; }
        public bool isBillboard { get; set; }
        public int territoryID { get; set; }
        public int languageID { get; set; }
        public int generic_A { get; set; }
        public int generic_B { get; set; }
        public int generic_C { get; set; }
        public string icon { get; set; }
        public string color { get; set; }
        public int accountId { get; set; }
        public string buffer_1 { get; set; }
        public string url_1 { get; set; }
        public string url_2 { get; set; }
        public int appID { get; set; }

        //Clan Specific Event
        public int ClanId { get; set; }
        public int generic_1 { get; set; }
        public int generic_2 { get; set; }
        public int generic_3 { get; set; }
        public int generic_4 { get; set; }
        public int generic_5 { get; set; }
    }
}
