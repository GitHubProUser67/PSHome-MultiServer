using System;
using System.IO;
using HttpMultipartParser;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading.Tasks;

namespace WebAPIService.GameServices.THQ
{
    public static class UFC2010PsHomeClass
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
                    using (MemoryStream copyStream = new MemoryStream(postdata))
                    {
                        byte[] ticketData = null;

                        var data = MultipartFormDataParser.Parse(copyStream, boundary);

                        string func = data.GetParameterValue("func");

                        string id = data.GetParameterValue("id");

                        foreach (FilePart file in data.Files.Where(x => x.FileName == "ticket.bin"))
                        {
                            using (Stream filedata = file.Data)
                            {
                                filedata.Position = 0;

                                // Find the number of bytes in the stream
                                int contentLength = (int)filedata.Length;

                                // Create a byte array
                                ticketData = new byte[contentLength];

                                // Read the contents of the memory stream into the byte array
                                filedata.Read(ticketData, 0, contentLength);
                            }
                        }

                        if (ticketData != null)
                        {
                            // Extract the desired portion of the binary data
                            byte[] extractedData = new byte[0x63 - 0x54 + 1];

                            // Copy it
                            Array.Copy(ticketData, 0x54, extractedData, 0, extractedData.Length);

                            // Convert 0x00 bytes to 0x20 so we pad as space.
                            for (int i = 0; i < extractedData.Length; i++)
                            {
                                if (extractedData[i] == 0x00)
                                    extractedData[i] = 0x20;
                            }

                            // Convert the modified data to a string
                            if (id == Encoding.ASCII.GetString(extractedData).Replace(" ", string.Empty))
                            {
                                const string tokensRegex = @"<tokens>(\d+)</tokens>";
                                string profileDirectoryPah = $"{apiPath}/HOME_THQ/{id}/";
                                string profilePath = $"{profileDirectoryPah}data.xml";

                                Directory.CreateDirectory(profileDirectoryPah);

                                switch (func)
                                {
                                    case "read":

                                        if (File.Exists(profilePath))
                                        {
                                            const string xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n";

                                            output = File.ReadAllText(profilePath);

                                            // Cleans up old xml data produced by previous versions of the API, while preserving token amount.
                                            if (output.StartsWith(xmlHeader))
                                            {
                                                Match match = Regex.Match(output, tokensRegex);
                                                if (match.Success && int.TryParse(match.Groups[1].Value, out int currentTokenAmount))
                                                {
                                                    output = Regex.Replace(UFCData, @"<tokens>\d+</tokens>", $"<tokens>{currentTokenAmount}</tokens>");
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

                                            string val2 = data.GetParameterValue("val2");

                                            if (File.Exists(profilePath))
                                            {
                                                output = File.ReadAllText(profilePath);

                                                if (Regex.Match(output, tokensRegex).Success)
                                                    output = Regex.Replace(output, $@"{tokenElement}\d+{tokenElementTerm}", $"{tokenElement}{val2}{tokenElementTerm}");
                                                else // Invalid data.
                                                {
                                                    output = null;
                                                    break;
                                                }
                                            }
                                            else
                                                output = UFCData.Replace($"{tokenElement}{defaultTokenAmount}{tokenElementTerm}", $"{tokenElement}{val2}{tokenElementTerm}");

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
                                            string subfunc = data.GetParameterValue("subfunc");

                                            switch (subfunc)
                                            {
                                                case "addcard":
                                                case "add2cards":

                                                    try
                                                    {
                                                        int cardnum = (int)double.Parse(data.GetParameterValue("cardnum"), CultureInfo.InvariantCulture);
                                                        string elementName;
                                                        XElement xml;

                                                        if (File.Exists(profilePath))
                                                            xml = XElement.Parse($"{rootXmlElement}{File.ReadAllText(profilePath)}{rootXmlElementTerm}");
                                                        else
                                                            xml = XElement.Parse($"{rootXmlElement}{UFCData}{rootXmlElementTerm}");

                                                        if (cardnum < 10)
                                                            elementName = card00 + cardnum;
                                                        else if (cardnum < 100)
                                                            elementName = card0 + cardnum;
                                                        else
                                                            elementName = card + cardnum;

                                                        XElement parserElement = xml.Element(elementName);

                                                        if (parserElement != null && int.TryParse(parserElement.Value, out int numOfCardObtained))
                                                            parserElement.Value = (numOfCardObtained + (subfunc == "add2cards" ? 2 : 1)).ToString();

                                                        try
                                                        {
                                                            elementName = "fb01";

                                                            string fb01 = data.GetParameterValue(elementName);

                                                            if (!string.IsNullOrEmpty(fb01))
                                                            {
                                                                parserElement = xml.Element(elementName);

                                                                if (parserElement != null)
                                                                    parserElement.Value = fb01;
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            // Not every requests has this field.
                                                        }

                                                        output = xml.ToString().Replace(rootXmlElement, string.Empty).Replace(rootXmlElementTerm, string.Empty)
                                                            .Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty);
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
                                                        int points = (int)double.Parse(data.GetParameterValue("points"), CultureInfo.InvariantCulture);
                                                        int numsets = (int)double.Parse(data.GetParameterValue("numsets"), CultureInfo.InvariantCulture);
                                                        int i;
                                                        string elementName;
                                                        XElement xml;

                                                        if (File.Exists(profilePath))
                                                            xml = XElement.Parse($"{rootXmlElement}{File.ReadAllText(profilePath)}{rootXmlElementTerm}");
                                                        else
                                                            xml = XElement.Parse($"{rootXmlElement}{UFCData}{rootXmlElementTerm}");

                                                        XElement parserElement;

                                                        parserElement = xml.Element("tokens");

                                                        if (parserElement != null && int.TryParse(parserElement.Value, out int currentTokenAmount))
                                                            parserElement.Value = (currentTokenAmount + points).ToString();

                                                        parserElement = xml.Element("books");

                                                        if (parserElement != null && int.TryParse(parserElement.Value, out int numOfSoldBooks))
                                                            parserElement.Value = (numOfSoldBooks + 1).ToString();

                                                        for (i = 1; i <= numsets; i++)
                                                        {
                                                            if (i < 10)
                                                                elementName = "set0" + i;
                                                            else
                                                                elementName = "set" + i;

                                                            parserElement = xml.Element(elementName);

                                                            if (parserElement != null && int.TryParse(parserElement.Value, out int numOfSoldSet))
                                                                parserElement.Value = (numOfSoldSet - 1).ToString();
                                                        }

                                                        output = xml.ToString().Replace(rootXmlElement, string.Empty).Replace(rootXmlElementTerm, string.Empty)
                                                            .Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty);
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
                                                        int points = (int)double.Parse(data.GetParameterValue("points"), CultureInfo.InvariantCulture);
                                                        int setnum = (int)double.Parse(data.GetParameterValue("setnum"), CultureInfo.InvariantCulture);
                                                        int[] cards = data.GetParameterValue("cards").Split('-').Select(cardx => (int)double.Parse(cardx, CultureInfo.InvariantCulture)).ToArray();

                                                        string elementName;
                                                        XElement xml;

                                                        if (File.Exists(profilePath))
                                                            xml = XElement.Parse($"{rootXmlElement}{File.ReadAllText(profilePath)}{rootXmlElementTerm}");
                                                        else
                                                            xml = XElement.Parse($"{rootXmlElement}{UFCData}{rootXmlElementTerm}");

                                                        XElement parserElement;

                                                        parserElement = xml.Element("tokens");

                                                        if (parserElement != null && int.TryParse(parserElement.Value, out int currentTokenAmount))
                                                            parserElement.Value = (currentTokenAmount + points).ToString();

                                                        if (setnum < 10)
                                                            elementName = "set0" + setnum;
                                                        else
                                                            elementName = "set" + setnum;

                                                        parserElement = xml.Element(elementName);

                                                        if (parserElement != null && int.TryParse(parserElement.Value, out int numOfSoldSet))
                                                            parserElement.Value = (numOfSoldSet + 1).ToString();

                                                        foreach (int cardIter in cards)
                                                        {
                                                            if (cardIter < 10)
                                                                elementName = card00 + cardIter;
                                                            else if (cardIter < 100)
                                                                elementName = card0 + cardIter;
                                                            else
                                                                elementName = card + cardIter;

                                                            parserElement = xml.Element(elementName);

                                                            if (parserElement != null && int.TryParse(parserElement.Value, out int numOfCardObtained))
                                                                parserElement.Value = (numOfCardObtained - 1).ToString();
                                                        }

                                                        output = xml.ToString().Replace(rootXmlElement, string.Empty).Replace(rootXmlElementTerm, string.Empty)
                                                            .Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty);
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
                                                        int cardnum = (int)double.Parse(data.GetParameterValue("cardnum"), CultureInfo.InvariantCulture);
                                                        string otherid = data.GetParameterValue("otherid");

                                                        _ = Task.Run(() => {
                                                            string otherProfileDirectoryPath = $"{apiPath}/HOME_THQ/{otherid}/";
                                                            string otherProfilePath = $"{otherProfileDirectoryPath}data.xml";
                                                            string elementName1;
                                                            XElement xml1;

                                                            Directory.CreateDirectory(otherProfileDirectoryPath);

                                                            if (File.Exists(otherProfilePath))
                                                                xml1 = XElement.Parse($"{rootXmlElement}{otherProfilePath}{rootXmlElementTerm}");
                                                            else
                                                                xml1 = XElement.Parse($"{rootXmlElement}{UFCData}{rootXmlElementTerm}");

                                                            XElement parserElement1;

                                                            if (cardnum < 10)
                                                                elementName1 = card00 + cardnum;
                                                            else if (cardnum < 100)
                                                                elementName1 = card0 + cardnum;
                                                            else
                                                                elementName1 = card + cardnum;

                                                            parserElement1 = xml1.Element(elementName1);

                                                            if (parserElement1 != null && int.TryParse(parserElement1.Value, out int numOfCardObtained1))
                                                                parserElement1.Value = (numOfCardObtained1 + (subfunc == "gift2cards" ? 2 : 1)).ToString();

                                                            File.WriteAllText(otherProfilePath, xml1.ToString().Replace(rootXmlElement, string.Empty).Replace(rootXmlElementTerm, string.Empty)
                                                            .Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty));
                                                        });

                                                        string elementName;
                                                        XElement xml;

                                                        if (File.Exists(profilePath))
                                                            xml = XElement.Parse($"{rootXmlElement}{File.ReadAllText(profilePath)}{rootXmlElementTerm}");
                                                        else
                                                            xml = XElement.Parse($"{rootXmlElement}{UFCData}{rootXmlElementTerm}");

                                                        XElement parserElement;

                                                        if (cardnum < 10)
                                                            elementName = card00 + cardnum;
                                                        else if (cardnum < 100)
                                                            elementName = card0 + cardnum;
                                                        else
                                                            elementName = card + cardnum;

                                                        parserElement = xml.Element(elementName);

                                                        if (parserElement != null && int.TryParse(parserElement.Value, out int numOfCardObtained))
                                                            parserElement.Value = (numOfCardObtained - 1).ToString();

                                                        output = xml.ToString().Replace(rootXmlElement, string.Empty).Replace(rootXmlElementTerm, string.Empty)
                                                            .Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty);
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
                    CustomLogger.LoggerAccessor.LogError($"[UFC2010PsHomeClass] - ProcessUFCUserData thrown an assertion. (Exception: {ex})");
					
                    output = null;
                }
            }

            return output;
        }

        private static string GenerateDefaultData()
        {
            int i;
            string elementName;

            StringBuilder st = new StringBuilder($"<UFC>2</UFC><tokens>{defaultTokenAmount}</tokens><books>0</books>");

            for (i = 1; i <= 10; i++)
            {
                if (i < 10)
                    elementName = "set0" + i;
                else
                    elementName = "set" + i;

                st.Append($"<{elementName}>0</{elementName}>");
            }

            for (i = 1; i <= 102; i++)
            {
                if (i < 10)
                    elementName = "card00" + i;
                else if (i < 100)
                    elementName = "card0" + i;
                else
                    elementName = "card" + i;

                st.Append($"<{elementName}>0</{elementName}>");
            }

            st.Append($"<fb01>{defaultWrittenDate}</fb01>");

            return st.ToString();
        }
    }
}
