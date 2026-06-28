using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using MultiServerLibrary.HTTP;

namespace NetCoreServer
{
    /// <summary>
    /// HTTP response is used to create or process parameters of HTTP protocol response(status, headers, etc).
    /// </summary>
    /// <remarks>Not thread-safe.</remarks>
    public class HttpResponse
    {
        public const string allowedMethods = "OPTIONS, HEAD, GET, PUT, POST, DELETE, PATCH";

        public static List<string> allowedOrigins = [];

        static HttpResponse()
        {
            _mimeTable = HTTPProcessor.MimeTypes;
        }

        /// <summary>
        /// Initialize an empty HTTP response
        /// </summary>
        public HttpResponse()
        {
            Clear();
        }

        /// <summary>
        /// Initialize a new HTTP response with a given status and protocol
        /// </summary>
        /// <param name="status">HTTP status</param>
        /// <param name="protocol">Protocol version (default is "HTTP/1.1")</param>
        public HttpResponse(int status, string protocol = "HTTP/1.1")
        {
            SetBegin(status, protocol);
        }

        /// <summary>
        /// Initialize a new HTTP response with a given status, status phrase and protocol
        /// </summary>
        /// <param name="status">HTTP status</param>
        /// <param name="statusPhrase">HTTP status phrase</param>
        /// <param name="protocol">Protocol version</param>
        public HttpResponse(int status, string statusPhrase, string protocol)
        {
            SetBegin(status, statusPhrase, protocol);
        }

        /// <summary>
        /// Is the HTTP response empty?
        /// </summary>
        public bool IsEmpty
        {
            get { return _cache.Size > 0; }
        }

        /// <summary>
        /// Is the HTTP response error flag set?
        /// </summary>
        public bool IsErrorSet { get; private set; }

        /// <summary>
        /// Get the HTTP response status
        /// </summary>
        public int Status { get; private set; }

        /// <summary>
        /// Get the HTTP response status phrase
        /// </summary>
        public string StatusPhrase
        {
            get { return _statusPhrase; }
        }

        /// <summary>
        /// Get the HTTP response protocol version
        /// </summary>
        public string Protocol
        {
            get { return _protocol; }
        }

        /// <summary>
        /// Get the HTTP response headers count
        /// </summary>
        public long Headers
        {
            get { return _headers.Count; }
        }

        /// <summary>
        /// Get the HTTP response header by index
        /// </summary>
        public (string, string) Header(int i)
        {
#if DEBUG
            Debug.Assert(i < _headers.Count, "Index out of bounds!");
#endif
            return i >= _headers.Count ? (string.Empty, string.Empty) : _headers[i];
        }

        #region Global CORS Handling

        private void SetCorsHeaders(string origin)
        {
            if (string.IsNullOrEmpty(origin) || allowedOrigins.Count == 0)
                // Allow requests with no Origin header (e.g., direct server-to-server requests) or if we not set any CORS rules.
                SetHeader("Access-Control-Allow-Origin", "*");
            else if (allowedOrigins.Contains(origin))
                // Allow requests with a valid Origin
                SetHeader("Access-Control-Allow-Origin", origin);
            else
            {
                // Not write any CORS header if denied.
                return;
            }

            SetHeader("Access-Control-Allow-Methods", allowedMethods);
            SetHeader("Access-Control-Allow-Headers", "*");
            SetHeader("Access-Control-Expose-Headers", string.Empty);
        }

        #endregion

        /// <summary>
        /// Get the HTTP response body as string
        /// </summary>
#if true // Serve as a HTTP json debugging.
        [JsonIgnore]
#endif
        public string Body
        {
            get { return _cache.ExtractString(_bodyIndex, _bodySize); }
        }

        /// <summary>
        /// Get the HTTP request body as byte array
        /// </summary>
        [JsonIgnore]
        public byte[] BodyBytes
        {
            get { return _cache.Data[_bodyIndex..(_bodyIndex + _bodySize)]; }
        }

        /// <summary>
        /// Get the HTTP request body as read-only byte span
        /// </summary>
        [JsonIgnore]
        public ReadOnlySpan<byte> BodySpan
        {
            get { return new ReadOnlySpan<byte>(_cache.Data, _bodyIndex, _bodySize); }
        }

