using System.Diagnostics;
using System.Reflection;
using CustomLogger;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Factory;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.QNetZ.Interfaces
{
    public class RMCServiceBase
    {
        RMCContext? _context;

        public RMCContext? Context
        {
            get { return _context; }
            set
            {
                // make it so user won't override it
                _context ??= value;
            }
        }

        public MethodInfo? GetServiceMethodById(RMCServiceFactory factory, uint methodId)
        {
            return RMCServiceFactory.GetServiceMethodById(GetType(), methodId);
        }

        protected void SendRMCCall<T>(QClient client, ushort protoId, uint methodId, T requestData)
            where T : class
        {
            if (Context != null)
                RMC.SendRMCCall(
                    Context.Handler,
                    client,
                    protoId,
                    methodId,
                    new RMCPRequestDDL<T>(requestData)
                );
        }

        protected static RMCResult Result<T>(T replyData)
            where T : class
        {
            return new RMCResult(new RMCPResponseDDL<T>(replyData));
        }

        protected static RMCResult Error(uint code)
        {
            return new RMCResult(new RMCPResponseEmpty(), true, code);
        }

        // This is for reverse-engineering
        protected void UNIMPLEMENTED(string additionalMessage = "")
        {
            StackTrace stackTrace = new();
            var method = stackTrace?.GetFrame(1)?.GetMethod();

            var methodName = method?.Name;

            var rmcMethodAttr = (RMCMethodAttribute?)
                method?.GetCustomAttributes(typeof(RMCMethodAttribute), true).SingleOrDefault();
            if (rmcMethodAttr != null && !string.IsNullOrWhiteSpace(rmcMethodAttr.Name))
                methodName = rmcMethodAttr.Name;

            LoggerAccessor.LogError(
                $"[RMCServiceBase] - Method '{_context?.RMC.proto}.{methodName}' is unimplemented"
            );

            if (!string.IsNullOrWhiteSpace(additionalMessage))
                LoggerAccessor.LogWarn($"[RMCServiceBase] - info: {additionalMessage} ");
        }
    }
}
