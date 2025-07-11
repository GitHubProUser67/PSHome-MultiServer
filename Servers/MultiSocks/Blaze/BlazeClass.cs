using Blaze3SDK;
using Blaze3SDK.Components;
using BlazeCommon;
using CustomLogger;
using MultiSocks.Blaze.Redirector;
using MultiSocks.ProtoSSL;
using MultiServerLibrary.Extension;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography;
using WebAPIService.WebCrypto;

namespace MultiSocks.Blaze
{
    public class BlazeClass : IDisposable
    {
        private bool disposedValue;

        private BlazeServer redirector;

        private BlazeServer? MassEffect2PS3mainBlaze;
        private BlazeServer? MassEffect3PS3mainBlaze;
        private BlazeServer? SsxmainBlaze;
        private BlazeServer? NFSHotPursuitmainBlaze;
        private BlazeServer? Fifa12mainBlaze;
        private BlazeServer? Crysis3mainBlaze;
        private BlazeServer? DeadSpace3mainBlaze;
        private BlazeServer? PVZGWmainBlaze;

        private VulnerableCertificateGenerator? SSLCache = new();

        public BlazeClass(CancellationToken cancellationToken)
        {
            const string sslDomain = "gosredirector.ea.com";

            // Create Blaze Redirector server

            redirector = Blaze3.CreateBlazeServer("Blaze3 Redirector", new IPEndPoint(IPAddress.Any, 42127), SSLCache.GetVulnerableCustomEaCert(sslDomain, "Global Online Studio", true, true).Item3);

            redirector.AddComponent<RedirectorComponent>();

            _ = StartRedirectorServers();

            // Create Main Blaze servers

            MassEffect2PS3mainBlaze = Blaze3.CreateBlazeServer("Mass Effect 2 (PS3)", new IPEndPoint(IPAddress.Any, 33153), SSLCache.GetVulnerableCustomEaCert(sslDomain, "Global Online Studio", false, false).Item3, false);
            MassEffect3PS3mainBlaze = Blaze3.CreateBlazeServer("Mass Effect 3 (PS3)", new IPEndPoint(IPAddress.Any, 33152), SSLCache.GetVulnerableCustomEaCert(sslDomain, "Global Online Studio", false, false).Item3, false);
            SsxmainBlaze = Blaze3.CreateBlazeServer("SSX 2012 (PS3)", new IPEndPoint(IPAddress.Any, 33162), SSLCache.GetVulnerableCustomEaCert(sslDomain, "Global Online Studio", false, false).Item3, false);
            Fifa12mainBlaze = Blaze3.CreateBlazeServer("FIFA 12 (PS3)", new IPEndPoint(IPAddress.Any, 33172), SSLCache.GetVulnerableCustomEaCert(sslDomain, "Global Online Studio", false, false).Item3, false);
            NFSHotPursuitmainBlaze = Blaze3.CreateBlazeServer("Need For Speed HotPursuit (PS3)", new IPEndPoint(IPAddress.Any, 33182), SSLCache.GetVulnerableCustomEaCert(sslDomain, "Global Online Studio", false, false).Item3, false);
            Crysis3mainBlaze = Blaze3.CreateBlazeServer("Crysis 3 (PS3)", new IPEndPoint(IPAddress.Any, 33192), SSLCache.GetVulnerableCustomEaCert(sslDomain, "Global Online Studio", false, false).Item3, false);
            DeadSpace3mainBlaze = Blaze3.CreateBlazeServer("Dead Space 3 (PS3)", new IPEndPoint(IPAddress.Any, 33202), SSLCache.GetVulnerableCustomEaCert(sslDomain, "Global Online Studio", false, false).Item3, false);
            PVZGWmainBlaze = Blaze3.CreateBlazeServer("PVZ Garden Warfare (PS3)", new IPEndPoint(IPAddress.Any, 33302), SSLCache.GetVulnerableCustomEaCert(sslDomain, "Global Online Studio", false, false).Item3, false);

            MassEffect3PS3mainBlaze.AddComponent<MassEffect3PS3Components.Auth.AuthComponent>();
            MassEffect3PS3mainBlaze.AddComponent<MassEffect3PS3Components.Util.UtilComponent>();

            _ = StartMainBlazeServers();

            LoggerAccessor.LogInfo("Blaze Servers initiated...");
        }

        private Task StartRedirectorServers()
        {
            //Start it!
            _ = redirector.Start(-1).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private Task StartMainBlazeServers()
        {
            //Start it!
            _ = MassEffect2PS3mainBlaze!.Start(-1).ConfigureAwait(false);
            _ = MassEffect3PS3mainBlaze!.Start(-1).ConfigureAwait(false);
            _ = SsxmainBlaze!.Start(-1).ConfigureAwait(false);
            _ = Fifa12mainBlaze!.Start(-1).ConfigureAwait(false);
            _ = NFSHotPursuitmainBlaze!.Start(-1).ConfigureAwait(false);
            _ = Crysis3mainBlaze!.Start(-1).ConfigureAwait(false);
            _ = DeadSpace3mainBlaze!.Start(-1).ConfigureAwait(false);
            _ = PVZGWmainBlaze!.Start(-1).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    redirector.Stop();

                    MassEffect2PS3mainBlaze?.Stop();
                    MassEffect3PS3mainBlaze?.Stop();
                    SsxmainBlaze?.Stop();
                    Fifa12mainBlaze?.Stop();
                    NFSHotPursuitmainBlaze?.Stop();
                    Crysis3mainBlaze?.Stop();
                    DeadSpace3mainBlaze?.Stop();
                    PVZGWmainBlaze?.Stop();

                    SSLCache = null;

                    LoggerAccessor.LogWarn("Blaze Servers stopped...");
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
