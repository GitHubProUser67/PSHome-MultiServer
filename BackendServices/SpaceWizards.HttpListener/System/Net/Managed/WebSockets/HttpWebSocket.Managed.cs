// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.Net;
using System.Net.WebSockets;

namespace SpaceWizards.HttpListener.WebSockets
{
    internal static partial class HttpWebSocket
    {
        private const string SupportedVersion = "13";

        internal static async Task<HttpListenerWebSocketContext> AcceptWebSocketAsyncCore(
            HttpListenerContext context,
            string subProtocol,
            int receiveBufferSize,
            TimeSpan keepAliveInterval,
            ArraySegment<byte>? internalBuffer = null
        )
        {
            ValidateOptions(subProtocol, receiveBufferSize, MinSendBufferSize, keepAliveInterval);

            // get property will create a new response if one doesn't exist.
            var response = context.Response;
            var request = context.Request;
            ValidateWebSocketHeaders(context);

            var secWebSocketVersion = request.Headers[HttpKnownHeaderNames.SecWebSocketVersion];

            // Optional for non-browser client
            var origin = request.Headers[HttpKnownHeaderNames.Origin];

            string[] secWebSocketProtocols = null;
            string outgoingSecWebSocketProtocolString;
            var shouldSendSecWebSocketProtocolHeader = ProcessWebSocketProtocolHeader(
                request.Headers[HttpKnownHeaderNames.SecWebSocketProtocol],
                subProtocol,
                out outgoingSecWebSocketProtocolString
            );

            if (shouldSendSecWebSocketProtocolHeader)
            {
                secWebSocketProtocols = new string[] { outgoingSecWebSocketProtocolString };
                response.Headers.Add(
                    HttpKnownHeaderNames.SecWebSocketProtocol,
                    outgoingSecWebSocketProtocolString
                );
            }

            // negotiate the websocket key return value
            var secWebSocketKey = request.Headers[HttpKnownHeaderNames.SecWebSocketKey];
            var secWebSocketAccept = GetSecWebSocketAcceptString(secWebSocketKey);

            response.Headers.Add(HttpKnownHeaderNames.Connection, HttpKnownHeaderNames.Upgrade);
            response.Headers.Add(HttpKnownHeaderNames.Upgrade, WebSocketUpgradeToken);
            response.Headers.Add(HttpKnownHeaderNames.SecWebSocketAccept, secWebSocketAccept);

            response.StatusCode = (int)HttpStatusCode.SwitchingProtocols; // HTTP 101
            response.StatusDescription = HttpStatusDescription.Get(
                HttpStatusCode.SwitchingProtocols
            );

            var responseStream = response.OutputStream as HttpResponseStream;

            // Send websocket handshake headers
            await responseStream.WriteWebSocketHandshakeHeadersAsync().ConfigureAwait(false);

            var webSocket = WebSocket.CreateFromStream(
                context.Connection.ConnectedStream,
                isServer: true,
                subProtocol,
                keepAliveInterval
            );

            var webSocketContext = new HttpListenerWebSocketContext(
                request.Url,
                request.Headers,
                request.Cookies,
                context.User,
                HttpListenerRequest.IsAuthenticated,
                request.IsLocal,
                request.IsSecureConnection,
                origin,
                secWebSocketProtocols ?? Array.Empty<string>(),
                secWebSocketVersion,
                secWebSocketKey,
                webSocket
            );

            return webSocketContext;
        }

        private const bool WebSocketsSupported = true;
    }
}
