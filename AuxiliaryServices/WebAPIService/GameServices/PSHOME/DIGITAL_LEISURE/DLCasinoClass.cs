using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIService.GameServices.PSHOME.DIGITAL_LEISURE
{
    public class DLCasinoClass
    {
        private string absolutepath;
        private string method;
        private string apipath;

        public DLCasinoClass(string method, string absolutepath, string apipath)
        {
            this.absolutepath = absolutepath;
            this.method = method;
            this.apipath = apipath;
        }

        public string ProcessRequest(IDictionary<string, string> QueryParameters, byte[] PostData, string ContentType)
        {
            if (string.IsNullOrEmpty(absolutepath) || QueryParameters.Count == 0)
                return null;

            return "<error></error>";
        }
    }
}
