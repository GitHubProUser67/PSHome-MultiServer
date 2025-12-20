namespace QuazalServer.RDVServices.DDL.Models
{
    public class ValidationFailureReason
    {
        public uint m_validation_id { get; set; }
        public string? m_description { get; set; }
    }

    public class UsernameValidation
    {
        public UsernameValidation()
        {
            m_suggestions = new List<string>();
            m_reasons = new List<UsernameValidation>();
        }

        public List<string> m_suggestions { get; set; }
        public List<UsernameValidation>? m_reasons { get; set; }
    }
}
