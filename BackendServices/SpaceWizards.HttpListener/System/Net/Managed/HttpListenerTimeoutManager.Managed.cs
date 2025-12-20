// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
#if NETCORE3_1_OR_LOWER
using System.Runtime.InteropServices;
#endif
using System.Runtime.Versioning;

namespace SpaceWizards.HttpListener
{
    public class HttpListenerTimeoutManager
    {
        private TimeSpan _drainEntityBody = TimeSpan.Zero;
        private TimeSpan _idleConnection = TimeSpan.Zero;

        internal HttpListenerTimeoutManager(HttpListener listener) { }

        public TimeSpan DrainEntityBody
        {
            get => _drainEntityBody;
            set
            {
                // Managed implementation currently doesn't pool connections,
                // so this is a nop other than roundtripping the value.
                ValidateTimeout(value);
                _drainEntityBody = value;
            }
        }

        public TimeSpan IdleConnection
        {
            get => _idleConnection;
            set
            {
                // Managed implementation currently doesn't pool connections,
                // so this is a nop other than roundtripping the value.
                ValidateTimeout(value);
                _idleConnection = value;
            }
        }

        public TimeSpan EntityBody
        {
            get => TimeSpan.Zero;
#if NET5_0_OR_GREATER
            [SupportedOSPlatform("windows")]
#endif
            set
            {
#if NETCORE3_1_OR_LOWER
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return;
#endif
                ValidateTimeout(value);
                throw new PlatformNotSupportedException(); // low usage, not currently implemented
            }
        }

        public TimeSpan HeaderWait
        {
            get => TimeSpan.Zero;
#if NET5_0_OR_GREATER
            [SupportedOSPlatform("windows")]
#endif
            set
            {
#if NETCORE3_1_OR_LOWER
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return;
#endif
                ValidateTimeout(value);
                throw new PlatformNotSupportedException(); // low usage, not currently implemented
            }
        }

        public long MinSendBytesPerSecond
        {
            get => 0;
#if NET5_0_OR_GREATER
            [SupportedOSPlatform("windows")]
#endif
            set
            {
#if NETCORE3_1_OR_LOWER
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return;
#endif
                if (value < 0 || value > uint.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                throw new PlatformNotSupportedException(); // low usage, not currently implemented
            }
        }

        public TimeSpan RequestQueue
        {
            get => TimeSpan.Zero;
#if NET5_0_OR_GREATER
            [SupportedOSPlatform("windows")]
#endif
            set
            {
#if NETCORE3_1_OR_LOWER
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return;
#endif
                ValidateTimeout(value);
                throw new PlatformNotSupportedException(); // low usage, not currently implemented
            }
        }

        private void ValidateTimeout(TimeSpan value)
        {
            long timeoutValue = Convert.ToInt64(value.TotalSeconds);
            if (timeoutValue < 0 || timeoutValue > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
