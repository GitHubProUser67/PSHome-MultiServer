using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct AdminConfig
	{

		[TdfMember("UAMX")]
		public ushort mMaxAdminServersPerUser;

		[TdfMember("MRMX")]
		public ushort mMaxMapRotationEntries;

		[TdfMember("UOMX")]
		public ushort mMaxOwnedServersPerUser;

		[TdfMember("PQMX")]
		public uint mMaxPurchaseQuantity;

		[TdfMember("SAMX")]
		public ushort mMaxServerAdmins;

		[TdfMember("SBMX")]
		public ushort mMaxServerBans;

		[TdfMember("SVMX")]
		public ushort mMaxServerVips;

		[TdfMember("SEXP")]
		public TimeValue mServerExpiredPeriod;

		[TdfMember("SEXT")]
		public TimeValue mServerExtensionPeriod;

		[TdfMember("SREM")]
		public TimeValue mServerReminderPeriod;

	}
}
