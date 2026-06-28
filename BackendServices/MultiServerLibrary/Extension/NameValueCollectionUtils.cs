using System.Collections.Specialized;
using System.Text;

namespace MultiServerLibrary.Extension
{
    public static class NameValueCollectionUtils
    {
        extension(NameValueCollection col)
        {
            public Dictionary<string, string> ToDictionary()
            {
                Dictionary<string, string> dict = [];
                foreach (var k in col.AllKeys)
                {
                    var value = col[k];
                    if (k != null && value != null)
                        dict.Add(k, value);
                }
                return dict;
            }

            public List<KeyValuePair<string, string>> ConvertHeadersToPhpFriendly()
            {
                var phpFriendlyHeaders = new List<KeyValuePair<string, string>>();

                if (col != null)
                {
                    foreach (string headerKey in col)
                    {
                        // Get all values for this header (they can be multiple)
                        var headerValues = col.GetValues(headerKey);

                        // Convert header name to uppercase, replace dashes with underscores, and prefix with "HTTP_"
                        var phpHeaderName = "HTTP_" + headerKey.ToUpper().Replace("-", "_");

                        if (headerValues != null)
                        {
                            var st = new StringBuilder();

                            // If there are multiple values for the same header, assemble them.
                            foreach (var value in headerValues)
                            {
                                if (st.Length != 0)
                                    st.Append("," + value);
                                else
                                    st.Append(value);
                            }

                            phpFriendlyHeaders.Add(
                                new KeyValuePair<string, string>(phpHeaderName, st.ToString())
                            );
                        }
                    }
                }

                return phpFriendlyHeaders;
            }
        }
    }
}
