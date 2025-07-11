using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace MultiServerLibrary.Extension
{
    public static class NameValueCollectionUtils
    {
        public static IDictionary<string, string> ToDictionary(this NameValueCollection col)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string k in col.AllKeys)
            {
                string value = col[k];
                if (k != null && value != null)
                    dict.Add(k, value);
            }
            return dict;
        }

        public static List<KeyValuePair<string, string>> ConvertHeadersToPhpFriendly(this NameValueCollection headers)
        {
            List<KeyValuePair<string, string>> phpFriendlyHeaders = new List<KeyValuePair<string, string>>();

            if (headers != null)
            {
                foreach (string headerKey in headers)
                {
                    // Get all values for this header (they can be multiple)
                    string[] headerValues = headers.GetValues(headerKey);

                    // Convert header name to uppercase, replace dashes with underscores, and prefix with "HTTP_"
                    string phpHeaderName = "HTTP_" + headerKey.ToUpper().Replace("-", "_");

                    if (headerValues != null)
                    {
                        StringBuilder st = new StringBuilder();

                        // If there are multiple values for the same header, assemble them.
                        foreach (string value in headerValues)
                        {
                            if (st.Length != 0)
                                st.Append("," + value);
                            else
                                st.Append(value);
                        }

                        phpFriendlyHeaders.Add(new KeyValuePair<string, string>(phpHeaderName, st.ToString()));
                    }
                }
            }

            return phpFriendlyHeaders;
        }
    }
}
