namespace WebAPIService.GameServices.FROMSOFTWARE
{
    public class FROMSOFTWAREClass(string method, string absolutepath, string apipath)
    {
        private readonly string absolutepath = absolutepath;
        private readonly string method = method;
        private readonly string apipath = apipath;

        public (byte[], string, string[][]) ProcessRequest(byte[] PostData, string ContentType)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return (null, null, null);

            switch (method)
            {
                case "GET":
                    switch (absolutepath)
                    {
                        case "/regulation/contents_101.bin":
                            if (File.Exists(apipath + "/FROMSOFTWARE/regulation/contents_101.bin"))
                                return (
                                    File.ReadAllBytes(
                                        apipath + "/FROMSOFTWARE/regulation/contents_101.bin"
                                    ),
                                    "application/octet-stream",
                                    [
                                        ["Last-Modified", "Wed, 15 Jan 2014 08:12:11 GMT"],
                                        ["Accept-Ranges", "bytes"],
                                        ["X-Cache", "Hit from cloudfront"],
                                        [
                                            "Via",
                                            "1.1 6895284e395204317ac1aa2c7b0a3d0c.cloudfront.net (CloudFront)",
                                        ],
                                        ["X-Amz-Cf-Pop", "MIA3-P4"],
                                        [
                                            "X-Amz-Cf-Id",
                                            "IefO3gqiGGVLwgLqePmDnindcBcuTqmbYD_Kp2GrZAsEqvtqes4qCg==",
                                        ],
                                        ["Age", "18139"],
                                    ]
                                );
                            break;
                    }
                    break;
                default:
                    break;
            }

            return (null, null, null);
        }
    }
}
