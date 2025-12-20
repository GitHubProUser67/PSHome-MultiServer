// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SpaceWizards.HttpListener
{
    public partial class HttpListener
    {
        internal X509Certificate2 LoadRootCACertificate(IPAddress addr, int port)
        {
            lock (_internalLock)
            {
                // Actually load the certificate
                if (_certificateCache != null && _certificateCache.TryGetValue((addr, port), out X509Certificate2 certificate))
                    return certificate;
            }

            return null;
        }
    }
}
