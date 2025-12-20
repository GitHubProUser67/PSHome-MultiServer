namespace QuazalServer.RDVServices.DDL.Models
{
    public class UplaySectionContentLocalized
    {
        public string? m_key { get; set; }
        public string? m_culture { get; set; }
        public string? m_text { get; set; }
        public string? m_url { get; set; }
        public int m_duration { get; set; }
        public string? m_size { get; set; }
        public string? m_width { get; set; }
        public string? m_height { get; set; }
    }

    public class UplaySectionContent
    {
        public string? m_key { get; set; }
        public string? m_name { get; set; }
        public short m_order { get; set; }
        public string? m_type_name { get; set; }
        public UplaySectionContentLocalized? m_localized_info { get; set; }
    }

    public class UplaySection
    {
        public string? m_key { get; set; }
        public string? m_name { get; set; }
        public string? m_type_name { get; set; }
        public string? m_menu_type_name { get; set; }
        public ICollection<UplaySectionContent>? m_content_list { get; set; }
        public string? m_game_code { get; set; }
        public string? m_platform_code { get; set; }

    }
}
