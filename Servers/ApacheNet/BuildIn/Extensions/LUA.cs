using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using NLua;
using WatsonWebserver.Core;
using WebAPIService.GameServices.OHS;

namespace ApacheNet.BuildIn.Extensions
{
    public class LUA
    {
        public static (string?, object?) ProcessLUAPage(string FilePath, HttpContextBase ctx)
        {
            byte[]? postData = ctx.Request.DataAsBytes;
            NameValueCollection queryElements = ctx.Request.Query.Elements;

            object[] results = LuaUtils.ExecuteLuaScriptFile(FilePath, ctx.Request.Query.Elements, Convert.ToBase64String(ctx.Request.DataAsBytes));

            if (results.Length > 1)
            {
                int i = 0;
                List<byte> output = new List<byte>();

                foreach (object result in results)
                {
                    switch (result)
                    {
                        case bool bVal:
                            output.AddRange(Encoding.UTF8.GetBytes($"{i}:," + bVal.ToString().ToLower()));
                            break;

                        case string s:
                            output.AddRange(Encoding.UTF8.GetBytes($"{i}:," + s));
                            break;

                        case byte[] b:
                            output.AddRange(Encoding.UTF8.GetBytes($"{i}:," + Convert.ToBase64String(b)));
                            break;

                        case LuaTable table:
                            output.AddRange(Encoding.UTF8.GetBytes($"{i}:," + Newtonsoft.Json.JsonConvert.SerializeObject(LuaUtils.ConvertLuaTableToDictionary(table))));
                            break;

                        default:
                            output.AddRange(Encoding.UTF8.GetBytes($"{i}:," + result.ToString()));
                            break;
                    }
                }

                return ("text/plain", output.ToArray());
            }
            else if (results.Length > 0)
            {
                object result = results[0];

                switch (result)
                {
                    case bool bVal:
                        return ("text/plain", bVal.ToString().ToLower());

                    case string s:
                        string trimmed = s.TrimStart();
                        return (trimmed.StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase) ||
                                   trimmed.StartsWith("<html", StringComparison.OrdinalIgnoreCase)
                            ? "text/html"
                            : "text/plain", s);

                    case byte[] b:
                        return ("application/octet-stream", b);

                    case LuaTable table:
                        return ("application/json", Newtonsoft.Json.JsonConvert.SerializeObject(LuaUtils.ConvertLuaTableToDictionary(table)));

                    default:
                        return ("text/plain", result.ToString());
                }
            }

            return (null, null);
        }
    }
}
