using System.Text;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebAPIService.GameServices.PSHOME.OHS
{
    public class UserCounter
    {
        public static string Set(
            byte[] PostData,
            string ContentType,
            string directorypath,
            string batchparams,
            int game
        )
        {
            string dataforohs = null;
            string output = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo(
                            $"[OHS] : Client Version - {data.GetParameterValue("version")}"
                        );
                        dataforohs = JaminProcessor.JaminDeFormat(
                            data.GetParameterValue("data"),
                            true,
                            game
                        );
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;

            object key = null;

            if (!string.IsNullOrEmpty(dataforohs))
            {
                var Token = JToken.Parse(dataforohs);

                var user = JtokenUtils.GetValueFromJToken(Token, "user");

                var value = JtokenUtils.GetValueFromJToken(Token, "value");

                key = JtokenUtils.GetValueFromJToken(Token, "key");

                if (value == null && key == null) // Special object (seen in sodium 2)
                {
                    var firstKeyValuePair = ExtractKeyValues(Token.ToString(), "data")
                        .FirstOrDefault(); // Maybe there can be more?

                    key = firstKeyValuePair.Key;

                    value = firstKeyValuePair.Value;
                }

                Directory.CreateDirectory(directorypath + $"/User_Profiles");

                try
                {
                    var profiledatastring = directorypath + $"/User_Profiles/{user}_Stats.json";

                    if (File.Exists(profiledatastring))
                    {
                        var jObject = JObject.Parse(File.ReadAllText(profiledatastring));

                        if (jObject != null)
                        {
                            // Check if the key name already exists in the JSON
                            var existingKey = jObject
                                .DescendantsAndSelf()
                                .FirstOrDefault(t => t.Path == (string)key);

                            if (existingKey != null && value != null)
                                // Update the value of the existing key
                                existingKey.Replace(JToken.FromObject(value));
                            else
                            {
                                var KeyEntry = jObject["key"];

                                if (KeyEntry != null && value != null && key != null)
                                    // Step 2: Add a new entry to the "Key" object
                                    KeyEntry[key] = JToken.FromObject(value);
                            }

                            File.WriteAllText(
                                profiledatastring,
                                jObject.ToString(Formatting.Indented)
                            );
                        }
                    }
                    else if (key != null)
                    {
                        var keystring = key.ToString();

                        if (!string.IsNullOrEmpty(keystring) && user != null && value != null)
                        {
                            // Create a new profile with the key field
                            var newProfile = new OHSUserProfile
                            {
                                user = user.ToString(),
                                key = new JObject { { keystring, JToken.FromObject(value) } },
                            };

                            File.WriteAllText(
                                profiledatastring,
                                JsonConvert.SerializeObject(newProfile)
                            );
                        }
                    }

                    if (value != null)
                        output = LuaUtils.ConvertJTokenToLuaTable(JToken.FromObject(value), true);
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[UserCounter] - Json Format Error - {ex}");
                }
            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                return string.IsNullOrEmpty(output) ? null : $"{{ [\"{key}\"] = {output} }}";
            }
            else
            {
                dataforohs = string.IsNullOrEmpty(output)
                    ? JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game)
                    : JaminProcessor.JaminFormat(
                        $"{{ [\"status\"] = \"success\", [\"value\"] = {{ [\"{key}\"] = {output} }} }}",
                        game
                    );
            }

            return dataforohs;
        }

        public static string Increment(
            byte[] PostData,
            string ContentType,
            string directorypath,
            string batchparams,
            int game,
            bool v2
        )
        {
            string dataforohs = null;
            (string, string)? output = null;

            if (directorypath.Contains("casino"))
                v2 = true;

            if (string.IsNullOrEmpty(batchparams))
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo(
                            $"[OHS] : Client Version - {data.GetParameterValue("version")}"
                        );
                        dataforohs = JaminProcessor.JaminDeFormat(
                            data.GetParameterValue("data"),
                            true,
                            game
                        );
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;
            if (!string.IsNullOrEmpty(dataforohs))
            {
                // Deserialize the JSON data into a JObject
                var jObject = JsonConvert.DeserializeObject<JObject>(dataforohs);

                if (jObject != null)
                {
                    object key = jObject.Value<string>("key");
                    var user = jObject.Value<string>("user");

                    var value = jObject.Value<int>("value");

                    try
                    {
                        var profileCurDataString =
                            directorypath + $"User_Profiles/{user}_Stats.json";

                        if (File.Exists(profileCurDataString))
                        {
                            var jObjectFromFile = JObject.Parse(
                                File.ReadAllText(profileCurDataString)
                            );

                            if (jObjectFromFile != null)
                            {
                                var existingKey = jObjectFromFile.SelectToken($"$..{key}");

                                if (existingKey != null && existingKey.Type == JTokenType.Integer)
                                    // Increment the value of the existing key (assuming it's an integer)
                                    existingKey.Replace(existingKey.Value<int>() + value);
                                else if (key != null)
                                {
                                    var KeyEntry = jObjectFromFile["key"];

                                    existingKey = value;

                                    KeyEntry?[key] = existingKey;
                                }

                                output = (key?.ToString(), existingKey?.ToString());

                                File.WriteAllText(
                                    profileCurDataString,
                                    jObjectFromFile.ToString(Formatting.Indented)
                                );
                            }
                        }
                        else if (key != null)
                        {
                            var keystring = key.ToString();

                            if (!string.IsNullOrEmpty(keystring) && user != null)
                            {
                                // Create a new profile with the key field and set it to 1
                                var newProfile = new OHSUserProfile
                                {
                                    user = user,
                                    key = new JObject { { keystring, value < 0 ? 0 : value } },
                                };

                                Directory.CreateDirectory(directorypath + $"/User_Profiles");

                                File.WriteAllText(
                                    profileCurDataString,
                                    JsonConvert.SerializeObject(newProfile)
                                );

                                // Set the output to incremented value
                                output = (keystring, value.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError($"[UserCounter] - Json Format Error - {ex}");
                    }
                }
            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                return output == null ? "{ }"
                    : v2 ? $"{{ [\"{output.Value.Item1}\"] = {output.Value.Item2} }}"
                    : output.Value.Item2;
            }
            else
            {
                dataforohs =
                    output == null
                        ? JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game)
                        : JaminProcessor.JaminFormat(
                            $"{{ [\"status\"] = \"success\", [\"value\"] = {(v2 ? $"{{ [\"{output.Value.Item1}\"] = {output.Value.Item2} }}" : output.Value.Item2)} }}",
                            game
                        );
            }

            return dataforohs;
        }

        public static string IncrementSetEntry(
            byte[] PostData,
            string ContentType,
            string directorypath,
            string batchparams,
            int game
        )
        {
            string dataforohs = null;
            string output = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo(
                            $"[OHS] : Client Version - {data.GetParameterValue("version")}"
                        );
                        dataforohs = JaminProcessor.JaminDeFormat(
                            data.GetParameterValue("data"),
                            true,
                            game
                        );
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;

            if (!string.IsNullOrEmpty(dataforohs))
            {
                // Deserialize the JSON data into a JObject
                var jObject = JsonConvert.DeserializeObject<JObject>(dataforohs);

                if (jObject != null)
                {
                    var counter_key = jObject.Value<string>("counter_key");
                    var entry_project = jObject.Value<string>("entry_project");
                    var entry_key = jObject.Value<string>("entry_key");
                    var entry_value = jObject.Value<object>("entry_value");
                    var counter_project = jObject.Value<string>("counter_project");
                    var user = jObject.Value<string>("user");
                    var counter_value = jObject.Value<int>("counter_value");

                    try
                    {
                        var CounterDataStringPath =
                            directorypath + $"/{counter_project}/User_Profiles/{user}_Stats.json";
                        var EntryDataStringPath =
                            directorypath + $"/{entry_project}/User_Profiles/{user}.json";

                        // Step 1 : Update Counter

                        if (File.Exists(CounterDataStringPath))
                        {
                            var jObjectFromFile = JObject.Parse(
                                File.ReadAllText(CounterDataStringPath)
                            );

                            if (jObjectFromFile != null)
                            {
                                var existingKey = jObjectFromFile.SelectToken($"$..{counter_key}");

                                if (existingKey != null && existingKey.Type == JTokenType.Integer)
                                    // Increment the value of the existing key (assuming it's an integer)
                                    existingKey.Replace(existingKey.Value<int>() + counter_value);
                                else if (counter_key != null)
                                {
                                    var KeyEntry = jObjectFromFile["key"];

                                    existingKey = counter_value;

                                    KeyEntry?[counter_key] = existingKey;
                                }

                                output = existingKey?.ToString();

                                File.WriteAllText(
                                    CounterDataStringPath,
                                    jObjectFromFile.ToString(Formatting.Indented)
                                );
                            }
                        }
                        else if (counter_key != null)
                        {
                            var keystring = counter_key;

                            if (!string.IsNullOrEmpty(keystring) && user != null)
                            {
                                // Create a new profile with the key field and set it to 1
                                var newProfile = new OHSUserProfile
                                {
                                    user = user,
                                    key = new JObject
                                    {
                                        { keystring, counter_value < 0 ? 0 : counter_value },
                                    },
                                };

                                Directory.CreateDirectory(
                                    directorypath + $"/{counter_project}/User_Profiles"
                                );

                                File.WriteAllText(
                                    CounterDataStringPath,
                                    JsonConvert.SerializeObject(newProfile)
                                );

                                // Set the output to incremented value
                                output = counter_value.ToString();
                            }
                        }

                        // Step 2 : Update User entry

                        if (File.Exists(EntryDataStringPath))
                        {
                            var profiledata = File.ReadAllText(EntryDataStringPath);

                            if (!string.IsNullOrEmpty(profiledata))
                            {
                                var profilejObject = JObject.Parse(profiledata);

                                if (profilejObject != null)
                                {
                                    // Check if the key name already exists in the JSON
                                    var existingKey = profilejObject
                                        .DescendantsAndSelf()
                                        .FirstOrDefault(t => t.Path == entry_key);

                                    if (existingKey != null && entry_value != null)
                                        // Update the value of the existing key
                                        existingKey.Replace(
                                            entry_value is JToken token
                                                ? token
                                                : JToken.FromObject(entry_value)
                                        );
                                    else if (entry_key != null && entry_value != null)
                                    {
                                        var KeyEntry = profilejObject["key"];

                                        // Add a new entry to the "Key" object
                                        KeyEntry?[entry_key] = entry_value is JToken token
                                            ? token
                                            : JToken.FromObject(entry_value);
                                    }

                                    File.WriteAllText(
                                        EntryDataStringPath,
                                        profilejObject.ToString(Formatting.Indented)
                                    );
                                }
                            }
                        }
                        else if (entry_key != null)
                        {
                            var keystring = entry_key;

                            if (keystring != null && user != null && entry_value != null)
                            {
                                // Create a new profile with the key field
                                var newProfile = new OHSUserProfile
                                {
                                    user = user,
                                    key = new JObject
                                    {
                                        { keystring, JToken.FromObject(entry_value) },
                                    },
                                };

                                Directory.CreateDirectory(
                                    directorypath + $"/{entry_project}/User_Profiles"
                                );

                                File.WriteAllText(
                                    EntryDataStringPath,
                                    JsonConvert.SerializeObject(newProfile)
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError($"[UserCounter] - Json Format Error - {ex}");
                    }
                }
            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                return string.IsNullOrEmpty(output) ? "{ }" : output;
            }
            else
            {
                dataforohs = string.IsNullOrEmpty(output)
                    ? JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game)
                    : JaminProcessor.JaminFormat(
                        $"{{ [\"status\"] = \"success\", [\"value\"] = {output} }}",
                        game
                    );
            }

            return dataforohs;
        }

        public static string Get_All(
            byte[] PostData,
            string ContentType,
            string directorypath,
            string batchparams,
            int game
        )
        {
            string dataforohs = null;
            string output = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo(
                            $"[OHS] : Client Version - {data.GetParameterValue("version")}"
                        );
                        dataforohs = JaminProcessor.JaminDeFormat(
                            data.GetParameterValue("data"),
                            true,
                            game
                        );
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    // Parsing the JSON string
                    var jsonObject = JObject.Parse(dataforohs);

                    // Getting the value of the "user" field
                    dataforohs = (string)jsonObject["user"];

                    if (
                        !string.IsNullOrEmpty(dataforohs)
                        && File.Exists(directorypath + $"/User_Profiles/{dataforohs}_Stats.json")
                    )
                    {
                        var tempreader = File.ReadAllText(
                            directorypath + $"User_Profiles/{dataforohs}_Stats.json"
                        );

                        if (!string.IsNullOrEmpty(tempreader))
                        {
                            // Parse the JSON string to a JObject
                            jsonObject = JObject.Parse(tempreader);

                            // Check if the "key" property exists and if it is an object
                            if (
                                jsonObject.TryGetValue("key", out var keyValueToken)
                                && keyValueToken.Type == JTokenType.Object
                            )
                                // Convert the JToken to a Lua table-like string
                                output = LuaUtils.ConvertJTokenToLuaTable(keyValueToken, true); // Nested, because we expect the array instead.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[UserCounter] - Json Format Error - {ex}");
            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                return string.IsNullOrEmpty(output) ? "{ }" : output;
            }
            else
            {
                dataforohs = string.IsNullOrEmpty(output)
                    ? JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game)
                    : JaminProcessor.JaminFormat(
                        $"{{ [\"status\"] = \"success\", [\"value\"] = {output} }}",
                        game
                    );
            }

            return dataforohs;
        }

        public static string Get_Many(
            byte[] PostData,
            string ContentType,
            string directorypath,
            string batchparams,
            int game
        )
        {
            string dataforohs = null;
            string output = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo(
                            $"[OHS] : Client Version - {data.GetParameterValue("version")}"
                        );
                        dataforohs = JaminProcessor.JaminDeFormat(
                            data.GetParameterValue("data"),
                            true,
                            game
                        );
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    // Parsing the JSON string
                    var jsonObject = JObject.Parse(dataforohs);

                    // Getting the value of the "user" field
                    dataforohs = (string)jsonObject["user"];
                    var keys = jsonObject["keys"]?.ToObject<string[]>();

                    if (
                        keys != null
                        && !string.IsNullOrEmpty(dataforohs)
                        && File.Exists(directorypath + $"/User_Profiles/{dataforohs}_Stats.json")
                    )
                    {
                        var tempreader = File.ReadAllText(
                            directorypath + $"User_Profiles/{dataforohs}_Stats.json"
                        );

                        if (!string.IsNullOrEmpty(tempreader))
                        {
                            var counterSb = new StringBuilder("{");

                            // Parse the JSON string to a JObject
                            jsonObject = JObject.Parse(tempreader);

                            foreach (var key in keys)
                            {
                                // Check if the "key" property exists
                                if (jsonObject.TryGetValue(key, out var keyValueToken))
                                {
                                    var outputOriginal = LuaUtils.ConvertJTokenToLuaTable(
                                        keyValueToken,
                                        false
                                    );

                                    if (counterSb.Length != 1)
                                        counterSb.Append($",[\"{key}\"] = {outputOriginal}");
                                    else
                                        counterSb.Append($"[\"{key}\"] = {outputOriginal}");
                                }
                            }

                            output = counterSb.ToString() + '}';
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[UserCounter] - Json Format Error - {ex}");
            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                return string.IsNullOrEmpty(output) ? "{ }" : output;
            }
            else
            {
                dataforohs = string.IsNullOrEmpty(output)
                    ? JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game)
                    : JaminProcessor.JaminFormat(
                        $"{{ [\"status\"] = \"success\", [\"value\"] = {output} }}",
                        game
                    );
            }

            return dataforohs;
        }

        public static string Get(
            byte[] PostData,
            string ContentType,
            string directorypath,
            string batchparams,
            int game
        )
        {
            string dataforohs = null;
            string output = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo(
                            $"[OHS] : Client Version - {data.GetParameterValue("version")}"
                        );
                        dataforohs = JaminProcessor.JaminDeFormat(
                            data.GetParameterValue("data"),
                            true,
                            game
                        );
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    // Parsing the JSON string
                    var jsonObject = JObject.Parse(dataforohs);

                    // Getting the value of the "user" field
                    dataforohs = (string)jsonObject["user"];
                    var ohsKey = (string)jsonObject["key"];

                    if (
                        !string.IsNullOrEmpty(dataforohs)
                        && File.Exists(directorypath + $"/User_Profiles/{dataforohs}_Stats.json")
                    )
                    {
                        var currencydata = File.ReadAllText(
                            directorypath + $"/User_Profiles/{dataforohs}_Stats.json"
                        );

                        if (!string.IsNullOrEmpty(currencydata))
                        {
                            // Check if the "Key" property exists and if it is an object
                            if (
                                JObject
                                    .Parse(currencydata)
                                    .TryGetValue("key", out var keyValueToken)
                                && keyValueToken.Type == JTokenType.Object
                            )
                            {
                                if (
                                    ((JObject)keyValueToken).TryGetValue(
                                        ohsKey,
                                        out var wishlistToken
                                    )
                                )
                                    output = LuaUtils.ConvertJTokenToLuaTable(wishlistToken, false);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(output))
                        output = "0";
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[UserCounter] - Json Format Error - {ex}");
            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                return string.IsNullOrEmpty(output) ? "{ }" : output;
            }
            else
            {
                dataforohs = string.IsNullOrEmpty(output)
                    ? JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game)
                    : JaminProcessor.JaminFormat(
                        $"{{ [\"status\"] = \"success\", [\"value\"] = {output} }}",
                        game
                    );
            }

            return dataforohs;
        }

        public static string Increment_Many(
            byte[] PostData,
            string ContentType,
            string directorypath,
            string batchparams,
            int game
        )
        {
            string dataforohs = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo(
                            $"[OHS] : Client Version - {data.GetParameterValue("version")}"
                        );
                        dataforohs = JaminProcessor.JaminDeFormat(
                            data.GetParameterValue("data"),
                            true,
                            game
                        );
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;

            Dictionary<string, string> IncrementResults = [];

            if (!string.IsNullOrEmpty(dataforohs))
            {
                // Deserialize the JSON data into a JObject
                var jObject = JsonConvert.DeserializeObject<JObject>(dataforohs);

                if (jObject != null)
                {
                    try
                    {
                        // Getting the value of the "user" field
                        dataforohs = (string)jObject["user"];
                        var keys = jObject["keys"]?.ToObject<string[]>();
                        var projects = jObject["projects"]?.ToObject<string[]>();
                        var values = jObject["values"]?.ToObject<int[]>();

                        if (
                            !string.IsNullOrEmpty(dataforohs)
                            && keys != null
                            && projects != null
                            && values != null
                            && keys.Length == projects.Length
                            && projects.Length == values.Length
                        )
                        {
                            var profileCurDataString = string.Empty;

                            var i = 0;

                            foreach (var project in projects)
                            {
                                profileCurDataString =
                                    directorypath
                                    + $"/{project}/User_Profiles/{dataforohs}_Stats.json";

                                if (File.Exists(profileCurDataString))
                                {
                                    var jObjectFromFile = JObject.Parse(
                                        File.ReadAllText(profileCurDataString)
                                    );

                                    if (jObjectFromFile != null)
                                    {
                                        var existingKey = jObjectFromFile.SelectToken(
                                            $"$..{keys[i]}"
                                        );

                                        if (
                                            existingKey != null
                                            && existingKey.Type == JTokenType.Integer
                                        )
                                            // Increment the value of the existing key (assuming it's an integer)
                                            existingKey.Replace(
                                                existingKey.Value<int>() + values[i]
                                            );
                                        else
                                        {
                                            var KeyEntry = jObjectFromFile["key"];

                                            existingKey = values[i];

                                            KeyEntry?[keys[i]] = existingKey;
                                        }

                                        // Set the output to the incremented value
                                        IncrementResults.Add(keys[i], existingKey.ToString());

                                        File.WriteAllText(
                                            profileCurDataString,
                                            jObjectFromFile.ToString(Formatting.Indented)
                                        );
                                    }
                                }
                                else if (keys[i] != null)
                                {
                                    var keystring = keys[i];

                                    if (
                                        !string.IsNullOrEmpty(keystring)
                                        && !string.IsNullOrEmpty(dataforohs)
                                    )
                                    {
                                        // Create a new profile with the key field and set it to 1
                                        var newProfile = new OHSUserProfile
                                        {
                                            user = dataforohs,
                                            key = new JObject
                                            {
                                                { keystring, values[i] < 0 ? 0 : values[i] },
                                            },
                                        };

                                        Directory.CreateDirectory(
                                            directorypath + $"/{project}/User_Profiles/"
                                        );

                                        File.WriteAllText(
                                            profileCurDataString,
                                            JsonConvert.SerializeObject(newProfile)
                                        );

                                        // Set the output to incremented value
                                        IncrementResults.Add(keys[i], values[i].ToString());
                                    }
                                }

                                i++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError($"[UserCounter] - Json Format Error - {ex}");
                    }
                }
            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                if (IncrementResults.Count <= 0)
                    return "{ }";
                else
                {
                    var sb = new StringBuilder();

                    var i = 1;

                    foreach (var item in IncrementResults)
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append($", [{i}] = {{ [\"value\"] = {item.Value} }}");
                        }
                        else
                            sb.Append($"{{ [{i}] = {{ [\"value\"] = {item.Value} }}");

                        i++;
                    }

                    if (sb.Length != 0)
                        sb.Append(" }");
                    else
                        sb.Append("{ }");

                    return sb.ToString();
                }
            }
            else
            {
                if (IncrementResults.Count <= 0)
                    dataforohs = JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game);
                else
                {
                    var sb = new StringBuilder();

                    var i = 1;

                    foreach (var item in IncrementResults)
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append($", [{i}] = {{ [\"value\"] = {item.Value} }}");
                        }
                        else
                            sb.Append($"{{ [{i}] = {{ [\"value\"] = {item.Value} }}");

                        i++;
                    }

                    if (sb.Length != 0)
                        sb.Append(" }");
                    else
                        sb.Append("{ }");

                    dataforohs = JaminProcessor.JaminFormat(
                        $"{{ [\"status\"] = \"success\", [\"value\"] = {sb} }}",
                        game
                    );
                }
            }

            return dataforohs;
        }

        private static Dictionary<object, object> ExtractKeyValues(
            string jsonString,
            string nameProperty
        )
        {
            var jsonObject = JObject.Parse(jsonString);
            var result = new Dictionary<object, object>();

            if (
                jsonObject.TryGetValue(nameProperty, out var dataToken)
                && dataToken is JObject dataObject
            )
            {
                foreach (var property in dataObject.Properties())
                {
                    result.Add(property.Name, property.Value);
                }
            }

            return result;
        }

        public class OHSUserProfile
        {
            public string user { get; set; }
            public object key { get; set; }
        }
    }
}
