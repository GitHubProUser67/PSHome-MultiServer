// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// System.Net.HttpEndPointManager
//
// Author:
//  Gonzalo Paniagua Javier (gonzalo@ximian.com)
//
// Copyright (c) 2005 Novell, Inc. (http://www.novell.com)
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace SpaceWizards.HttpListener
{
    internal sealed class HttpEndPointManager
    {
        private static Dictionary<IPAddress, Dictionary<int, HttpEndPointListener>> s_ipEndPoints = new Dictionary<IPAddress, Dictionary<int, HttpEndPointListener>>();

        private HttpEndPointManager()
        {
        }

        public static void AddListener(HttpListener listener)
        {
            List<string> added = new List<string>();
            try
            {
                lock ((s_ipEndPoints as ICollection).SyncRoot)
                {
                    foreach (string prefix in listener.Prefixes)
                    {
                        AddPrefixInternal(prefix, listener);
                        added.Add(prefix);
                    }
                }
            }
            catch
            {
                foreach (string prefix in added)
                {
                    RemovePrefix(prefix, listener);
                }
                throw;
            }
        }

        public static void AddPrefix(string prefix, HttpListener listener)
        {
            lock ((s_ipEndPoints as ICollection).SyncRoot)
            {
                AddPrefixInternal(prefix, listener);
            }
        }

        private static void AddPrefixInternal(string p, HttpListener listener)
        {
            ListenerPrefix lp = new ListenerPrefix(p);
            if (lp.Host != "*" && lp.Host != "+" && Uri.CheckHostName(lp.Host) == UriHostNameType.Unknown)
                throw new HttpListenerException((int)HttpStatusCode.BadRequest, SR.net_listener_host);

            if (lp.Port <= 0 || lp.Port >= 65536)
                throw new HttpListenerException((int)HttpStatusCode.BadRequest, SR.net_invalid_port);

            if (lp.Path.Contains('%'))
                throw new HttpListenerException((int)HttpStatusCode.BadRequest, SR.net_invalid_path);

            if (lp.Path.IndexOf("//", StringComparison.Ordinal) != -1)
                throw new HttpListenerException((int)HttpStatusCode.BadRequest, SR.net_invalid_path);

            // listens on all the interfaces if host name cannot be parsed by IPAddress.
            foreach (HttpEndPointListener epl in GetEPListener(lp.Host, lp.Port, listener, lp.Secure))
            {
                epl.AddPrefix(lp, listener);
            }
        }

        private static IEnumerable<HttpEndPointListener> GetEPListener(string host, int port, HttpListener listener, bool secure)
        {
            IPAddress[] addresses;
            if (host == "*" || host == "+")
            {
                addresses = new []{IPAddress.Any, IPAddress.IPv6Any};
            }
            else
            {
                if (host.StartsWith("[") && host.EndsWith("]"))
                    host = host.Substring(1, host.Length - 2);

                const int NotSupportedErrorCode = 50;
                try
                {
                    addresses = Dns.GetHostAddresses(host);
                }
                catch
                {
                    // Throw same error code as windows, request is not supported.
                    throw new HttpListenerException(NotSupportedErrorCode, SR.net_listener_not_supported);
                }

                if (addresses.Any(a => a.Equals(IPAddress.IPv6Any) || a.Equals(IPAddress.Any)))
                {
                    // Don't support listening to 0.0.0.0, match windows behavior.
                    throw new HttpListenerException(NotSupportedErrorCode, SR.net_listener_not_supported);
                }
            }

            foreach (var addr in addresses)
            {
                Dictionary<int, HttpEndPointListener> p = null;
                if (s_ipEndPoints.ContainsKey(addr))
                {
                    p = s_ipEndPoints[addr];
                }
                else
                {
                    p = new Dictionary<int, HttpEndPointListener>();
                    s_ipEndPoints[addr] = p;
                }

                HttpEndPointListener epl = null;
                if (p.ContainsKey(port))
                {
                    epl = p[port];
                }
                else
                {
                    try
                    {
                        epl = new HttpEndPointListener(listener, addr, port, secure);
                    }
                    catch (SocketException ex)
                    {
                        throw new HttpListenerException(ex.ErrorCode, ex.Message);
                    }
                    p[port] = epl;
                }

                yield return epl;
            }
        }

        public static void RemoveEndPoint(HttpEndPointListener epl, IPEndPoint ep)
        {
            lock ((s_ipEndPoints as ICollection).SyncRoot)
            {
                Dictionary<int, HttpEndPointListener> p = null;
                p = s_ipEndPoints[ep.Address];
                p.Remove(ep.Port);
                if (p.Count == 0)
                {
                    s_ipEndPoints.Remove(ep.Address);
                }
                epl.Close();
            }
        }

        public static void RemoveListener(HttpListener listener)
        {
            lock ((s_ipEndPoints as ICollection).SyncRoot)
            {
                foreach (string prefix in listener.Prefixes)
                {
                    RemovePrefixInternal(prefix, listener);
                }
            }
        }

        public static void RemovePrefix(string prefix, HttpListener listener)
        {
            lock ((s_ipEndPoints as ICollection).SyncRoot)
            {
                RemovePrefixInternal(prefix, listener);
            }
        }

        private static void RemovePrefixInternal(string prefix, HttpListener listener)
        {
            ListenerPrefix lp = new ListenerPrefix(prefix);
            if (lp.Path.Contains('%'))
                return;

            if (lp.Path.IndexOf("//", StringComparison.Ordinal) != -1)
                return;

            foreach (HttpEndPointListener epl in GetEPListener(lp.Host, lp.Port, listener, lp.Secure))
            {
                epl.RemovePrefix(lp, listener);
            }
        }
    }
}
