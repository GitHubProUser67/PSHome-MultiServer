namespace QuazalServer.RDVServices.DDL.Models
{
	public class StorageFile
	{
		public byte[] m_buffer { get; set; }
		public uint retcode { get; set; }
		public StorageFile()
		{
			m_buffer = Array.Empty<byte>();
			retcode = 0;
		}
	}
}
