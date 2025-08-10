using System;

namespace HomeTools.Utils
{
    public static class ThreadLimiter
    {
#if SERVER_MODE
        public static readonly int NumOfThreadsAvailable = 2;
#else
        public static readonly int NumOfThreadsAvailable = Environment.ProcessorCount;
#endif
    }
}
