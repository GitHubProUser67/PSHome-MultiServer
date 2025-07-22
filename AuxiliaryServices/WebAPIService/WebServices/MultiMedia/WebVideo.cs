using System.IO;

namespace WebAPIService.WebServices.MultiMedia
{
    /// <summary>
    /// Converted web video
    /// </summary>
    public class WebVideo
    {
        /// <summary>
        /// The video file stream
        /// </summary>
        public Stream VideoStream { get; internal set; }
        /// <summary>
        /// The video container MIME content type
        /// </summary>
        public string ContentType { get; internal set; }
        /// <summary>
        /// The video container file name
        /// </summary>
        public string FileName { get; internal set; }
        /// <summary>
        /// Is the download &amp; convert successful
        /// </summary>
        public bool Available { get; internal set; }
        /// <summary>
        /// Error messages (if any)
        /// </summary>
        public string ErrorMessage { get; internal set; }

        public WebVideo()
        {
            Available = false;
            ErrorMessage = "WebVideo not configured!";
            ContentType = "text/plain";
            FileName = "webvideo.err";
        }
    }
}
