using CustomLogger;
using Lextm.BouncyCastle;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MultiServerLibrary.SNMP
{
    // Test software available at: https://github.com/lextudio/sharpsnmplib-samples/tree/master/Samples/CSharp/snmptrapd
    public class SnmpTrapSender : IDisposable
    {
        public readonly ReportMessage report;

        private readonly string snmpEnterpriseOid;
        private readonly string snmpUsername;
        private readonly object snmpAuthProvider;
        private readonly IPEndPoint snmpEndPoint;
        private readonly BlockingCollection<(string Severity, string Message)> trapQueue = new BlockingCollection<(string Severity, string Message)>();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly Task processingTask;

        public SnmpTrapSender(string hashAlgorithm, string trapHost, string username, string authPwd, string privPwd, string enterpriseOid, int port = 162)
        {
            snmpEnterpriseOid = "1.3.6.1.4.1." + enterpriseOid;
            snmpUsername = username;
#pragma warning disable
            if (AESPrivacyProvider.IsSupported)
                snmpAuthProvider = new AESPrivacyProvider(new OctetString(privPwd), GetAuthenticationProvider(hashAlgorithm, new OctetString(authPwd)));
            else
                snmpAuthProvider = new BouncyCastleAESPrivacyProvider(new OctetString(privPwd), GetAuthenticationProvider(hashAlgorithm, new OctetString(authPwd)));
#pragma warning restore
            snmpEndPoint = new IPEndPoint(IPAddress.Parse(trapHost), port);

            // Send initial v1 trap
            Messenger.SendTrapV1(snmpEndPoint, IPAddress.Loopback, new OctetString("public"),
                new ObjectIdentifier(snmpEnterpriseOid), GenericCode.ColdStart, 0, 0, new List<Variable>());

            const int timeoutValue = 60000;

            try
            {
                report = Messenger.GetNextDiscovery(SnmpType.InformRequestPdu).GetResponse(timeoutValue, snmpEndPoint);

                AESPrivacyProvider provider = snmpAuthProvider as AESPrivacyProvider;
                if (provider != null)
                {
                    Messenger.SendInform(0, VersionCode.V3, snmpEndPoint, new OctetString(snmpUsername), OctetString.Empty,
                        new ObjectIdentifier(snmpEnterpriseOid), 0, new List<Variable>(), 2000,
                        provider,
                        report);
                }
                else
                {
                    Messenger.SendInform(0, VersionCode.V3, snmpEndPoint, new OctetString(snmpUsername), OctetString.Empty,
                        new ObjectIdentifier(snmpEnterpriseOid), 0, new List<Variable>(), 2000,
                        (BouncyCastleAESPrivacyProvider)snmpAuthProvider,
                        report);
                }
            }
            catch
            {
                LoggerAccessor.LogError($"[SnmpTrapSender] - SNMPv3 failed to Inform the supervisor, SNMP reporting will be disabled!");
            }

            processingTask = Task.Run(ProcessTrapQueueAsync);
        }

        public void SendInfo(string message) => EnqueueTrap("INFO", message);
        public void SendWarn(string message) => EnqueueTrap("WARNING", message);
        public void SendCrit(string message) => EnqueueTrap("CRITICAL", message);

        private void EnqueueTrap(string severity, string message)
        {
            while (!trapQueue.IsAddingCompleted)
            {
                if (trapQueue.TryAdd((severity, message), TimeSpan.FromMilliseconds(100)))
                    break;
            }
        }

        private async Task ProcessTrapQueueAsync()
        {
            foreach (var (severity, message) in trapQueue.GetConsumingEnumerable(cts.Token))
            {
                try
                {
                    SendTrap(severity, message);
                }
                catch
                {
                }

                await Task.Delay(10).ConfigureAwait(false);
            }
        }

        private void SendTrap(string severity, string message)
        {
            if (report == null || report.Parameters.IsInvalid ||
                report.Parameters.EngineBoots == null || report.Parameters.EngineTime == null)
                return;

            DateTime now = DateTime.Now;
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(now);
            uint sysUpTime = (uint)Environment.TickCount / 10;

            AESPrivacyProvider provider = snmpAuthProvider as AESPrivacyProvider;
            if (provider != null)
            {
                new TrapV2Message(
                VersionCode.V3,
                Messenger.NextMessageId,
                Messenger.NextRequestId,
                new OctetString(snmpUsername),
                new ObjectIdentifier(snmpEnterpriseOid),
                sysUpTime,
                new List<Variable>
                {
                    new Variable(new ObjectIdentifier($"{snmpEnterpriseOid}.1"), new OctetString(severity)),
                    new Variable(new ObjectIdentifier($"{snmpEnterpriseOid}.2"), new OctetString(message)),
                    new Variable(new ObjectIdentifier($"{snmpEnterpriseOid}.3"), new TimeTicks(sysUpTime)),
                    new Variable(new ObjectIdentifier($"{snmpEnterpriseOid}.4"), new OctetString(DateTime.UtcNow.ToString("o"))),
                    new Variable(new ObjectIdentifier($"{snmpEnterpriseOid}.5"), new OctetString(new byte[]
                    {
                        (byte)(now.Year >> 8),
                        (byte)(now.Year & byte.MaxValue),
                        (byte)now.Month,
                        (byte)now.Day,
                        (byte)now.Hour,
                        (byte)now.Minute,
                        (byte)now.Second,
                        (byte)(now.Millisecond / 100),
                        (byte)(offset.Ticks < 0 ? '-' : '+'),
                        (byte)Math.Abs(offset.Hours),
                        (byte)Math.Abs(offset.Minutes)
                    }))
                },
                provider,
                report.Header.MaxSize,
                report.Parameters.EngineId,
                report.Parameters.EngineBoots.ToInt32(),
                report.Parameters.EngineTime.ToInt32()).Send(snmpEndPoint);
            }
            else
            {
                new TrapV2Message(
                VersionCode.V3,
                Messenger.NextMessageId,
                Messenger.NextRequestId,
                new OctetString(snmpUsername),
                new ObjectIdentifier(snmpEnterpriseOid),
                sysUpTime,
                new List<Variable>
                {
                    new Variable(new ObjectIdentifier($"{snmpEnterpriseOid}.1"), new OctetString(severity)),
                    new Variable(new ObjectIdentifier($"{snmpEnterpriseOid}.2"), new OctetString(message)),
                    new Variable(new ObjectIdentifier($"{snmpEnterpriseOid}.3"), new TimeTicks(sysUpTime)),
                    new Variable(new ObjectIdentifier($"{snmpEnterpriseOid}.4"), new OctetString(DateTime.UtcNow.ToString("o"))),
                    new Variable(new ObjectIdentifier($"{snmpEnterpriseOid}.5"), new OctetString(new byte[]
                    {
                        (byte)(now.Year >> 8),
                        (byte)(now.Year & byte.MaxValue),
                        (byte)now.Month,
                        (byte)now.Day,
                        (byte)now.Hour,
                        (byte)now.Minute,
                        (byte)now.Second,
                        (byte)(now.Millisecond / 100),
                        (byte)(offset.Ticks < 0 ? '-' : '+'),
                        (byte)Math.Abs(offset.Hours),
                        (byte)Math.Abs(offset.Minutes)
                    }))
                },
                (BouncyCastleAESPrivacyProvider)snmpAuthProvider,
                report.Header.MaxSize,
                report.Parameters.EngineId,
                report.Parameters.EngineBoots.ToInt32(),
                report.Parameters.EngineTime.ToInt32()).Send(snmpEndPoint);
            }
        }

        public void Dispose()
        {
            trapQueue.CompleteAdding();
            cts.Cancel();
            processingTask.Wait();
            trapQueue.Dispose();
            cts.Dispose();
        }

        private static IAuthenticationProvider GetAuthenticationProvider(string providerName, OctetString phrase)
        {
            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentNullException(nameof(providerName));

            string providerNameUpper = providerName.ToUpper();

            switch (providerNameUpper)
            {
                case "MD5":
#pragma warning disable
                    return new MD5AuthenticationProvider(phrase);
                case "SHA1":
                    return new SHA1AuthenticationProvider(phrase);
#pragma warning restore
                case "SHA256":
                    return new SHA256AuthenticationProvider(phrase);
                case "SHA384":
                    return new SHA384AuthenticationProvider(phrase);
                case "SHA512":
                    return new SHA512AuthenticationProvider(phrase);
            }

            const string cryptoLibrary = "CastleLibrary";

            // Load external assembly (if not already loaded)
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == cryptoLibrary);

            if (assembly == null)
                // Load from file or other source if necessary
                assembly = Assembly.Load(cryptoLibrary);

            Type providerType = assembly.GetTypes()
                .FirstOrDefault(t => t.Name.Equals("BouncyCastle" + providerNameUpper + "AuthenticationProvider")
                    && typeof(IAuthenticationProvider).IsAssignableFrom(t));

            if (providerType == null)
                throw new InvalidOperationException($"[SnmpTrapSender] - Bouncy Castle Authentication provider '{providerName}' not found in {cryptoLibrary}.");

            return (IAuthenticationProvider)Activator.CreateInstance(providerType, phrase);
        }
    }
}