        /// <summary>
        /// Get the HTTP response body length
        /// </summary>
        public long BodyLength
        {
            get { return _bodyLength; }
        }

        /// <summary>
        /// Get the HTTP response cache content
        /// </summary>
        [JsonIgnore]
        public Buffer Cache
        {
            get { return _cache; }
        }

        /// <summary>
        /// Get string from the current HTTP response
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Status: {Status}");
            sb.AppendLine($"Status phrase: {StatusPhrase}");
            sb.AppendLine($"Protocol: {Protocol}");
            sb.AppendLine($"Headers: {Headers}");
            for (var i = 0; i < Headers; i++)
            {
                var header = Header(i);
                sb.AppendLine($"{header.Item1} : {header.Item2}");
            }
            sb.AppendLine($"Body: {BodyLength}");
            sb.AppendLine(Body);
            return sb.ToString();
        }

        /// <summary>
        /// Clear the HTTP response cache
        /// </summary>
        public HttpResponse Clear()
        {
            IsErrorSet = false;
            Status = 0;
            _statusPhrase = string.Empty;
            _protocol = string.Empty;
            _headers.Clear();
            _bodyIndex = 0;
            _bodySize = 0;
            _bodyLength = 0;
            _bodyLengthProvided = false;

            _cache.Clear();
            _cacheSize = 0;
            return this;
        }

        /// <summary>
        /// Set the HTTP response begin with a given status and protocol
        /// </summary>
        /// <param name="status">HTTP status</param>
        /// <param name="protocol">Protocol version (default is "HTTP/1.1")</param>
        public HttpResponse SetBegin(int status, string protocol = "HTTP/1.1")
        {
            var statusPhrase = status switch
            {
                100 => "Continue",
                101 => "Switching Protocols",
                102 => "Processing",
                103 => "Early Hints",
                200 => "OK",
                201 => "Created",
                202 => "Accepted",
                203 => "Non-Authoritative Information",
                204 => "No Content",
                205 => "Reset Content",
                206 => "Partial Content",
                207 => "Multi-Status",
                208 => "Already Reported",
                226 => "IM Used",
                300 => "Multiple Choices",
                301 => "Moved Permanently",
                302 => "Found",
                303 => "See Other",
                304 => "Not Modified",
                305 => "Use Proxy",
                306 => "Switch Proxy",
                307 => "Temporary Redirect",
                308 => "Permanent Redirect",
                400 => "Bad Request",
                401 => "Unauthorized",
                402 => "Payment Required",
                403 => "Forbidden",
                404 => "Not Found",
                405 => "Method Not Allowed",
                406 => "Not Acceptable",
                407 => "Proxy Authentication Required",
                408 => "Request Timeout",
                409 => "Conflict",
                410 => "Gone",
                411 => "Length Required",
                412 => "Precondition Failed",
                413 => "Payload Too Large",
                414 => "URI Too Long",
                415 => "Unsupported Media Type",
                416 => "Range Not Satisfiable",
                417 => "Expectation Failed",
                421 => "Misdirected Request",
                422 => "Unprocessable Entity",
                423 => "Locked",
                424 => "Failed Dependency",
                425 => "Too Early",
                426 => "Upgrade Required",
                427 => "Unassigned",
                428 => "Precondition Required",
                429 => "Too Many Requests",
                431 => "Request Header Fields Too Large",
                451 => "Unavailable For Legal Reasons",
                500 => "Internal Server Error",
                501 => "Not Implemented",
                502 => "Bad Gateway",
                503 => "Service Unavailable",
                504 => "Gateway Timeout",
                505 => "HTTP Version Not Supported",
                506 => "Variant Also Negotiates",
                507 => "Insufficient Storage",
                508 => "Loop Detected",
                510 => "Not Extended",
                511 => "Network Authentication Required",
                _ => "Unknown",
            };
            SetBegin(status, statusPhrase, protocol);
            return this;
        }

