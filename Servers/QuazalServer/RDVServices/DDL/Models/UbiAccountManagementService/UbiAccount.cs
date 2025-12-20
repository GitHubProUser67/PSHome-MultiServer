namespace QuazalServer.RDVServices.DDL.Models
{
	public class UbiAccountStatus
	{
		public UbiAccountStatus()
		{
			m_basic_status = 0;
			m_missing_required_informations = false;
			m_recovering_password = false;
			m_pending_deactivation = false;
		}

		public uint m_basic_status { get; set; }
		public bool m_missing_required_informations { get; set; }
		public bool m_recovering_password { get; set; }
		public bool m_pending_deactivation { get; set; }
	}

	public class ExternalAccount
	{
		public ExternalAccount()
		{
			m_account_type = 0;
		}

		public uint m_account_type { get; set; }
		public string? m_id { get; set; }
		public string? m_username { get; set; }
	}

	public class UbiAccount
	{
		public UbiAccount()
		{
			m_external_accounts = new List<ExternalAccount>();
			m_status = new UbiAccountStatus();
			m_date_of_birth = new DateTime(2000, 10, 12);
		}

		public string? m_ubi_account_id { get; set; }
		public string? m_username { get; set; }
		public string? m_password { get; set; }
		public UbiAccountStatus m_status { get; set; }
		public string? m_email { get; set; }
		public DateTime m_date_of_birth { get; set; }
		public uint m_gender { get; set; }
		public string? m_country_code { get; set; } // Country Code in Caps (usually given earlier).
		public bool m_opt_in { get; set; } // Some registration stuff.
		public bool m_third_party_opt_in { get; set; } // Some registration stuff.
        public string? m_first_name { get; set; }
		public string? m_last_name { get; set; }
		public string? m_preferred_language { get; set; } // Language Code in lower Caps (usually given earlier).
        public List<ExternalAccount> m_external_accounts { get; set; }

	}
}
