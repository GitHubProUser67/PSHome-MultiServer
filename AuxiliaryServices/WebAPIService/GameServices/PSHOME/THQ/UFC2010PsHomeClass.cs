using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CastleLibrary.S0ny.XI5;
using CustomLogger;
using HttpMultipartParser;

namespace WebAPIService.GameServices.PSHOME.THQ
{
    public static partial class UFC2010PsHomeClass
    {
        private const int defaultTokenAmount = 25000;
        private const string defaultWrittenDate = "2009:00:00:00:00:00:00";

        private static readonly string UFCData = GenerateDefaultData();

        public static string ProcessUFCUserData(byte[] postdata, string boundary, string apiPath)
        {
            string output = null;

            if (!string.IsNullOrEmpty(boundary) && postdata != null)
            {
                try
                {
                    using (var copyStream = new MemoryStream(postdata))
                    {
                        byte[] ticketData = null;

                        var data = MultipartFormDataParser.Parse(copyStream, boundary);

                        var func = data.GetParameterValue("func");

                        var id = data.GetParameterValue("id");

                        foreach (var file in data.Files.Where(x => x.FileName == "ticket.bin"))
                        {
                            using (var filedata = file.Data)
                            {
                                filedata.Position = 0;

                                // Find the number of bytes in the stream
                                var contentLength = (int)filedata.Length;

                                // Create a byte array
                                ticketData = new byte[contentLength];

                                // Read the contents of the memory stream into the byte array
                                filedata.Read(ticketData, 0, contentLength);
                            }
                        }

                        if (ticketData != null)
                        {
                            // get ticket
                            var ticket = XI5Ticket.ReadFromBytes(ticketData);

                            // setup username
                            var username = ticket.Username;

                            // invalid ticket
                            if (!ticket.Valid)
                            {
                                // log to console
                                LoggerAccessor.LogWarn(
                                    $"[UFC2010PsHomeClass] - ProcessUFCUserData: User {username} tried to alter their ticket data"
                                );

                                return null;
                            }

                            // RPCN
                            if (ticket.IsSignedByRPCN)
                                LoggerAccessor.LogInfo(
                                    $"[UFC2010PsHomeClass] - ProcessUFCUserData: User {username} connected at: {DateTime.Now} and is on RPCN"
                                );
                            else if (username.EndsWith($"@{XI5Ticket.RPCNSigner}"))
                            {
                                LoggerAccessor.LogError(
                                    $"[UFC2010PsHomeClass] - ProcessUFCUserData: User {username} was caught using a RPCN suffix while not on it!"
                                );

                                return null;
                            }
                            else
                                LoggerAccessor.LogInfo(
                                    $"[UFC2010PsHomeClass] - ProcessUFCUserData: User {username} connected at: {DateTime.Now} and is on PSN"
                                );

                            if (id == username)
                            {
                                const string tokensRegex = @"<tokens>(\d+)</tokens>";
                                var profileDirectoryPah = $"{apiPath}/HOME_THQ/{id}/";
                                var profilePath = $"{profileDirectoryPah}data.xml";

                                Directory.CreateDirectory(profileDirectoryPah);

                                switch (func)
                                {
                                    case "read":

                                        if (File.Exists(profilePath))
                                        {
                                            const string xmlHeader =
                                                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n";

                                            output = File.ReadAllText(profilePath);

                                            // Cleans up old xml data produced by previous versions of the API, while preserving token amount.
                                            if (output.StartsWith(xmlHeader))
                                            {
                                                var match = MyRegex().Match(output);
                                                if (
                                                    match.Success
                                                    && int.TryParse(
                                                        match.Groups[1].Value,
                                                        out var currentTokenAmount
                                                    )
                                                )
                                                {
                                                    output = Regex.Replace(
                                                        UFCData,
                                                        @"<tokens>\d+</tokens>",
                                                        $"<tokens>{currentTokenAmount}</tokens>"
                                                    );
                                                    File.WriteAllText(profilePath, output);
                                                }
                                                else // Invalid data.
                                                    output = null;
                                            }
                                        }
                                        else
                                            output = UFCData;

                                        break;
                                    case "write":

                                        try
                                        {
                                            const string tokenElement = "<tokens>";
                                            const string tokenElementTerm = "</tokens>";

                                            var val2 = data.GetParameterValue("val2");

                                            if (File.Exists(profilePath))
                                            {
                                                output = File.ReadAllText(profilePath);

                                                if (MyRegex().Match(output).Success)
                                                    output = Regex.Replace(
                                                        output,
                                                        $@"{tokenElement}\d+{tokenElementTerm}",
                                                        $"{tokenElement}{val2}{tokenElementTerm}"
                                                    );
                                                else // Invalid data.
                                                {
                                                    output = null;
                                                    break;
                                                }
                                            }
                                            else
                                                output = UFCData.Replace(
                                                    $"{tokenElement}{defaultTokenAmount}{tokenElementTerm}",
                                                    $"{tokenElement}{val2}{tokenElementTerm}"
                                                );

                                            File.WriteAllText(profilePath, output);
                                        }
                                        catch
                                        {
                                            // Invalid request.
                                        }

                                        break;
                                    case "cards":

                                        const string rootXmlElement = "<root>";
                                        const string rootXmlElementTerm = "</root>";
                                        const string card00 = "card00";
                                        const string card0 = "card0";
                                        const string card = "card";

                                        try
                                        {
                                            var subfunc = data.GetParameterValue("subfunc");

                                            switch (subfunc)
                                            {
                                                case "addcard":
                                                case "add2cards":

                                                    try
                                                    {
                                                        var cardnum = (int)
                                                            double.Parse(
                                                                data.GetParameterValue("cardnum"),
                                                                CultureInfo.InvariantCulture
                                                            );
                                                        string elementName;
                                                        var xml = File.Exists(profilePath)
                                                            ? XElement.Parse(
                                                                $"{rootXmlElement}{File.ReadAllText(profilePath)}{rootXmlElementTerm}"
                                                            )
                                                            : XElement.Parse(
                                                                $"{rootXmlElement}{UFCData}{rootXmlElementTerm}"
                                                            );
                                                        elementName =
                                                            cardnum < 10 ? card00 + cardnum
                                                            : cardnum < 100 ? card0 + cardnum
                                                            : card + cardnum;

                                                        var parserElement = xml.Element(
                                                            elementName
                                                        );

                                                        if (
                                                            parserElement != null
                                                            && int.TryParse(
                                                                parserElement.Value,
                                                                out var numOfCardObtained
                                                            )
                                                        )
                                                            parserElement.Value = (
                                                                numOfCardObtained
                                                                + (subfunc == "add2cards" ? 2 : 1)
                                                            ).ToString();

                                                        try
                                                        {
                                                            elementName = "fb01";

                                                            var fb01 = data.GetParameterValue(
                                                                elementName
                                                            );

                                                            if (!string.IsNullOrEmpty(fb01))
                                                            {
                                                                parserElement = xml.Element(
                                                                    elementName
                                                                );

                                                                parserElement?.Value = fb01;
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            // Not every requests has this field.
                                                        }

                                                        output = xml.ToString()
                                                            .Replace(rootXmlElement, string.Empty)
                                                            .Replace(
                                                                rootXmlElementTerm,
                                                                string.Empty
                                                            )
                                                            .Replace(" ", string.Empty)
                                                            .Replace(
                                                                Environment.NewLine,
                                                                string.Empty
                                                            );
                                                        File.WriteAllText(profilePath, output);
                                                    }
                                                    catch
                                                    {
                                                        // Invalid request or XML data.
                                                    }

                                                    break;

                                                case "cashbook":

                                                    try
                                                    {
                                                        var points = (int)
                                                            double.Parse(
                                                                data.GetParameterValue("points"),
                                                                CultureInfo.InvariantCulture
                                                            );
                                                        var numsets = (int)
                                                            double.Parse(
                                                                data.GetParameterValue("numsets"),
                                                                CultureInfo.InvariantCulture
                                                            );
                                                        int i;
                                                        string elementName;
                                                        var xml = File.Exists(profilePath)
                                                            ? XElement.Parse(
                                                                $"{rootXmlElement}{File.ReadAllText(profilePath)}{rootXmlElementTerm}"
                                                            )
                                                            : XElement.Parse(
                                                                $"{rootXmlElement}{UFCData}{rootXmlElementTerm}"
                                                            );
                                                        XElement parserElement;

                                                        parserElement = xml.Element("tokens");

                                                        if (
                                                            parserElement != null
                                                            && int.TryParse(
                                                                parserElement.Value,
                                                                out var currentTokenAmount
                                                            )
                                                        )
                                                            parserElement.Value = (
                                                                currentTokenAmount + points
                                                            ).ToString();

                                                        parserElement = xml.Element("books");

                                                        if (
                                                            parserElement != null
                                                            && int.TryParse(
                                                                parserElement.Value,
                                                                out var numOfSoldBooks
                                                            )
                                                        )
                                                            parserElement.Value = (
                                                                numOfSoldBooks + 1
                                                            ).ToString();

                                                        for (i = 1; i <= numsets; i++)
                                                        {
                                                            elementName =
                                                                i < 10 ? "set0" + i : "set" + i;

                                                            parserElement = xml.Element(
                                                                elementName
                                                            );

                                                            if (
                                                                parserElement != null
                                                                && int.TryParse(
                                                                    parserElement.Value,
                                                                    out var numOfSoldSet
                                                                )
                                                            )
                                                                parserElement.Value = (
                                                                    numOfSoldSet - 1
                                                                ).ToString();
                                                        }

                                                        output = xml.ToString()
                                                            .Replace(rootXmlElement, string.Empty)
                                                            .Replace(
                                                                rootXmlElementTerm,
                                                                string.Empty
                                                            )
                                                            .Replace(" ", string.Empty)
                                                            .Replace(
                                                                Environment.NewLine,
                                                                string.Empty
                                                            );
                                                        File.WriteAllText(profilePath, output);
                                                    }
                                                    catch
                                                    {
                                                        // Invalid request or XML data.
                                                    }

                                                    break;

                                                case "cashset":

                                                    try
                                                    {
                                                        var points = (int)
                                                            double.Parse(
                                                                data.GetParameterValue("points"),
                                                                CultureInfo.InvariantCulture
                                                            );
                                                        var setnum = (int)
                                                            double.Parse(
                                                                data.GetParameterValue("setnum"),
                                                                CultureInfo.InvariantCulture
                                                            );
                                                        int[] cards =
                                                        [
                                                            .. data.GetParameterValue("cards")
                                                                .Split('-')
                                                                .Select(cardx =>
                                                                    (int)
                                                                        double.Parse(
                                                                            cardx,
                                                                            CultureInfo.InvariantCulture
                                                                        )
                                                                ),
                                                        ];

                                                        string elementName;
                                                        var xml = File.Exists(profilePath)
                                                            ? XElement.Parse(
                                                                $"{rootXmlElement}{File.ReadAllText(profilePath)}{rootXmlElementTerm}"
                                                            )
                                                            : XElement.Parse(
                                                                $"{rootXmlElement}{UFCData}{rootXmlElementTerm}"
                                                            );
                                                        XElement parserElement;

                                                        parserElement = xml.Element("tokens");

                                                        if (
                                                            parserElement != null
                                                            && int.TryParse(
                                                                parserElement.Value,
                                                                out var currentTokenAmount
                                                            )
                                                        )
                                                            parserElement.Value = (
                                                                currentTokenAmount + points
                                                            ).ToString();

                                                        elementName =
                                                            setnum < 10
                                                                ? "set0" + setnum
                                                                : "set" + setnum;

                                                        parserElement = xml.Element(elementName);

                                                        if (
                                                            parserElement != null
                                                            && int.TryParse(
                                                                parserElement.Value,
                                                                out var numOfSoldSet
                                                            )
                                                        )
                                                            parserElement.Value = (
                                                                numOfSoldSet + 1
                                                            ).ToString();

                                                        foreach (var cardIter in cards)
                                                        {
                                                            elementName =
                                                                cardIter < 10 ? card00 + cardIter
                                                                : cardIter < 100 ? card0 + cardIter
                                                                : card + cardIter;

                                                            parserElement = xml.Element(
                                                                elementName
                                                            );

                                                            if (
                                                                parserElement != null
                                                                && int.TryParse(
                                                                    parserElement.Value,
                                                                    out var numOfCardObtained
                                                                )
                                                            )
                                                                parserElement.Value = (
                                                                    numOfCardObtained - 1
                                                                ).ToString();
                                                        }

                                                        output = xml.ToString()
                                                            .Replace(rootXmlElement, string.Empty)
                                                            .Replace(
                                                                rootXmlElementTerm,
                                                                string.Empty
                                                            )
                                                            .Replace(" ", string.Empty)
                                                            .Replace(
                                                                Environment.NewLine,
                                                                string.Empty
                                                            );
                                                        File.WriteAllText(profilePath, output);
                                                    }
                                                    catch
                                                    {
                                                        // Invalid request or XML data.
                                                    }

                                                    break;

                                                case "giftcard":
                                                case "gift2cards":

                                                    try
                                                    {
                                                        var cardnum = (int)
                                                            double.Parse(
                                                                data.GetParameterValue("cardnum"),
                                                                CultureInfo.InvariantCulture
                                                            );
                                                        var otherid = data.GetParameterValue(
                                                            "otherid"
                                                        );

                                                        _ = Task.Run(() =>
                                                        {
                                                            var otherProfileDirectoryPath =
                                                                $"{apiPath}/HOME_THQ/{otherid}/";
                                                            var otherProfilePath =
                                                                $"{otherProfileDirectoryPath}data.xml";
                                                            string elementName1;
                                                            XElement xml1;

                                                            Directory.CreateDirectory(
                                                                otherProfileDirectoryPath
                                                            );

                                                            xml1 = File.Exists(otherProfilePath)
                                                                ? XElement.Parse(
                                                                    $"{rootXmlElement}{otherProfilePath}{rootXmlElementTerm}"
                                                                )
                                                                : XElement.Parse(
                                                                    $"{rootXmlElement}{UFCData}{rootXmlElementTerm}"
                                                                );

                                                            XElement parserElement1;

                                                            elementName1 =
                                                                cardnum < 10 ? card00 + cardnum
                                                                : cardnum < 100 ? card0 + cardnum
                                                                : card + cardnum;

                                                            parserElement1 = xml1.Element(
                                                                elementName1
                                                            );

                                                            if (
                                                                parserElement1 != null
                                                                && int.TryParse(
                                                                    parserElement1.Value,
                                                                    out var numOfCardObtained1
                                                                )
                                                            )
                                                                parserElement1.Value = (
                                                                    numOfCardObtained1
                                                                    + (
                                                                        subfunc == "gift2cards"
                                                                            ? 2
                                                                            : 1
                                                                    )
                                                                ).ToString();

                                                            File.WriteAllText(
                                                                otherProfilePath,
                                                                xml1.ToString()
                                                                    .Replace(
                                                                        rootXmlElement,
                                                                        string.Empty
                                                                    )
                                                                    .Replace(
                                                                        rootXmlElementTerm,
                                                                        string.Empty
                                                                    )
                                                                    .Replace(" ", string.Empty)
                                                                    .Replace(
                                                                        Environment.NewLine,
                                                                        string.Empty
                                                                    )
                                                            );
                                                        });

                                                        string elementName;
                                                        var xml = File.Exists(profilePath)
                                                            ? XElement.Parse(
                                                                $"{rootXmlElement}{File.ReadAllText(profilePath)}{rootXmlElementTerm}"
                                                            )
                                                            : XElement.Parse(
                                                                $"{rootXmlElement}{UFCData}{rootXmlElementTerm}"
                                                            );
                                                        XElement parserElement;

                                                        elementName =
                                                            cardnum < 10 ? card00 + cardnum
                                                            : cardnum < 100 ? card0 + cardnum
                                                            : card + cardnum;

                                                        parserElement = xml.Element(elementName);

                                                        if (
                                                            parserElement != null
                                                            && int.TryParse(
                                                                parserElement.Value,
                                                                out var numOfCardObtained
                                                            )
                                                        )
                                                            parserElement.Value = (
                                                                numOfCardObtained - 1
                                                            ).ToString();

                                                        output = xml.ToString()
                                                            .Replace(rootXmlElement, string.Empty)
                                                            .Replace(
                                                                rootXmlElementTerm,
                                                                string.Empty
                                                            )
                                                            .Replace(" ", string.Empty)
                                                            .Replace(
                                                                Environment.NewLine,
                                                                string.Empty
                                                            );
                                                        File.WriteAllText(profilePath, output);
                                                    }
                                                    catch
                                                    {
                                                        // Invalid request or XML data.
                                                    }

                                                    break;
                                            }
                                        }
                                        catch
                                        {
                                            // Invalid request.
                                        }

                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError(
                        $"[UFC2010PsHomeClass] - ProcessUFCUserData: thrown an assertion. (Exception: {ex})"
                    );

                    output = null;
                }
            }

            return output;
        }

        private static string GenerateDefaultData()
        {
            int i;
            string elementName;

            var st = new StringBuilder(
                $"<UFC>2</UFC><tokens>{defaultTokenAmount}</tokens><books>0</books>"
            );

            for (i = 1; i <= 10; i++)
            {
                elementName = i < 10 ? "set0" + i : "set" + i;

                st.Append($"<{elementName}>0</{elementName}>");
            }

            for (i = 1; i <= 102; i++)
            {
                elementName =
                    i < 10 ? "card00" + i
                    : i < 100 ? "card0" + i
                    : "card" + i;

                st.Append($"<{elementName}>0</{elementName}>");
            }

            st.Append($"<fb01>{defaultWrittenDate}</fb01>");

            return st.ToString();
        }

        [GeneratedRegex(@"<tokens>(\d+)</tokens>")]
        private static partial Regex MyRegex();
    }
}
