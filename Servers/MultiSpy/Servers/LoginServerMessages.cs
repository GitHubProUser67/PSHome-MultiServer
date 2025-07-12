using MultiSpy.Data;
using MultiSpyService.Utils;
using System.Net;
using System.Text;

namespace MultiSpy.Servers
{
    internal class LoginServerMessages
	{
		private readonly static Random _random = new Random();

		private readonly static Encoding _byteEncoder = Encoding.GetEncoding("ISO-8859-1");

		public static byte[] GenerateServerChallenge(ref LoginSocketState state)
		{
			state.ServerChallenge = _random.GetString(10);
			return DataFunctions.StringToBytes(String.Format(@"\lc\1\challenge\{0}\id\1\final\", state.ServerChallenge));
		}

        public static void HandleStatus(ref LoginSocketState state, Dictionary<string, string> keyValues)
		{
			if (keyValues.ContainsKey("logout"))
                state.Dispose();
		}

        public static byte[] SendProof(ref LoginSocketState state, Dictionary<string, string> keyValues)
		{
			string response = String.Empty;

			int requiredValues = 0;

			state.Name = String.Empty;

			if (keyValues.ContainsKey("uniquenick")) {
				state.Name = keyValues["uniquenick"];
				requiredValues++;
			}

			if (keyValues.ContainsKey("challenge")) {
				state.ClientChallenge = keyValues["challenge"];
				requiredValues++;
			}

			if (keyValues.ContainsKey("response")) {
				response = keyValues["response"];
				requiredValues++;
			}

			if (requiredValues != 3)
				return DataFunctions.StringToBytes(@"\error\\err\0\fatal\\errmsg\Invalid Query!\id\1\final\");

			var clientData = LoginDatabase.Instance.GetData(state.Name);

			if (clientData != null) {
				state.PasswordEncrypted = (string)clientData["passwordenc"];
				string responseValue = GenerateResponseValue(ref state);

                if (response.Equals(responseValue)) {
					ushort session = GenerateSession(state.Name);

					string proof = String.Format(@"\lc\2\sesskey\{0}\proof\{1}\userid\{2}\profileid\{3}\uniquenick\{4}\lt\{5}\id\1\final\",
						session,
						GenerateProofValue(state),
						clientData["userid"],
						clientData["profileid"],
						state.Name,
						_random.GetString(22, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ][") + "__");

					/*state.Session = session.ToString();
					Dictionary<string, object> updateClientData = new Dictionary<string, object>() {
						{ "session", session }
					};
					LoginDatabase.Instance.SetData(state.Name, updateClientData);*/

					LoginDatabase.Instance.LogLogin(state.Name, ((IPEndPoint)state.Socket.RemoteEndPoint).Address);

					state.State++;
					return DataFunctions.StringToBytes(proof);
				}
				else
				{
					CustomLogger.LoggerAccessor.LogError($"[LoginServerMessages] - SendProof - MD5 mismatch! response:{response} response_value:{responseValue}");
					return DataFunctions.StringToBytes(@"\error\\err\260\fatal\\errmsg\The password provided is incorrect.\id\1\final\");
				}
			}
			else
                return DataFunctions.StringToBytes(String.Format(@"\error\\err\265\fatal\\errmsg\Username [{0}] doesn't exist!\id\1\final\", state.Name));
        }

        public static byte[] SendProfile(ref LoginSocketState state, Dictionary<string, string> keyValues, bool retrieve)
		{
			var clientData = LoginDatabase.Instance.GetData(state.Name);

			if (clientData == null) {
				return DataFunctions.StringToBytes(String.Format(@"\error\\err\265\fatal\\errmsg\Username [{0}] doesn't exist!\id\1\final\", state.Name));
			}

			string message = String.Format(
				@"\pi\\profileid\{0}\nick\{1}\userid\{2}\email\{3}\sig\{4}\uniquenick\{5}\pid\{6}" +
				@"\firstname\lastname\countrycode\{7}\birthday\{8}\lon\{9}\lat\{10}\loc\id\{11}\final\",
				clientData["profileid"],
				state.Name,
				clientData["userid"],
				clientData["email"],
				_random.GetString(32, "0123456789abcdef"),
				state.Name,
				0,
				clientData["country"],
				16844722,
				"0.000000",
				"0.000000",
				retrieve ? 5 : 2
			);

			if (!retrieve)
				state.State++;

			return DataFunctions.StringToBytes(message);
		}

		public static void UpdateProfile(ref LoginSocketState state, Dictionary<string, string> keyValues)
		{
			string country = "??";
			if (keyValues.ContainsKey("countrycode")) {
				country = keyValues["countrycode"].ToUpperInvariant();
			}

			Dictionary<string, object> clientData = new Dictionary<string, object>() {
				{ "country", country }
			};

			LoginDatabase.Instance.SetData(state.Name, clientData);
			state.State++;
		}

		public static void Logout(ref LoginSocketState state, Dictionary<string, string> keyValues)
		{
			// we're not doing anything about session, so no need to reset it back to 0...
			// maybe one day though...
			/*Dictionary<string, object> clientData = new Dictionary<string, object>() {
				{ "session", (Int64)0 }
			};
			LoginDatabase.Instance.SetData(state.Name, clientData);*/
			state.Dispose();
		}

		public static byte[] NewUser(ref LoginSocketState state, Dictionary<string, string> keyValues)
		{
			string message = String.Empty;

			if (keyValues.ContainsKey("nick")) {
				state.Name = keyValues["nick"];
			} else {
				return DataFunctions.StringToBytes(@"\error\\err\0\fatal\\errmsg\Invalid Query!\id\1\final\");
			}
			
			if (keyValues.ContainsKey("email")) {
				state.Email = keyValues["email"];
			} else {
				return DataFunctions.StringToBytes(@"\error\\err\0\fatal\\errmsg\Invalid Query!\id\1\final\");
			}

			if (keyValues.ContainsKey("passwordenc")) {
				state.PasswordEncrypted = keyValues["passwordenc"];
			} else {
				return DataFunctions.StringToBytes(@"\error\\err\0\fatal\\errmsg\Invalid Query!\id\1\final\");
			}

			if (LoginDatabase.Instance.UserExists(state.Name)) {
				return DataFunctions.StringToBytes(@"\error\\err\516\fatal\\errmsg\This account name is already in use!\id\1\final\");
			} else {
				string password = DecryptPassword(state.PasswordEncrypted);

				// Verify password
				if (password.Length < 3) {
					return DataFunctions.StringToBytes(@"\error\\err\0\fatal\\errmsg\The password is too short, must be 3 characters at least!\id\1\final\"); 
				}
				if (password.Length > 30) {
					return DataFunctions.StringToBytes(@"\error\\err\0\fatal\\errmsg\The password is too long, must be 30 characters at most!\id\1\final\"); 
				}

				LoginDatabase.Instance.CreateUser(state.Name, NetHasher.DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(password)).ToLower(), state.Email, "??", ((IPEndPoint)state.Socket.RemoteEndPoint).Address);
				
				var clientData = LoginDatabase.Instance.GetData(state.Name);

				if (clientData == null) {
					return DataFunctions.StringToBytes(@"\error\\err\0\fatal\\errmsg\Error creating account!\id\1\final\");
				}

				message = String.Format(@"\nur\\userid\{0}\profileid\{1}\id\1\final\", clientData["userid"], clientData["profileid"]);
			}

			return DataFunctions.StringToBytes(message);
		}

		public static byte[] SendKeepAlive()
		{
			return DataFunctions.StringToBytes(@"\ka\\final\");
		}

		public static byte[] SendHeartbeat()
		{
			return DataFunctions.StringToBytes(String.Format(@"\lt\{0}\final\", _random.GetString(22, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ][") + "__"));
		}

		internal static byte[] SendNicks(ref LoginSocketState state, Dictionary<string, string> keyValues)
		{
			if (!keyValues.ContainsKey("email") || (!keyValues.ContainsKey("passenc") && !keyValues.ContainsKey("pass"))) {
				return DataFunctions.StringToBytes(@"\error\\err\0\fatal\\errmsg\Invalid Query!\id\1\final\");
			}

			string password = String.Empty;
			if (keyValues.ContainsKey("passenc")) {
				password = DecryptPassword(keyValues["passenc"]);
			} else if (keyValues.ContainsKey("pass")) {
				password = keyValues["pass"];
			}

			password = NetHasher.DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(password)).ToLower();

			var clientData = LoginDatabase.Instance.GetData(keyValues["email"], password);

			if (clientData == null) {
				return DataFunctions.StringToBytes(@"\error\\err\551\fatal\\errmsg\Unable to get any associated profiles.\id\1\final\");
			}

			List<string> nicks = new List<string>();
			foreach (var client in clientData) {
				nicks.Add((string)client["name"]);
			}

			if (nicks.Count == 0) {
				return DataFunctions.StringToBytes(@"\nr\0\ndone\\final\");
			}

			state.State++;
			return DataFunctions.StringToBytes(GenerateNicks(nicks.ToArray()));
		}

		private static string GenerateNicks(string[] nicks)
		{
			string message = @"\nr\" + nicks.Length;
			for (int i = 0; i < nicks.Length; i++) {
				message += String.Format(@"\nick\{0}\uniquenick\{0}", nicks[i]);
			}
			message += @"\ndone\final\";
			return message;
		}

		internal static byte[] SendCheck(ref LoginSocketState state, Dictionary<string, string> keyValues)
		{
			string name = String.Empty;

			if (String.IsNullOrWhiteSpace(name)) {
				if (keyValues.ContainsKey("uniquenick")) {
					name = keyValues["uniquenick"];
				}
			}
			if (String.IsNullOrWhiteSpace(name)) {
				if (keyValues.ContainsKey("nick")) {
					name = keyValues["nick"];
				}
			}
			if (String.IsNullOrWhiteSpace(name)) {
				return DataFunctions.StringToBytes(@"\error\\err\0\fatal\\errmsg\Invalid Query!\id\1\final\");
			}

			var clientData = LoginDatabase.Instance.GetData(name);

			if (clientData == null) {
				return DataFunctions.StringToBytes(String.Format(@"\error\\err\265\fatal\\errmsg\Username [{0}] doesn't exist!\id\1\final\", name));
			}

			string message = String.Format(@"\cur\0\pid\{0}\final\", clientData["profileid"]);

			return DataFunctions.StringToBytes(message);
		}

		private static string GenerateProofValue(LoginSocketState state)
		{
			string value = state.PasswordEncrypted;
			value += new String(' ', 48);
			value += state.Name;
			value += state.ServerChallenge;
			value += state.ClientChallenge;
			value += state.PasswordEncrypted;

			return NetHasher.DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(value)).ToLower();
		}

		private static string GenerateResponseValue(ref LoginSocketState state)
		{
			string value = state.PasswordEncrypted;
			value += new String(' ', 48);
			value += state.Name;
			value += state.ClientChallenge;
			value += state.ServerChallenge;
			value += state.PasswordEncrypted;

			return NetHasher.DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(value)).ToLower();
		}

		private static readonly ushort[] SessionCRCTable = new ushort[256] {
			0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
			0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
			0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
			0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
			0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
			0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
			0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
			0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
			0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
			0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
			0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
			0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
			0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
			0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
			0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
			0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
			0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
			0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
			0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
			0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
			0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
			0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
			0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
			0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
			0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
			0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
			0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
			0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
			0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
			0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
			0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
			0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
		};

		private static ushort GenerateSession(string name)
		{
			int len = name.Length;
			int nameIndex = 0;

			ushort session = 0;
			while (len-- != 0) {
				session = (ushort)(SessionCRCTable[((name[nameIndex] ^ session) & 0xff) % 256] ^ (session >> 8));
				nameIndex++;
			}

			return session;
		}

		private static string DecryptPassword(string password)
		{
			return gsEncode(gsBase64Decode(password));
		}

		public static string gsEncode(string password)
		{
			int i;
			int a;
			int c;
			int d;
			int num = 0x79707367;   // "gspy"
            byte[] pass = DataFunctions.StringToBytes(password);
            int passlen = pass.Length;

			if (num == 0)
				num = 1;
			else
				num &= 0x7fffffff;

			for (i = 0; i < passlen; i++) {
				d = 0xff;
				c = 0;
				d -= c;
				if (d != 0) {
					num = gsLame(num);
					a = num % d;
					a += c;
				} else
					a = c;

				pass[i] ^= (byte)(a % 256);
			}

			return DataFunctions.BytesToString(pass);
		}

		private static int gsLame(int num)
		{
			int a;
			int c = (num >> 16) & 0xffff;

			a = num & 0xffff;
			c *= 0x41a7;
			a *= 0x41a7;
			a += ((c & 0x7fff) << 16);

			if (a < 0) {
				a &= 0x7fffffff;
				a++;
			}

			a += (c >> 15);

			if (a < 0) {
				a &= 0x7fffffff;
				a++;
			}

			return a;
		}

		private static readonly char[] Base64Chars = new char[128] {   // supports also the Gamespy base64
			'\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00',
			'\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00',
			'\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00', '\x3e', '\x00', '\x00', '\x00', '\x3f',
			'\x34', '\x35', '\x36', '\x37', '\x38', '\x39', '\x3a', '\x3b', '\x3c', '\x3d', '\x00', '\x00', '\x00', '\x00', '\x00', '\x00',
			'\x00', '\x00', '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07', '\x08', '\x09', '\x0a', '\x0b', '\x0c', '\x0d', '\x0e',
			'\x0f', '\x10', '\x11', '\x12', '\x13', '\x14', '\x15', '\x16', '\x17', '\x18', '\x19', '\x3e', '\x00', '\x3f', '\x00', '\x00',
			'\x00', '\x1a', '\x1b', '\x1c', '\x1d', '\x1e', '\x1f', '\x20', '\x21', '\x22', '\x23', '\x24', '\x25', '\x26', '\x27', '\x28',
			'\x29', '\x2a', '\x2b', '\x2c', '\x2d', '\x2e', '\x2f', '\x30', '\x31', '\x32', '\x33', '\x00', '\x00', '\x00', '\x00', '\x00'
		};

		private static string gsBase64Decode(string s)
		{
			byte[] data = DataFunctions.StringToBytes(s);

			int datalen = data.Length;

			byte[] buff = new byte[((datalen >> 2) * 3) + 1];

			int a = 0;
			int b = 0;
			int c = 0;
			int buffoffs = 0;
			int dataoffs = 0;
			for (int step = 0; ; step++)
			{
				do
				{
					if (dataoffs >= datalen)
					{
						c = 0; // reached the end
						break;
					}
					else
					{
						c = data[dataoffs++];
						if ((c == '=') || (c == '_'))  // support also the Gamespy base64
						{
							c = 0;
							break;
						}
					}
				} while (c != 0 && ((c <= ' ') || (c > 0x7f)));
				if (c == 0) break;

				switch (step & 3)
				{
					case 0:
						a = Base64Chars[c];
						break;
					case 1:
						b = Base64Chars[c];
						buff[buffoffs++] = (byte)(((a << 2) | (b >> 4)) % 256);
						break;
					case 2:
						a = Base64Chars[c];
						buff[buffoffs++] = (byte)((((b & 15) << 4) | (a >> 2)) % 256);
						break;
					case 3:
						buff[buffoffs++] = (byte)((((a & 3) << 6) | Base64Chars[c]) % 256);
						break;
					default:
						break;
				}
			}

			return _byteEncoder.GetString(buff, 0, buffoffs);
		}
	}
}
