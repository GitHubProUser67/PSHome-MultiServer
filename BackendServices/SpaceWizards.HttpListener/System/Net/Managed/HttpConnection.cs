// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// System.Net.HttpConnection
//
// Author:
//  Gonzalo Paniagua Javier (gonzalo.mono@gmail.com)
//
// Copyright (c) 2005-2009 Novell, Inc. (http://www.novell.com)
// Copyright (c) 2012 Xamarin, Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using CustomLogger;
using FixedSsl;
using MultiServerLibrary.SSL;
using Org.Mentalis.Security.Ssl;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceWizards.HttpListener
{
    internal sealed class HttpConnection
    {
        private readonly ManualResetEventSlim _initializedEvent = new(false);
        private static readonly Action<Task<int>, object> s_onreadCallback = OnRead;
        private const int BufferSize = 8192;
        private Socket _socket;
        private Stream _stream;
        private HttpEndPointListener _epl;
        private MemoryStream _memoryStream;
        private byte[] _buffer;
        private HttpListenerContext _context;
        private StringBuilder _currentLine;
        private ListenerPrefix _prefix;
        private HttpRequestStream _requestStream;
        private HttpResponseStream _responseStream;
        private bool _chunked;
        private int _reuses;
        private bool _contextBound;
        private bool _secure;
        private X509Certificate2 _cert;
        private int _timeout = 90000; // 90k ms for first request, 15k ms from then on
        private Timer _timer;
        private IPEndPoint _localEndPoint;
        private HttpListener _lastListener;
        private int[] _clientCertErrors;
        private X509Certificate2 _clientCert;
        private string _sniDomain;
        private InputState _inputState = InputState.RequestLine;
        private LineState _lineState = LineState.None;
        private int _position;

        public HttpConnection(Socket sock, HttpEndPointListener epl, bool secure, X509Certificate2 cert)
        {
            _socket = sock;
            _epl = epl;
            _secure = secure;
            _cert = cert;
            _timer = new Timer(OnTimeout, null, Timeout.Infinite, Timeout.Infinite);
            SslSocket.BeginAuthenticateAsServer(sock, secure ? new SslServerAuthenticationOptions
            {
                ClientCertificateRequired = false,
                EnabledSslProtocols = epl.Listener.SslProtocols,
                CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
                ServerCertificateSelectionCallback = (sender, actualHostName) =>
                {
                    IPEndPoint localEndpoint = (IPEndPoint)sock.LocalEndPoint;

                    if (string.IsNullOrEmpty(actualHostName))
                        _sniDomain = localEndpoint.Address.ToString() ?? "127.0.0.1";
                    else
                        _sniDomain = actualHostName;
#if NET5_0_OR_GREATER
                    // Actually load the certificate
                    try
                    {
                        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                            , ".mono");
                        path = Path.Combine(path, "httplistener");
                        string cert_prefix = _sniDomain + $"-{localEndpoint.Port}";
                        string cert_file = Path.Combine(path, string.Format("{0}.pem", cert_prefix));
                        string pvk_file = Path.Combine(path, string.Format("{0}_privkey.pem", cert_prefix));
                        if (File.Exists(cert_file) && File.Exists(pvk_file))
                            return CertificateHelper.LoadCertificate(cert_file, pvk_file);
                        cert_file = Path.Combine(path, string.Format("{0}.cer", cert_prefix));
                        pvk_file = Path.Combine(path, string.Format("{0}.pvk", cert_prefix));
                        if (File.Exists(cert_file) && File.Exists(pvk_file))
                            return CertificateHelper.LoadCertificate(cert_file, pvk_file);
                        string origin_directory = Path.Combine(path, cert_prefix);
                        if (Directory.Exists(origin_directory))
                        {
                            cert_file = Path.Combine(origin_directory, "Origin Certificate");
                            pvk_file = Path.Combine(origin_directory, "Private Key");
                            if (File.Exists(cert_file) && File.Exists(pvk_file))
                                return CertificateHelper.LoadCertificate(cert_file, pvk_file);
                        }
                    }
                    catch
                    {
                        // ignore errors
                    }
#endif
                    return CertificateHelper.IsCertificateAuthority(_cert) ? CertificateHelper.MakeChainSignedCert(_sniDomain, _cert, epl.Listener.GetPreferedHashAlgorithm(),
                    ((IPEndPoint)sock.RemoteEndPoint).Address ?? IPAddress.Any, DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(7),
                    epl.Listener.wildcardCertificates) : _cert;
                }
            } : null, false, false, AuthenticateAsServerCallback, null, out _clientCert, out _clientCertErrors);
        }

        public void AuthenticateAsServerCallback(IAsyncResult result)
        {
            try
            {
                _stream = SslSocket.EndAuthenticateAsServer(result);

                if (_stream == null)
                    LoggerAccessor.LogWarn($"[HttpConnection] - Failed to authenticate as server.");
            }
            catch (Exception ex)
            {
                _stream = null;

                LoggerAccessor.LogError($"[HttpConnection] - Errored out while trying to authenticate as server. (Exception: {ex}).");
            }
            finally
            {
                if (_stream != null)
                    Init();

                _initializedEvent.Set();
            }
        }

        public void WaitForAuthentication()
        {
            _initializedEvent.Wait();
        }

        internal int[] ClientCertificateErrors
        {
            get { return _clientCertErrors; }
        }

        internal X509Certificate2 ClientCertificate
        {
            get { return _clientCert; }
        }

        internal Stream SslStream
        {
            get 
            {
                if (_stream is SslStream sslStream)
                    return sslStream;
                else if (_stream is SecureNetworkStream sslStream1)
                    return sslStream1;
                else
                    throw new Exception($"[HttpConnection] - Connection stream can only be of types:{typeof(SslStream)}|{typeof(SecureNetworkStream)}.");
            }
        }
#if NET5_0_OR_GREATER
        [MemberNotNull(nameof(_memoryStream))]
        [MemberNotNull(nameof(_context))]
#endif
        private void Init()
        {
            _contextBound = false;
            _requestStream = null;
            _responseStream = null;
            _prefix = null;
            _chunked = false;
            _memoryStream = new MemoryStream();
            _position = 0;
            _inputState = InputState.RequestLine;
            _lineState = LineState.None;
            _context = new HttpListenerContext(this);
        }

        public Stream ConnectedStream => _stream;

        public bool IsClosed
        {
            get { return (_socket == null); }
        }

        public int Reuses
        {
            get { return _reuses; }
        }

        public IPEndPoint LocalEndPoint
        {
            get
            {
                if (_localEndPoint != null)
                    return _localEndPoint;

                _localEndPoint = (IPEndPoint)_socket.LocalEndPoint;
                return _localEndPoint;
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return (IPEndPoint)_socket.RemoteEndPoint; }
        }

        public bool IsSecure
        {
            get { return _secure; }
        }

        public ListenerPrefix Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        private void OnTimeout(object unused)
        {
            CloseSocket();
            Unbind();
        }

        public void BeginReadRequest()
        {
            if (_buffer == null)
                _buffer = new byte[BufferSize];
            try
            {
                if (_reuses == 1)
                    _timeout = 15000;
                _timer.Change(_timeout, Timeout.Infinite);
                _stream.ReadAsync(_buffer, 0, BufferSize).ContinueWith(s_onreadCallback, this);
            }
            catch
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                CloseSocket();
                Unbind();
            }
        }

        public HttpRequestStream GetRequestStream(bool chunked, long contentlength)
        {
            if (_requestStream == null)
            {
                byte[] buffer = _memoryStream.GetBuffer();
                int length = (int)_memoryStream.Length;
                _memoryStream = null;
                if (chunked)
                {
                    _chunked = true;
                    _context.Response.SendChunked = true;
                    _requestStream = new ChunkedInputStream(_context, _stream, buffer, _position, length - _position);
                }
                else
                {
                    _requestStream = new HttpRequestStream(_stream, buffer, _position, length - _position, contentlength);
                }
            }
            return _requestStream;
        }

        public HttpResponseStream GetResponseStream()
        {
            if (_responseStream == null)
            {
                HttpListener listener = _context._listener;

                if (listener == null)
                    return new HttpResponseStream(_stream, _context.Response, true);

                _responseStream = new HttpResponseStream(_stream, _context.Response, listener.IgnoreWriteExceptions);
            }
            return _responseStream;
        }

        private static void OnRead(Task<int> task, object state)
        {
            HttpConnection cnc = (HttpConnection)state;
            cnc.OnReadInternal(task);
        }

        private void OnReadInternal(Task<int> ares)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            int nread = -1;
            try
            {
                nread = ares.Result;
                _memoryStream.Write(_buffer, 0, nread);
                if (_memoryStream.Length > 32768)
                {
                    SendError(HttpStatusDescription.Get(400), 400);
                    Close(true);
                    return;
                }
            }
            catch
            {
                if (_memoryStream != null && _memoryStream.Length > 0)
                    SendError();
                if (_socket != null)
                {
                    CloseSocket();
                    Unbind();
                }
                return;
            }

            if (nread == 0)
            {
                CloseSocket();
                Unbind();
                return;
            }

            if (ProcessInput(_memoryStream))
            {
                if (!_context.HaveError)
                    _context.Request.FinishInitialization();

                if (_context.HaveError)
                {
                    SendError();
                    Close(true);
                    return;
                }

                if (!_epl.BindContext(_context))
                {
                    const int NotFoundErrorCode = 404;
                    SendError(HttpStatusDescription.Get(NotFoundErrorCode), NotFoundErrorCode);
                    Close(true);
                    return;
                }
                HttpListener listener = _context._listener;
                if (_lastListener != listener)
                {
                    RemoveConnection();
                    listener.AddConnection(this);
                    _lastListener = listener;
                }

                _contextBound = true;
                listener.RegisterContext(_context);
                return;
            }

            _stream.ReadAsync(_buffer, 0, BufferSize).ContinueWith(s_onreadCallback, this);
        }

        private void RemoveConnection()
        {
            if (_lastListener == null)
                _epl.RemoveConnection(this);
            else
                _lastListener.RemoveConnection(this);
        }

        private enum InputState
        {
            RequestLine,
            Headers
        }

        private enum LineState
        {
            None,
            CR,
            LF
        }

        // true -> done processing
        // false -> need more input
        private bool ProcessInput(MemoryStream ms)
        {
            byte[] buffer = ms.GetBuffer();
            int len = (int)ms.Length;
            int used = 0;
            string line;

            while (true)
            {
                if (_context.HaveError)
                    return true;

                if (_position >= len)
                    break;

                try
                {
                    line = ReadLine(buffer, _position, len - _position, ref used);
                    _position += used;
                }
                catch
                {
                    _context.ErrorMessage = HttpStatusDescription.Get(400);
                    _context.ErrorStatus = 400;
                    return true;
                }

                if (line == null)
                    break;

                if (line == string.Empty)
                {
                    if (_inputState == InputState.RequestLine)
                        continue;
                    _currentLine = null;
                    ms = null;
                    return true;
                }

                if (_inputState == InputState.RequestLine)
                {
                    _context.Request.SetRequestLine(line);
                    _inputState = InputState.Headers;
                }
                else
                {
                    try
                    {
                        _context.Request.AddHeader(line);
                    }
                    catch (Exception e)
                    {
                        _context.ErrorMessage = e.Message;
                        _context.ErrorStatus = 400;
                        return true;
                    }
                }
            }

            if (used == len)
            {
                ms.SetLength(0);
                _position = 0;
            }
            return false;
        }

        private string ReadLine(byte[] buffer, int offset, int len, ref int used)
        {
            if (_currentLine == null)
                _currentLine = new StringBuilder(128);
            int last = offset + len;
            used = 0;
            for (int i = offset; i < last && _lineState != LineState.LF; i++)
            {
                used++;
                byte b = buffer[i];
                if (b == 13)
                {
                    _lineState = LineState.CR;
                }
                else if (b == 10)
                {
                    _lineState = LineState.LF;
                }
                else
                {
                    _currentLine.Append((char)b);
                }
            }

            string result = null;
            if (_lineState == LineState.LF)
            {
                _lineState = LineState.None;
                result = _currentLine.ToString();
                _currentLine.Length = 0;
            }

            return result;
        }

        public void SendError(string msg, int status)
        {
            try
            {
                HttpListenerResponse response = _context.Response;
                response.StatusCode = status;
                response.ContentType = "text/html";
                string description = HttpStatusDescription.Get(status);
                string str = !string.IsNullOrEmpty(msg) ?
                    $"<h1>{description} ({msg})</h1>" :
                    $"<h1>{description}</h1>";

                byte[] error = Encoding.Default.GetBytes(str);
                response.Close(error, false);
            }
            catch
            {
                // response was already closed
            }
        }

        public void SendError()
        {
            SendError(_context.ErrorMessage, _context.ErrorStatus);
        }

        private void Unbind()
        {
            if (_contextBound)
            {
                _epl.UnbindContext(_context);
                _contextBound = false;
            }
        }

        public void Close()
        {
            Close(false);
        }

        private void CloseSocket()
        {
            if (_socket == null)
                return;

            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            catch { }
            finally
            {
                _socket = null;
            }

            RemoveConnection();
        }

        internal void Close(bool force)
        {
            if (_socket != null)
            {
                Stream st = GetResponseStream();
                if (st != null)
                    st.Close();

                _responseStream = null;
            }

            if (_socket != null)
            {
                force |= !_context.Request.KeepAlive;
                if (!force)
                    force = (_context.Response.Headers[HttpKnownHeaderNames.Connection] == HttpHeaderStrings.Close);

                if (!force && _context.Request.FlushInput())
                {
                    if (_chunked && _context.Response.ForceCloseChunked == false)
                    {
                        // Don't close. Keep working.
                        _reuses++;
                        Unbind();
                        Init();
                        BeginReadRequest();
                        return;
                    }

                    _reuses++;
                    Unbind();
                    Init();
                    BeginReadRequest();
                    return;
                }

                Socket s = _socket;
                _socket = null;
                try
                {
                    if (s != null)
                        s.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                finally
                {
                    if (s != null)
                        s.Close();
                }
                Unbind();
                RemoveConnection();
                return;
            }
        }
    }
}