        /// <summary>
        /// Set the HTTP response begin with a given status, status phrase and protocol
        /// </summary>
        /// <param name="status">HTTP status</param>
        /// <param name="statusPhrase"> HTTP status phrase</param>
        /// <param name="protocol">Protocol version</param>
        public HttpResponse SetBegin(int status, string statusPhrase, string protocol)
        {
            // Clear the HTTP response cache
            Clear();

            // Append the HTTP response protocol version
            _cache.Append(protocol);
            _protocol = protocol;

            _cache.Append(" ");

            // Append the HTTP response status
            _cache.Append(status.ToString());
            Status = status;

            _cache.Append(" ");

            // Append the HTTP response status phrase
            _cache.Append(statusPhrase);
            _statusPhrase = statusPhrase;

            _cache.Append("\r\n");
            return this;
        }

        /// <summary>
        /// Set the HTTP response content type
        /// </summary>
        /// <param name="extension">Content extension</param>
        public HttpResponse SetContentType(string extension)
        {
            // Try to lookup the content type in mime table
            return _mimeTable.TryGetValue(extension, out var mime)
                ? SetHeader("Content-Type", mime)
                : this;
        }

        /// <summary>
        /// Set the HTTP response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        public HttpResponse SetHeader(string key, string value)
        {
            // Append the HTTP response header's key
            _cache.Append(key);

            _cache.Append(": ");

            // Append the HTTP response header's value
            _cache.Append(value);

            _cache.Append("\r\n");

            // Add the header to the corresponding collection
            _headers.Add((key, value));
            return this;
        }

        /// <summary>
        /// Set the HTTP response cookie
        /// </summary>
        /// <param name="name">Cookie name</param>
        /// <param name="value">Cookie value</param>
        /// <param name="maxAge">Cookie age in seconds until it expires (default is 86400)</param>
        /// <param name="path">Cookie path (default is "")</param>
        /// <param name="domain">Cookie domain (default is "")</param>
        /// <param name="secure">Cookie secure flag (default is true)</param>
        /// <param name="strict">Cookie strict flag (default is true)</param>
        /// <param name="httpOnly">Cookie HTTP-only flag (default is true)</param>
        public HttpResponse SetCookie(
            string name,
            string value,
            int maxAge = 86400,
            string path = "",
            string domain = "",
            bool secure = true,
            bool strict = true,
            bool httpOnly = true
        )
        {
            var key = "Set-Cookie";

            // Append the HTTP response header's key
            _cache.Append(key);

            _cache.Append(": ");

            // Append the HTTP response header's value
            var valueIndex = (int)_cache.Size;

            // Append cookie
            _cache.Append(name);
            _cache.Append("=");
            _cache.Append(value);
            _cache.Append("; Max-Age=");
            _cache.Append(maxAge.ToString());
            if (!string.IsNullOrEmpty(domain))
            {
                _cache.Append("; Domain=");
                _cache.Append(domain);
            }
            if (!string.IsNullOrEmpty(path))
            {
                _cache.Append("; Path=");
                _cache.Append(path);
            }
            if (secure)
                _cache.Append("; Secure");
            if (strict)
                _cache.Append("; SameSite=Strict");
            if (httpOnly)
                _cache.Append("; HttpOnly");

            var valueSize = (int)_cache.Size - valueIndex;

            var cookie = _cache.ExtractString(valueIndex, valueSize);

            _cache.Append("\r\n");

            // Add the header to the corresponding collection
            _headers.Add((key, cookie));
            return this;
        }

        /// <summary>
        /// Set the HTTP response body
        /// </summary>
        /// <param name="body">Body string content (default is "")</param>
        public HttpResponse SetBody(
            string body = "",
            string encoding = null,
            string origin = null
        ) => SetBody(body.AsSpan(), encoding, origin);

