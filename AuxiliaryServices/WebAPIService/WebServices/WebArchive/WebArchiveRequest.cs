using System;
using System.Net.Http;
using System.IO;

namespace WebAPIService.WebServices.WebArchive
{
    /// <summary>
    /// From WebOne (https://github.com/atauenis/webone).
    /// Request to a Internet Archive Wayback Machine CDX server for archived copy of website
    /// </summary>
    public class WebArchiveRequest
	{
        /* Documentation:
		https://github.com/internetarchive/wayback/blob/master/wayback-cdx-server/README.md
		https://github.com/atauenis/webone/wiki/Wayback-Machine
		https://archive.org/help/wayback_api.php */

        /// <summary>
        /// Upper limit of Web Archive page date
        /// </summary>
        public static int ArchiveDateLimit = 0;

        /// <summary>
        /// Is the requested URL archived by Wayback Machine
        /// </summary>
        public bool Archived { get; private set; }

        /// <summary>
        /// Address of archived copy of requested URL
        /// </summary>
        public string ArchivedURL { get; private set; }

        /// <summary>
        /// Check Wayback Machine for archived copy of Web page at <paramref name="URL"/>
        /// </summary>
        /// <param name="URL">Address of original Web page</param>
        public WebArchiveRequest(string URL)
		{
			const int CdxFieldsCount = 3;

			// send request to CDX server
			using (HttpResponseMessage CdxResponse = new HttpClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(string.Format(
			"https://web.archive.org/cdx/search/cdx?fl={0}&url={1}",
			"timestamp,original,statuscode", // fields: ["urlkey","timestamp","original","mimetype","statuscode","digest","length"]
			Uri.EscapeDataString(URL))))).Result)
			{
                if (!CdxResponse.IsSuccessStatusCode) throw new Exception("Unsuccessful Web Archive request: " + CdxResponse.ReasonPhrase ?? " without reason");
                using (StreamReader st = new StreamReader(CdxResponse.Content.ReadAsStreamAsync().Result))
                {
                    string[] CdxBody = st.ReadToEnd().TrimEnd().Split('\n');
                    st.Close();

                    if (CdxBody.Length == 0 || CdxBody[0] == string.Empty)
                    {
                        // not archived
                        Archived = false;
                        ArchivedURL = string.Empty;
                        return;
                    }

                    // find last (or last at ArchiveDateLimit date) archived version, preferable without redirects
                    string LastCdxEntry = string.Empty;
                    foreach (string CdxEntry in CdxBody)
                    {
                        string[] Fields = CdxEntry.Split(' ');
                        if (Fields.Length != CdxFieldsCount) continue;
                        if (ArchiveDateLimit > 0)
                        {
                            long.TryParse(Fields[0], out long Timestamp);
                            if (Timestamp > ArchiveDateLimit * Math.Pow(10, 6)) continue;
                        }
                        if (Fields[2] == "200") LastCdxEntry = CdxEntry;
                    }
                    if (LastCdxEntry == string.Empty) LastCdxEntry = CdxBody[CdxBody.Length - 1];

                    string[] ResultFields = LastCdxEntry.Split(' ');
                    if (ResultFields.Length != CdxFieldsCount)
                    {
                        // bad CDX syntax
                        Archived = false;
                        ArchivedURL = string.Empty;
                        throw new Exception("Incorrect Web Archive request: " + LastCdxEntry);
                    }

                    Archived = true;
                    ArchivedURL = string.Format("http://web.archive.org/web/{0}/{1}", ResultFields[0], ResultFields[1]);
                }
            }
		}
	}
}
