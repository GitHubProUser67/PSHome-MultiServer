// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Threading.Tasks;
using SpaceWizards.HttpListener.WebSockets;

namespace SpaceWizards.HttpListener
{
    public sealed unsafe partial class HttpListenerContext
    {
        internal HttpListener _listener;
        private HttpListenerResponse _response;
        private IPrincipal _user;

        public HttpListenerRequest Request { get; }

        public IPrincipal User => _user;

        // This can be used to cache the results of HttpListener.AuthenticationSchemeSelectorDelegate.
        internal AuthenticationSchemes AuthenticationSchemes { get; set; }

        public HttpListenerResponse Response
        {
            get
            {
                if (_response == null)
                {
                    _response = new HttpListenerResponse(this);
                }

                return _response;
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public Task<WebSockets.HttpListenerWebSocketContext> AcceptWebSocketAsync(string subProtocol)
        {
            return AcceptWebSocketAsync(subProtocol, HttpWebSocket.DefaultReceiveBufferSize, WebSocket.DefaultKeepAliveInterval);
        }

        public Task<WebSockets.HttpListenerWebSocketContext> AcceptWebSocketAsync(string subProtocol, TimeSpan keepAliveInterval)
        {
            return AcceptWebSocketAsync(subProtocol, HttpWebSocket.DefaultReceiveBufferSize, keepAliveInterval);
        }
#endif
    }
}