        /// <summary>
        /// Set the HTTP response body
        /// </summary>
        /// <param name="body">Body string content as a span of characters</param>
        public HttpResponse SetBody(
            ReadOnlySpan<char> body,
            string encoding = null,
            string origin = null
        )
        {
            var length = body.IsEmpty ? 0 : Encoding.UTF8.GetByteCount(body);

            // Append CORS header
            SetCorsHeaders(origin);

            if (!string.IsNullOrEmpty(encoding) && length > 0)
            {
                var payload = Encoding.UTF8.GetBytes(body.ToString());
                if (!string.IsNullOrEmpty(encoding))
                {
                    if (encoding.Contains("zstd"))
                    {
                        SetHeader("Content-Encoding", "zstd");
                        payload = HTTPProcessor.CompressZstd(payload);
                    }
                    else if (encoding.Contains("br"))
                    {
                        SetHeader("Content-Encoding", "br");
                        payload = HTTPProcessor.CompressBrotli(payload);
                    }
                    else if (encoding.Contains("gzip"))
                    {
                        SetHeader("Content-Encoding", "gzip");
                        payload = HTTPProcessor.CompressGzip(payload);
                    }
                    else if (encoding.Contains("deflate"))
                    {
                        SetHeader("Content-Encoding", "deflate");
                        payload = HTTPProcessor.Deflate(payload);
                    }
                }

                length = payload.Length;

                // Append content length header
                SetHeader("Content-Length", length.ToString());

                _cache.Append("\r\n");

                var index = (int)_cache.Size;

                // Append the HTTP response body
                _cache.Append(payload);
                _bodyIndex = index;
                _bodySize = length;
                _bodyLength = length;
            }
            else
            {
                // Append content length header
                SetHeader("Content-Length", length.ToString());

                _cache.Append("\r\n");

                var index = (int)_cache.Size;

                // Append the HTTP response body
                _cache.Append(body);
                _bodyIndex = index;
                _bodySize = length;
                _bodyLength = length;
            }

            _bodyLengthProvided = true;
            return this;
        }

        /// <summary>
        /// Set the HTTP response body
        /// </summary>
        /// <param name="body">Body binary content</param>
        public HttpResponse SetBody(byte[] body, string encoding = null, string origin = null) =>
            SetBody(body.AsSpan(), encoding, origin);

        /// <summary>
        /// Set the HTTP response body
        /// </summary>
        /// <param name="body">Body binary content as a span of bytes</param>
        public HttpResponse SetBody(
            ReadOnlySpan<byte> body,
            string encoding = null,
            string origin = null
        )
        {
            var length = body.Length;

            // Append CORS header
            SetCorsHeaders(origin);

            if (!string.IsNullOrEmpty(encoding) && length > 0)
            {
                var payload = body.ToArray();
                if (!string.IsNullOrEmpty(encoding))
                {
                    if (encoding.Contains("zstd"))
                    {
                        SetHeader("Content-Encoding", "zstd");
                        payload = HTTPProcessor.CompressZstd(payload);
                    }
                    else if (encoding.Contains("br"))
                    {
                        SetHeader("Content-Encoding", "br");
                        payload = HTTPProcessor.CompressBrotli(payload);
                    }
                    else if (encoding.Contains("gzip"))
                    {
                        SetHeader("Content-Encoding", "gzip");
                        payload = HTTPProcessor.CompressGzip(payload);
                    }
                    else if (encoding.Contains("deflate"))
                    {
                        SetHeader("Content-Encoding", "deflate");
                        payload = HTTPProcessor.Deflate(payload);
                    }
                }

                length = payload.Length;

                // Append content length header
                SetHeader("Content-Length", length.ToString());

                _cache.Append("\r\n");

                var index = (int)_cache.Size;

                // Append the HTTP response body
                _cache.Append(payload);
                _bodyIndex = index;
                _bodySize = length;
                _bodyLength = length;
            }
            else
            {
                // Append content length header
                SetHeader("Content-Length", length.ToString());

                _cache.Append("\r\n");

                var index = (int)_cache.Size;

                // Append the HTTP response body
                _cache.Append(body);
                _bodyIndex = index;
                _bodySize = length;
                _bodyLength = length;
            }

            _bodyLengthProvided = true;
            return this;
        }

