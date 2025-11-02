    using SpaceWizards.HttpListener;
    using System;
    using WatsonWebserver.Core;

namespace WatsonWebserver
{
    /// <summary>
    /// HTTP context including both request and response.
    /// </summary>
    public class HttpContext : HttpContextBase
    {
        #region Public-Members

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public HttpContext()
        {

        }

        /// <summary>
        /// Instantiate.
        /// </summary>
        /// <param name="listenerCtx">HTTP listener context.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="events">Events.</param>
        /// <param name="serializer">Serializer.</param>
        internal HttpContext(
            object listenerCtx, 
            WebserverSettings settings, 
            WebserverEvents events,
            ISerializationHelper serializer,
            bool KeepAliveResponseData)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));

            if (listenerCtx is System.Net.HttpListenerContext nativeCtx)
            {
                Request = new HttpRequestNative(nativeCtx, serializer);
                Response = new HttpResponseNative(Request, nativeCtx, settings, events, serializer, KeepAliveResponseData);
            }
            else if (listenerCtx is HttpListenerContext managedCtx)
            {
                Request = new HttpRequest(managedCtx, serializer);
                Response = new HttpResponse(Request, managedCtx, settings, events, serializer, KeepAliveResponseData);
            }
            else 
                // Implicit
                throw new ArgumentNullException(nameof(listenerCtx));
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
