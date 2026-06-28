using System.Data;
using System.Text.RegularExpressions;
using EndianTools;
using MultiServerLibrary.Extension.NET;
using Org.BouncyCastle.Utilities.Encoders;

namespace MultiServerLibrary.Extension
{
    public static partial class StringUtils
    {
        extension(string s)
        {
            public string ChopOffBefore(string Before)
            {
                // Usefull function for chopping up strings
                var end = s.IndexOf(Before, StringComparison.CurrentCultureIgnoreCase);
                return end > -1 ? s[(end + Before.Length)..] : s;
            }

            public string ChopOffAfter(string After)
            {
                // Usefull function for chopping up strings
                var end = s.IndexOf(After, StringComparison.CurrentCultureIgnoreCase);
                return end > -1 ? s[..end] : s;
            }

            public string ReplaceIgnoreCase(string Pattern, string Replacement)
            {
                // using \\$ in the pattern will screw this regex up
                // return Regex.Replace(Source, Pattern, Replacement, RegexOptions.IgnoreCase);

                if (Regex.IsMatch(s, Pattern, RegexOptions.IgnoreCase))
                    s = Regex.Replace(s, Pattern, Replacement, RegexOptions.IgnoreCase);

                return s;
            }

            public string TrimBeforeExtension()
            {
                return Path.GetFileNameWithoutExtension(s).Trim() + Path.GetExtension(s);
            }

            public string GetSubstringByString(string b, string c)
            {
                return c[c.IndexOf(s)..c.IndexOf(b)];
            }

            public string RemoveSuffix(char toRemove) =>
                string.IsNullOrEmpty(s) ? s : (s.EndsWith(toRemove) ? s[..^1] : s);

            public double Eval(string filter = null)
            {
                return Convert.ToDouble(new DataTable().Compute(s, filter));
            }

            /// <summary>
            /// Verify if the string is in base64 format.
            /// <para>Vérifie si un string est en format base64.</para>
            /// </summary>
            /// <param name="s">The base64 string.</param>
            /// <returns>A tuple boolean, byte array.</returns>
            public (bool IsValid, byte[] DecodedBytes) IsBase64()
            {
                if (string.IsNullOrEmpty(s))
                    return (false, null);

                const char equalSign = '=';
                Span<byte> buffer = new byte[
                    (((s.Length * 3) + 3) / 4)
                        - (
                            s.Length > 0 && s[^1] == equalSign
                                ? s.Length > 1 && s[^2] == equalSign
                                    ? 2
                                    : 1
                                : 0
                        )
                ];

                if (Convert.TryFromBase64String(s, buffer, out var bytesWritten))
                    return (true, buffer[..bytesWritten].ToArray());
                var base64CharArray = s.Replace(" ", string.Empty)
                    .Replace("\t", string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty)
                    .ToCharArray();
                try
                {
                    // Fallback to managed implementation (NET's own decoder has issues with python base64 data)
                    var managedDecodeResult = ManagedBase64.Decode(base64CharArray);
                    var hasDecoded = managedDecodeResult.success;
                    if (!hasDecoded)
                    {
                        managedDecodeResult = ManagedBase64.Decode(base64CharArray, true);
                        hasDecoded = managedDecodeResult.success;
                        return (
                            hasDecoded,
                            hasDecoded
                                ? ManagedBase64
                                    .NumberArrayToString(managedDecodeResult.data)
                                    .SelectMany(c =>
                                    {
                                        var charBytes = BitConverter.GetBytes(c);
                                        if (!EndianAwareConverter.isLittleEndianSystem)
                                            Array.Reverse(charBytes);
                                        return charBytes;
                                    })
                                    .Where((_, index) => index % 2 == 0)
                                    .ToArray()
                                : null
                        );
                    }
                    else
                        return (
                            hasDecoded,
                            ManagedBase64
                                .NumberArrayToString(managedDecodeResult.data)
                                .SelectMany(c =>
                                {
                                    var charBytes = BitConverter.GetBytes(c);
                                    if (!EndianAwareConverter.isLittleEndianSystem)
                                        Array.Reverse(charBytes);
                                    return charBytes;
                                })
                                .Where((_, index) => index % 2 == 0)
                                .ToArray()
                        );
                }
                catch { }

                return (false, null);
            }
        }

        public static async Task<string> GenerateRandomBase64KeyAsync()
        {
            const string url = "https://www.digitalsanctuary.com/aes-key-generator-free";
            const string startText = "AES-256 Key:";
            const string endText = "You ";
            string content;

            try
            {
                using (var client = new FixedWebClientWithTimeout())
                    content = await client.DownloadStringTaskAsync(url).ConfigureAwait(false);

                var startIndex = content.IndexOf(startText);

                if (startIndex != -1)
                {
                    startIndex += startText.Length; // Move past the marker text
                    var endIndex = content.IndexOf(endText, startIndex);

                    if (endIndex != -1)
                    {
                        var match = MyRegex()
                            .Match(content.Substring(startIndex, endIndex - startIndex).Trim());

                        if (match.Success)
                            return match.Groups[1].Value.Trim();
                    }
                }

                CustomLogger.LoggerAccessor.LogDebug(
                    $"[StringUtils] - GenerateRandomBase64KeyAsync - website didn't return the expected data, switching to built-in engine..."
                );
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogDebug(
                    $"[StringUtils] - GenerateRandomBase64KeyAsync - an exception was thrown while fetching the key:{ex}, switching to built-in engine..."
                );
            }

            return Base64.ToBase64String(ByteUtils.GenerateRandomBytes(32));
        }

        [GeneratedRegex(@"<strong>(.*?)<\/strong>")]
        private static partial Regex MyRegex();
    }
}