        /// <summary>
        /// Set the HTTP response body length
        /// </summary>
        /// <param name="length">Body length</param>
        public HttpResponse SetBodyLength(int length)
        {
            // Append content length header
            SetHeader("Content-Length", length.ToString());

            _cache.Append("\r\n");

            var index = (int)_cache.Size;

            // Clear the HTTP response body
            _bodyIndex = index;
            _bodySize = 0;
            _bodyLength = length;
            _bodyLengthProvided = true;
            return this;
        }

        /// <summary>
        /// Make OK response
        /// </summary>
        /// <param name="status">OK status (default is 200 (OK))</param>
        public HttpResponse MakeOkResponse(int status = 200)
        {
            Clear();
            SetBegin(status);
            SetBody();
            return this;
        }

        /// <summary>
        /// Make ERROR response
        /// </summary>
        /// <param name="content">Error content (default is "")</param>
        /// <param name="contentType">Error content type (default is "text/plain; charset=UTF-8")</param>
        public HttpResponse MakeErrorResponse(
            string content = "",
            string contentType = "text/plain; charset=UTF-8"
        )
        {
            return MakeErrorResponse((int)HttpStatusCode.InternalServerError, content, contentType);
        }

        /// <summary>
        /// Make ERROR response
        /// </summary>
        /// <param name="status">Error status</param>
        /// <param name="content">Error content (default is "")</param>
        /// <param name="contentType">Error content type (default is "text/plain; charset=UTF-8")</param>
        public HttpResponse MakeErrorResponse(
            int status,
            string content = "",
            string contentType = "text/plain; charset=UTF-8"
        )
        {
            Clear();
            SetBegin(status);
            if (!string.IsNullOrEmpty(contentType))
                SetHeader("Content-Type", contentType);
            SetBody(content);
            return this;
        }

        /// <summary>
        /// Make HEAD response
        /// </summary>
        public HttpResponse MakeHeadResponse()
        {
            Clear();
            SetBegin((int)HttpStatusCode.OK);
            SetBody();
            return this;
        }

        /// <summary>
        /// Make GET response
        /// </summary>
        /// <param name="content">String content (default is "")</param>
        /// <param name="contentType">Content type (default is "text/plain; charset=UTF-8")</param>
        public HttpResponse MakeGetResponse(
            string content = "",
            string contentType = "text/plain; charset=UTF-8"
        ) => MakeGetResponse(content.AsSpan(), contentType);

        /// <summary>
        /// Make GET response
        /// </summary>
        /// <param name="content">String content as a span of characters</param>
        /// <param name="contentType">Content type (default is "text/plain; charset=UTF-8")</param>
        public HttpResponse MakeGetResponse(
            ReadOnlySpan<char> content,
            string contentType = "text/plain; charset=UTF-8"
        )
        {
            Clear();
            SetBegin((int)HttpStatusCode.OK);
            if (!string.IsNullOrEmpty(contentType))
                SetHeader("Content-Type", contentType);
            SetBody(content);
            return this;
        }

        /// <summary>
        /// Make GET response
        /// </summary>
        /// <param name="content">Binary content</param>
        /// <param name="contentType">Content type (default is "")</param>
        public HttpResponse MakeGetResponse(byte[] content, string contentType = "") =>
            MakeGetResponse(content.AsSpan(), contentType);

        /// <summary>
        /// Make GET response
        /// </summary>
        /// <param name="content">Binary content as a span of bytes</param>
        /// <param name="contentType">Content type (default is "")</param>
        public HttpResponse MakeGetResponse(ReadOnlySpan<byte> content, string contentType = "")
        {
            Clear();
            SetBegin((int)HttpStatusCode.OK);
            if (!string.IsNullOrEmpty(contentType))
                SetHeader("Content-Type", contentType);
            SetBody(content);
            return this;
        }

        /// <summary>
        /// Make OPTIONS response
        /// </summary>
        /// <param name="allow">Allow methods (default is "HEAD,GET,POST,PUT,DELETE,OPTIONS,TRACE")</param>
        public HttpResponse MakeOptionsResponse(
            string allow = "HEAD,GET,POST,PUT,DELETE,OPTIONS,TRACE"
        )
        {
            Clear();
            SetBegin((int)HttpStatusCode.OK);
            SetHeader("Allow", allow);
            SetBody();
            return this;
        }

