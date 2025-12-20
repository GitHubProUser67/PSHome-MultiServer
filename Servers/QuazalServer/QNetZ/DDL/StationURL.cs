namespace QuazalServer.QNetZ.DDL
{
	public class StationURL : IAnyData
	{
		public string? _urlString;	// cached string
	
		public string _urlScheme;
		public string _address;
		private Dictionary<string, int> _parameters;
		private bool _dirty;

		public StationURL()
		{
			_urlScheme = string.Empty;
			_address = string.Empty;
			_parameters = new Dictionary<string, int>();
			_dirty = false;

			Valid = false;
		}

		public StationURL(string? urlStringText) : this()
		{
			ParseStationUrl(urlStringText);
		}

		public StationURL(string scheme, string address, IDictionary<string, int> parameters) : this()
		{
			_urlScheme = scheme;
			_address = address;

			if (parameters != null)
			{
				foreach(var key in parameters.Keys)
				{
					_parameters.TryAdd(key, parameters[key]);
				}
			}
			_dirty = true;
		}

		public string? urlString {
			get
			{
				BuildUrlString();
				return _urlString;
			} 
			set
			{

				ParseStationUrl(value);
			}
		}

		public bool Valid { get; private set; }

		public int this[string key]
		{
			get
			{
				return _parameters[key];
			}
			set
			{
				_parameters[key] = value;
				_dirty = true;
			}
		}

		public string UrlScheme { get => _urlScheme; set{ _urlScheme = value; _dirty = true; } }  // "prudp" or "prudps"

		public string Address { get => _address; set { _address = value; _dirty = true; } }
		public Dictionary<string, int> Parameters { get => _parameters; set { _parameters = value; _dirty = true; } }

        public bool IsPublic
        {
            get
            {
                int type = 0;
                _parameters.TryGetValue("type", out type);
                return (type & 2) != 0;
            }
        }
        public bool IsBehindNAT
        {
            get
            {
                int type = 0;
                _parameters.TryGetValue("type", out type);
                return (type & 1) != 0;
            }
        }
        public bool IsGlobal { get => IsPublic && IsBehindNAT; }

        void BuildUrlString()
		{
			if (!_dirty)
				return;

			_dirty = false;
			Valid = false;

			// check the data
			if (string.IsNullOrWhiteSpace(_urlScheme))
			{
				_urlString = string.Empty;
				return;
			}

			if (string.IsNullOrWhiteSpace(_address))
			{
				_urlString = $"{ _urlScheme }:/";
				return;
			}

			string paramsString = string.Join(";", _parameters.Keys.Select(x => $"{x}={_parameters[x]}"));

			string strSep = paramsString.Length > 0 ? ";" : string.Empty;

			_urlString = $"{ _urlScheme }:/address={ _address }{strSep}{ paramsString }";
			Valid = true;
		}

        public bool Compare(StationURL otherUrl, IEnumerable<string> compareParameters = null)
        {
            if (otherUrl == null)
                return false;

            if (_urlScheme != otherUrl._urlScheme)
                return false;

            if (_address != otherUrl._address)
                return false;

            if (compareParameters == null)
                return true;

            foreach (var paramName in compareParameters)
            {
                int param = 0;
                int otherParam = 0;
                if (_parameters.TryGetValue(paramName, out param) && otherUrl._parameters.TryGetValue(paramName, out otherParam))
                {
                    if (param != otherParam)
                        return false;
                }
            }
            return true;
        }

        public void ParseStationUrl(string? newUrlValue)
		{
			if (!string.IsNullOrEmpty(newUrlValue))
			{
                _dirty = false;
                Valid = false;

                var urlParts = newUrlValue.Split(":/");
                if (urlParts.Length != 2)
                    return;

                _urlScheme = urlParts[0];

                var parameterList = urlParts[1].Split(";");
                foreach (var param in parameterList)
                {
                    var key_value = param.Split("=");
                    if (key_value.Length != 2)
                        return;

                    if (key_value[0] == "address")
                        _address = key_value[1];
                    else
                        _parameters.TryAdd(key_value[0], Convert.ToInt32(key_value[1]));
                }

                _urlString = newUrlValue;
                Valid = true;
            }
		}

		public override string? ToString()
		{
			return urlString;
		}

		public void Read(Stream s)
		{
			string value = Helper.ReadString(s);
			ParseStationUrl(value);
		}

		public void Write(Stream s)
		{
			Helper.WriteString(s, urlString);
		}
	}
}
