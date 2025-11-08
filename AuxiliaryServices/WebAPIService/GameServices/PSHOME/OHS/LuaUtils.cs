using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using CustomLogger;
using Newtonsoft.Json.Linq;
using NLua;

namespace WebAPIService.GameServices.PSHOME.OHS
{
    public class LuaUtils
    {
        // Function to convert a JToken to a Lua table-like string
        public static string ConvertJTokenToLuaTable(JToken token, bool nested)
        {
            int arrayIndex = 1;

            if (nested && token.Type == JTokenType.Object)
            {
                StringBuilder resultBuilder = new StringBuilder("{ ");

                foreach (JProperty property in token.Children<JProperty>())
                {
                    if (property.Value.Type == JTokenType.Array)
                    {
                        resultBuilder.Append($"[\"{property.Name}\"] = {{ ");
                        foreach (JToken arrayItem in property.Value)
                        {
                            resultBuilder.Append(ConvertJTokenToLuaTable(arrayItem, true));
                            if (arrayIndex < property.Value.Count())
                                resultBuilder.Append(", ");
                            arrayIndex++;
                        }
                        resultBuilder.Append(" }, ");
                        arrayIndex = 1;
                    }
                    else if (property.Value.Type == JTokenType.Object)
                        resultBuilder.Append($"[\"{property.Name}\"] = {ConvertJTokenToLuaTable(property.Value, true)}, ");
                    else if (property.Value.Type == JTokenType.String)
                        resultBuilder.Append($"[\"{property.Name}\"] = \"{property.Value}\", ");
                    else if (token.Type == JTokenType.Boolean)
                        resultBuilder.Append($"[\"{property.Name}\"] = {property.Value.ToString().ToLower()}, ");
                    else if (token.Type == JTokenType.Null)
                        resultBuilder.Append($"[\"{property.Name}\"] = nil, ");
                    else
                        resultBuilder.Append($"[\"{property.Name}\"] = {property.Value}, ");
                }

                if (resultBuilder.Length > 2)
                    resultBuilder.Length -= 2;

                resultBuilder.Append(" }");

                return resultBuilder.ToString();
            }
            else if (token.Type == JTokenType.Object)
            {
                StringBuilder resultBuilder = new StringBuilder();

                foreach (JProperty property in token.Children<JProperty>())
                {
                    if (property.Value.Type == JTokenType.Array)
                    {
                        resultBuilder.Append("{ ");
                        foreach (JToken arrayItem in property.Value)
                        {
                            resultBuilder.Append(ConvertJTokenToLuaTable(arrayItem, true));
                            if (arrayIndex < property.Value.Count())
                                resultBuilder.Append(", ");
                            arrayIndex++;
                        }
                        resultBuilder.Append(" }, ");
                        arrayIndex = 1;
                    }
                    else if (property.Value.Type == JTokenType.Object)
                        resultBuilder.Append($"{ConvertJTokenToLuaTable(property.Value, true)}, ");
                    else if (property.Value.Type == JTokenType.String)
                        resultBuilder.Append($"\"{property.Value}\", ");
                    else if (token.Type == JTokenType.Boolean)
                        resultBuilder.Append($"{property.Value.ToString().ToLower()}, ");
                    else if (token.Type == JTokenType.Null)
                        resultBuilder.Append("nil, ");
                    else
                        resultBuilder.Append($"{property.Value}, ");
                }

                if (resultBuilder.Length > 2)
                    resultBuilder.Length -= 2;

                return resultBuilder.ToString();
            }
            else if (token.Type == JTokenType.Array)
            {
                StringBuilder resultBuilder = new StringBuilder("{ ");
                foreach (JToken arrayItem in token)
                {
                    resultBuilder.Append(ConvertJTokenToLuaTable(arrayItem, true));
                    resultBuilder.Append(", ");
                }

                if (resultBuilder.Length > 2)
                    resultBuilder.Length -= 2;

                resultBuilder.Append(" }");

                return resultBuilder.ToString();
            }
            else if (token.Type == JTokenType.String)
                return $"\"{token.Value<string>()}\"";
            else if (token.Type == JTokenType.Boolean)
                return token.ToString().ToLower();
            else if (token.Type == JTokenType.Null)
                return "nil";
            else
                return token.ToString(); // For other value types, use their raw string representation
        }

        public static object ConvertLuaTableToDictionary(LuaTable table)
        {
            Dictionary<object, object> dict = new Dictionary<object, object>();
            foreach (var key in table.Keys)
            {
                var value = table[key];
                if (value is LuaTable nestedTable)
                    dict[key] = ConvertLuaTableToDictionary(nestedTable);
                else
                    dict[key] = value;
            }
            return dict;
        }