        /// <summary>
        /// Make TRACE response
        /// </summary>
        /// <param name="content">String content</param>
        public HttpResponse MakeTraceResponse(string content) =>
            MakeTraceResponse(content.AsSpan());

        /// <summary>
        /// Make TRACE response
        /// </summary>
        /// <param name="content">String content as a span of characters</param>
        public HttpResponse MakeTraceResponse(ReadOnlySpan<char> content)
        {
            Clear();
            SetBegin((int)HttpStatusCode.OK);
            SetHeader("Content-Type", "message/http");
            SetBody(content);
            return this;
        }

        /// <summary>
        /// Make TRACE response
        /// </summary>
        /// <param name="content">Binary content</param>
        public HttpResponse MakeTraceResponse(byte[] content) =>
            MakeTraceResponse(content.AsSpan());

        /// <summary>
        /// Make TRACE response
        /// </summary>
        /// <param name="content">Binary content as a span of bytes</param>
        public HttpResponse MakeTraceResponse(ReadOnlySpan<byte> content)
        {
            Clear();
            SetBegin((int)HttpStatusCode.OK);
            SetHeader("Content-Type", "message/http");
            SetBody(content);
            return this;
        }

        /// <summary>
        /// Make TRACE response
        /// </summary>
        /// <param name="request">HTTP request</param>
        public HttpResponse MakeTraceResponse(HttpRequest request) =>
            MakeTraceResponse(request.Cache.AsSpan());

        // HTTP response status phrase
        private string _statusPhrase;

        // HTTP response protocol
        private string _protocol;

        // HTTP response headers
        private readonly List<(string, string)> _headers = [];

        // HTTP response body
        private int _bodyIndex;
        private int _bodySize;
        private int _bodyLength;
        private bool _bodyLengthProvided;

        // HTTP response cache
        private readonly Buffer _cache = new();
        private int _cacheSize;

        // HTTP response mime table
        private static readonly Dictionary<string, string> _mimeTable;

        // Is pending parts of HTTP response
        internal bool IsPendingHeader()
        {
            return !IsErrorSet && (_bodyIndex == 0);
        }

        internal bool IsPendingBody()
        {
            return !IsErrorSet && (_bodyIndex > 0) && (_bodySize > 0);
        }

