using CustomLogger;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Factory;
using QuazalServer.RDVServices.RMC;
using System.Diagnostics;
using System.Reflection;

namespace QuazalServer.QNetZ.Interfaces
{
    public class RMCServiceBase
	{
		RMCContext? _context;

		public RMCContext? Context {
			get { return _context; }
			set {
                // make it so user won't override it
                _context ??= value;
			} 
		}

		public MethodInfo? GetServiceMethodById(RMCServiceFactory factory, uint methodId)
		{
			return factory.GetServiceMethodById(GetType(), methodId);
		}

		protected void SendRMCCall<T>(QClient client, ushort protoId, uint methodId, T requestData) where T : class
		{
			if (Context != null)
                RMC.SendRMCCall(Context.Handler, client, protoId, methodId, new RMCPRequestDDL<T>(requestData));
        }

        protected RMCResult Result<T>(T replyData) where T: class
		{
			return new RMCResult(new RMCPResponseDDL<T>(replyData));
		}

		protected RMCResult Error(uint code)
		{
			return new RMCResult(new RMCPResponseEmpty(), true, code);
		}

		// This is for reverse-engineering
		protected void UNIMPLEMENTED(string additionalMessage = "")
		{
            StackTrace stackTrace = new();
			MethodBase? method = stackTrace?.GetFrame(1)?.GetMethod();

			string? methodName = method?.Name;

            RMCMethodAttribute? rmcMethodAttr = (RMCMethodAttribute?)method?.GetCustomAttributes(typeof(RMCMethodAttribute), true).SingleOrDefault();
			if (rmcMethodAttr != null && !string.IsNullOrWhiteSpace(rmcMethodAttr.Name))
				methodName = rmcMethodAttr.Name;

			LoggerAccessor.LogError($"[RMCServiceBase] - Method '{ _context?.RMC.proto }.{ methodName }' is unimplemented");

			if (!string.IsNullOrWhiteSpace(additionalMessage))
				LoggerAccessor.LogWarn($"[RMCServiceBase] - info: { additionalMessage } ");
		}
	}
}