        // Function to execute a lua script contained within a string.
        public static object[] ExecuteLuaScript(string luaScript, NameValueCollection queryParams = null, string postData = null)
        {
            object[] returnValues = null;

            using (Lua lua = new Lua())
            {
                try
                {
                    if (queryParams != null)
                    {
                        // Set query params as a Lua table
                        lua.DoString("query = {}");
                        LuaTable queryTable = (LuaTable)lua["query"];
                        foreach (string key in queryParams.AllKeys)
                        {
                            queryTable[key] = queryParams[key];
                        }
                    }

                    if (postData != null)
                        lua["postData"] = postData;

                    // Execute the Lua script
                    returnValues = lua.DoString(luaScript);

                    // If the script returns null, return an empty object array
                    if (returnValues == null)
                        returnValues = Array.Empty<object>();
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that might occur during script execution
                    LoggerAccessor.LogError("[ExecuteLuaScriptString] - Error executing Lua script: " + ex);
                    returnValues = Array.Empty<object>();
                }
            }

            return returnValues;
        }

        // Function to execute a lua script stored in a given file (luac compatible).
        public static object[] ExecuteLuaScriptFile(string luaPath, NameValueCollection queryParams = null, string postData = null)
        {
            if (!File.Exists(luaPath))
                return Array.Empty<object>();

            object[] returnValues = null;

            using (Lua lua = new Lua())
            {
                try
                {
                    if (queryParams != null)
                    {
                        // Set query params as a Lua table
                        lua.DoString("query = {}");
                        LuaTable queryTable = (LuaTable)lua["query"];
                        foreach (string key in queryParams.AllKeys)
                        {
                            queryTable[key] = queryParams[key];
                        }
                    }

                    if (postData != null)
                        lua["postData"] = postData;

                    // Execute the Lua script
                    returnValues = lua.LoadFile(luaPath).Call();

                    // If the script returns null, return an empty object array
                    if (returnValues == null)
                        returnValues = Array.Empty<object>();
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that might occur during script execution
                    LoggerAccessor.LogError("[ExecuteLuaScriptFile] - Error executing Lua script: " + ex);
                    returnValues = Array.Empty<object>();
                }
            }

            return returnValues;
        }

        public static string ToLiteral(string input)
        {
            StringBuilder literal = new StringBuilder(input.Length + 2);
            literal.Append('"');
            foreach (char c in input)
            {
                switch (c)
                {
                    case '\"': literal.Append("\\\""); break;
                    case '\\': literal.Append(@"\\"); break;
                    case '\0': literal.Append(@"\0"); break;
                    case '\a': literal.Append(@"\a"); break;
                    case '\b': literal.Append(@"\b"); break;
                    case '\f': literal.Append(@"\f"); break;
                    case '\n': literal.Append(@"\n"); break;
                    case '\r': literal.Append(@"\r"); break;
                    case '\t': literal.Append(@"\t"); break;
                    case '\v': literal.Append(@"\v"); break;
                    default:
                        // ASCII printable character
                        if (c >= 0x20 && c <= 0x7e)
                            literal.Append(c);
                        // As UTF16 escaped character
                        else
                        {
                            literal.Append(@"\u");
                            literal.Append(((int)c).ToString("x4"));
                        }
                        break;
                }
            }
            literal.Append('"');
            return literal.ToString();
        }

        public static string HotfixBooleanValuesForLUA(string luaScript)
        {
            bool inSingleQuoteString = false;
            bool inDoubleQuoteString = false;
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < luaScript.Length; i++)
            {
                char currentChar = luaScript[i];

                // Toggle state if inside string literals
                if (currentChar == '\'' && !inDoubleQuoteString)
                {
                    inSingleQuoteString = !inSingleQuoteString;
                    result.Append(currentChar);
                    continue;
                }
                else if (currentChar == '"' && !inSingleQuoteString)
                {
                    inDoubleQuoteString = !inDoubleQuoteString;
                    result.Append(currentChar);
                    continue;
                }

                // If inside a string, just append the character
                if (inSingleQuoteString || inDoubleQuoteString)
                {
                    result.Append(currentChar);
                    continue;
                }

                // Detect and replace booleans outside of strings
                if (i + 3 < luaScript.Length && luaScript.Substring(i, 4).Equals("True", StringComparison.OrdinalIgnoreCase))
                {
                    result.Append("true");
                    i += 3; // skip the next 3 characters as they are part of "True"
                }
                else if (i + 4 < luaScript.Length && luaScript.Substring(i, 5).Equals("False", StringComparison.OrdinalIgnoreCase))
                {
                    result.Append("false");
                    i += 4; // skip the next 4 characters as they are part of "False"
                }
                else
                    result.Append(currentChar);
            }

            return result.ToString();
        }
    }
}
