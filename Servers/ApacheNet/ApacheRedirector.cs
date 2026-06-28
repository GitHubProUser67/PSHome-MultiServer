using System.Net;
using System.Text.RegularExpressions;
using ApacheNet.Models;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;

namespace ApacheNet
{
    public static partial class ApacheRedirector
    {
        public static Task<bool> RedirectRequest(
            ApacheContext ctx,
            ref string absolutepath,
            ref string fullurl
        )
        {
            if (ApacheNetServerConfiguration.RedirectRules != null)
            {
                foreach (var rule in ApacheNetServerConfiguration.RedirectRules)
                {
                    if (
                        !string.IsNullOrEmpty(rule)
                        && rule.StartsWith("Redirect")
                        && rule.Length >= 9
                    ) // Redirect + whitespace is minimum 9 in length.
                    {
                        var RouteRule = rule.ChopOffBefore("Redirect");

                        if (RouteRule.StartsWith("Match "))
                        {
                            var match = ApacheMatchRegex().Match(RouteRule);
                            if (match.Success && match.Groups.Count >= 3)
                            {
                                // Compare the regex rule against the test URL
                                if (Regex.IsMatch(absolutepath, match.Groups[2].Value))
                                {
                                    var extractedStatusCode = (HttpStatusCode)
                                        int.Parse(match.Groups[1].Value);
                                    if (extractedStatusCode == HttpStatusCode.OK)
                                    {
                                        absolutepath = match.Groups[3].Value;
                                        if (!absolutepath.StartsWith("/"))
                                            absolutepath = "/" + absolutepath;
                                        fullurl =
                                            absolutepath
                                            + HTTPProcessor.ProcessQueryString(fullurl, true);
                                    }
                                    else
                                    {
                                        ctx.StatusCode = extractedStatusCode;
                                        ctx.Response.Headers.Add("Location", match.Groups[3].Value);
                                        return ctx.SendImmediate();
                                    }
                                }
                            }
                        }
                        else if (RouteRule.StartsWith("Permanent "))
                        {
                            var parts = RouteRule.Split(
                                [' '],
                                StringSplitOptions.RemoveEmptyEntries
                            );

                            if (parts.Length == 3 && (parts[1] == "/" || parts[1] == absolutepath))
                            {
                                ctx.StatusCode = HttpStatusCode.PermanentRedirect;
                                ctx.Response.Headers.Add("Location", parts[2]);
                                return ctx.SendImmediate();
                            }
                        }
                        else if (RouteRule.StartsWith(' '))
                        {
                            RouteRule = RouteRule[1..];
                            var parts = RouteRule.Split(
                                [' '],
                                StringSplitOptions.RemoveEmptyEntries
                            );

                            if (
                                parts.Length >= 4
                                && "Match".Equals(parts[0])
                                && int.TryParse(parts[1], out _)
                            )
                            {
                                var match = ApacheMatchRegex().Match(RouteRule);
                                if (match.Success && match.Groups.Count >= 3)
                                {
                                    // Compare the regex rule against the test URL
                                    if (Regex.IsMatch(absolutepath, match.Groups[2].Value))
                                    {
                                        var extractedStatusCode = (HttpStatusCode)
                                            int.Parse(match.Groups[1].Value);
                                        if (extractedStatusCode == HttpStatusCode.OK)
                                        {
                                            absolutepath = match.Groups[3].Value;
                                            if (!absolutepath.StartsWith("/"))
                                                absolutepath = "/" + absolutepath;
                                            fullurl =
                                                absolutepath
                                                + HTTPProcessor.ProcessQueryString(fullurl, true);
                                        }
                                        else
                                        {
                                            ctx.StatusCode = extractedStatusCode;
                                            ctx.Response.Headers.Add(
                                                "Location",
                                                match.Groups[3].Value
                                            );
                                            return ctx.SendImmediate();
                                        }
                                    }
                                }
                            }
                            else if (
                                parts.Length == 3
                                && (parts[1] == "/" || parts[1] == absolutepath)
                            )
                            {
                                // Check if the input string contains an HTTP method
                                if (
                                    HttpMethodRegex().Match(parts[0]).Success
                                    && ctx.Request.Method.ToString() == parts[0]
                                )
                                {
                                    ctx.StatusCode = HttpStatusCode.Found;
                                    ctx.Response.Headers.Add("Location", parts[2]);
                                    return ctx.SendImmediate();
                                }
                                // Check if the input string contains a status code
                                else if (
                                    HttpStatusCodeRegex().Match(parts[0]).Success
                                    && int.TryParse(parts[0], out var statuscode)
                                )
                                {
                                    ctx.StatusCode = (HttpStatusCode)statuscode;
                                    ctx.Response.Headers.Add("Location", parts[2]);
                                    return ctx.SendImmediate();
                                }
                                else if (
                                    "permanent".Equals(
                                        parts[0],
                                        StringComparison.InvariantCultureIgnoreCase
                                    )
                                )
                                {
                                    ctx.StatusCode = HttpStatusCode.PermanentRedirect;
                                    ctx.Response.Headers.Add("Location", parts[2]);
                                    return ctx.SendImmediate();
                                }
                            }
                            else if (
                                parts.Length == 2
                                && (parts[0] == "/" || parts[0] == absolutepath)
                            )
                            {
                                ctx.StatusCode = HttpStatusCode.Found;
                                ctx.Response.Headers.Add("Location", parts[1]);
                                return ctx.SendImmediate();
                            }
                        }
                    }
                }
            }

            return Task.FromResult(false);
        }

        [GeneratedRegex(@"Match (\d{3}) (\S+) (\S+)$")]
        private static partial Regex ApacheMatchRegex();

        [GeneratedRegex("^(GET|POST|PUT|DELETE|HEAD|OPTIONS|PATCH)")]
        private static partial Regex HttpMethodRegex();

        [GeneratedRegex("\\b\\d{3}\\b")]
        private static partial Regex HttpStatusCodeRegex();
    }
}
