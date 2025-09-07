// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SpaceWizards.HttpListener
{
    public partial class HttpListener
    {
        internal X509Certificate2 LoadRootCACertificate(IPAddress addr, int port)
        {
            X509Certificate2 certificate;

            lock (_internalLock)
            {
                // Actually load the certificate
                if (_certificateCache != null && _certificateCache.TryGetValue((addr, port), out certificate))
                {
                    return certificate;
                }
            }

            return null;
        }
    }
}
