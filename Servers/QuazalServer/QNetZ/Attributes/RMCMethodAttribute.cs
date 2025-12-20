namespace QuazalServer.QNetZ.Attributes
{
	/// <summary>
	/// RMC method attribute for identifying function as a method handler
	/// Quazal::Protocol::AddMethodID
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class RMCMethodAttribute : Attribute
	{
		public readonly uint MethodId;
		public readonly string? Name;

		public RMCMethodAttribute(uint methodId, string? name = null)
		{
			MethodId = methodId;
			Name = name;
		}
	}
}