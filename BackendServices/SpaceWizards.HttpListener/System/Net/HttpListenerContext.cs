// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Net.WebSockets;
using System.Security.Principal;
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
                _response ??= new HttpListenerResponse(this);

                return _response;
            }
        }

        public Task<WebSockets.HttpListenerWebSocketContext> AcceptWebSocketAsync(
            string subProtocol
        )
        {
            return AcceptWebSocketAsync(
                subProtocol,
                HttpWebSocket.DefaultReceiveBufferSize,
                WebSocket.DefaultKeepAliveInterval
            );
        }

        public Task<WebSockets.HttpListenerWebSocketContext> AcceptWebSocketAsync(
            string subProtocol,
            TimeSpan keepAliveInterval
        )
        {
            return AcceptWebSocketAsync(
                subProtocol,
                HttpWebSocket.DefaultReceiveBufferSize,
                keepAliveInterval
            );
        }
    }
}