        // Receive parts of HTTP response
        internal bool ReceiveHeader(byte[] buffer, int offset, int size)
        {
            // Update the request cache
            _cache.Append(buffer, offset, size);

            // Try to seek for HTTP header separator
            for (var i = _cacheSize; i < (int)_cache.Size; i++)
            {
                // Check for the request cache out of bounds
                if ((i + 3) >= (int)_cache.Size)
                    break;

                // Check for the header separator
                if (
                    (_cache[i + 0] == '\r')
                    && (_cache[i + 1] == '\n')
                    && (_cache[i + 2] == '\r')
                    && (_cache[i + 3] == '\n')
                )
                {
                    var index = 0;

                    // Set the error flag for a while...
                    IsErrorSet = true;

                    // Parse protocol version
                    var protocolIndex = index;
                    var protocolSize = 0;
                    while (_cache[index] != ' ')
                    {
                        protocolSize++;
                        index++;
                        if (index >= (int)_cache.Size)
                            return false;
                    }
                    index++;
                    if (index >= (int)_cache.Size)
                        return false;
                    _protocol = _cache.ExtractString(protocolIndex, protocolSize);

                    // Parse status code
                    var statusIndex = index;
                    var statusSize = 0;
                    while (_cache[index] != ' ')
                    {
                        if ((_cache[index] < '0') || (_cache[index] > '9'))
                            return false;
                        statusSize++;
                        index++;
                        if (index >= (int)_cache.Size)
                            return false;
                    }
                    Status = 0;
                    for (var j = statusIndex; j < (statusIndex + statusSize); j++)
                    {
                        Status *= 10;
                        Status += _cache[j] - '0';
                    }
                    index++;
                    if (index >= (int)_cache.Size)
                        return false;

                    // Parse status phrase
                    var statusPhraseIndex = index;
                    var statusPhraseSize = 0;
                    while (_cache[index] != '\r')
                    {
                        statusPhraseSize++;
                        index++;
                        if (index >= (int)_cache.Size)
                            return false;
                    }
                    index++;
                    if ((index >= (int)_cache.Size) || (_cache[index] != '\n'))
                        return false;
                    index++;
                    if (index >= (int)_cache.Size)
                        return false;
                    _statusPhrase = _cache.ExtractString(statusPhraseIndex, statusPhraseSize);

                    // Parse headers
                    while ((index < (int)_cache.Size) && (index < i))
                    {
                        // Parse header name
                        var headerNameIndex = index;
                        var headerNameSize = 0;
                        while (_cache[index] != ':')
                        {
                            headerNameSize++;
                            index++;
                            if (index >= i)
                                break;
                            if (index >= (int)_cache.Size)
                                return false;
                        }
                        index++;
                        if (index >= i)
                            break;
                        if (index >= (int)_cache.Size)
                            return false;

                        // Skip all prefix space characters
                        while (char.IsWhiteSpace((char)_cache[index]))
                        {
                            index++;
                            if (index >= i)
                                break;
                            if (index >= (int)_cache.Size)
                                return false;
                        }

                        // Parse header value
                        var headerValueIndex = index;
                        var headerValueSize = 0;
                        while (_cache[index] != '\r')
                        {
                            headerValueSize++;
                            index++;
                            if (index >= i)
                                break;
                            if (index >= (int)_cache.Size)
                                return false;
                        }
                        index++;
                        if ((index >= (int)_cache.Size) || (_cache[index] != '\n'))
                            return false;
                        index++;
                        if (index >= (int)_cache.Size)
                            return false;

                        // Validate header name and value (sometimes value can be empty)
                        if (headerNameSize == 0)
                            return false;

                        // Add a new header
                        var headerName = _cache.ExtractString(headerNameIndex, headerNameSize);
                        _headers.Add(
                            (headerName, _cache.ExtractString(headerValueIndex, headerValueSize))
                        );

                        // Try to find the body content length
                        if (
                            string.Compare(
                                headerName,
                                "Content-Length",
                                StringComparison.OrdinalIgnoreCase
                            ) == 0
                        )
                        {
                            _bodyLength = 0;
                            for (
                                var j = headerValueIndex;
                                j < (headerValueIndex + headerValueSize);
                                j++
                            )
                            {
                                if ((_cache[j] < '0') || (_cache[j] > '9'))
                                    return false;
                                _bodyLength *= 10;
                                _bodyLength += _cache[j] - '0';
                                _bodyLengthProvided = true;
                            }
                        }
                    }

                    // Reset the error flag
                    IsErrorSet = false;

                    // Update the body index and size
                    _bodyIndex = i + 4;
                    _bodySize = (int)_cache.Size - i - 4;

                    // Update the parsed cache size
                    _cacheSize = (int)_cache.Size;

                    return true;
                }
            }

            // Update the parsed cache size
            _cacheSize = ((int)_cache.Size >= 3) ? ((int)_cache.Size - 3) : 0;

            return false;
        }

        internal bool ReceiveBody(byte[] buffer, int offset, int size)
        {
            // Update the request cache
            _cache.Append(buffer, offset, size);

            // Update the parsed cache size
            _cacheSize = (int)_cache.Size;

            // Update body size
            _bodySize += size;

            // Check if the body length was provided
            if (_bodyLengthProvided)
            {
                // Was the body fully received?
                if (_bodySize >= _bodyLength)
                {
                    _bodySize = _bodyLength;
                    return true;
                }
            }
            else
            {
                // Check the body content to find the response body end
                if (_bodySize >= 4)
                {
                    var index = _bodyIndex + _bodySize - 4;

                    // Was the body fully received?
                    if (
                        (_cache[index + 0] == '\r')
                        && (_cache[index + 1] == '\n')
                        && (_cache[index + 2] == '\r')
                        && (_cache[index + 3] == '\n')
                    )
                    {
                        _bodyLength = _bodySize;
                        return true;
                    }
                }
            }

            // Body was received partially...
            return false;
        }
    }
}
