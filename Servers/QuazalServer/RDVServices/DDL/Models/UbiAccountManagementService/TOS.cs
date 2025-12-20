namespace QuazalServer.RDVServices.DDL.Models
{
    public class TOS
    {
        public TOS(string country_code, string language_code)
        {
            m_locale_code = $"{language_code}_{country_code}";
            m_content = "Welcome to MultiServer2XP Terms of service, well, just enjoy and play, that's it!";
            m_storing_info_question = "ApparentlyNobodyCanSeeMe";
        }

        public string? m_locale_code { get; set; }
        public string? m_content { get; set; }
        public string? m_storing_info_question { get; set; }
    }

    public class TOSEx
    {
        public TOSEx()
        {
            m_locale_code = "en_US";
            m_content = new List<string>() { "Welcome to MultiServer2XP Terms of service, well, just enjoy and play, that's it!", "Wait, you still are having no fun?" };
            m_storing_info_question = "ApparentlyNobodyCanSeeMe";
        }

        public string? m_locale_code { get; set; }
        public List<string>? m_content { get; set; }
        public string? m_storing_info_question { get; set; }
    }
}
