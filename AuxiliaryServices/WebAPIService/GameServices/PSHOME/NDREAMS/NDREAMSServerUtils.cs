using System.Collections.Generic;
using CustomLogger;
using NLua;
using System.Text;
using System;
using NetHasher;

namespace WebAPIService.GameServices.PSHOME.NDREAMS
{
    public static class NDREAMSServerUtils
    {
        public static string Server_GetSignatureCustom(string signature, string username, string data, DateTime timeObj)
        {
            return DotNetHasher.ComputeSHA1String(Encoding.UTF8.GetBytes(signature + username + timeObj.Year + timeObj.Month + timeObj.Day + data + "Signature")).ToLower();
        }

        public static string Server_GetSignature(string parameter, string username, string data, DateTime timeObj)
        {
            return DotNetHasher.ComputeSHA1String(Encoding.UTF8.GetBytes("nDreams" + parameter + username + timeObj.Year + timeObj.Month + timeObj.Day + data + "Signature")).ToLower();
        }

        public static string Server_KeyToHash(string key, DateTime dObj, string level)
        {
            Dictionary<char, char> map = new Dictionary<char, char>()
            {
                { '0', '#' },
                { '1', 'a' },
                { '2', '@' },
                { '3', 'C' },
                { '4', 'T' },
                { '5', 'U' },
                { '6', 'e' },
                { '7', 'X' },
                { '8', 'k' },
                { '9', 'z' },
                { 'A', '1' },
                { 'B', 'c' },
                { 'C', '*' },
                { 'D', 'v' },
                { 'E', 'D' },
                { 'F', 'A' },
                { 'G', 'g' },
                { 'H', 'U' },
                { 'I', '8' },
                { 'J', '2' }
            };

            StringBuilder str = new StringBuilder();
            for (int i = 7; i >= 0; i--)
            {
                char currentChar = key[i];
                char mappedChar;
                if (map.TryGetValue(currentChar, out char value))
                    mappedChar = value;
                else
                    mappedChar = map[Convert.ToChar(Convert.ToInt32(currentChar))];

                str.Append(mappedChar);
            }

            return DotNetHasher.ComputeSHA1String(Encoding.UTF8.GetBytes("keyString" + level + dObj.Year + dObj.Month + dObj.Day + str.ToString() + level)).ToLower();
        }

        public static string CreateBase64StringFromGuids(List<string> GUIDS)
        {
            if (GUIDS.Count == 0)
                return null;

            StringBuilder sb = new StringBuilder();

            foreach (string guid in GUIDS)
            {
                if (sb.Length == 0)
                    sb.Append($"\"{guid}\"");
                else
                    sb.Append($",\"{guid}\"");
            }

            // Execute the Lua script and get the result
            byte[] LuaTableOutput = ExecuteLuaScript(sb.ToString());

            if (LuaTableOutput != null)
                return Convert.ToBase64String(LuaTableOutput);

            return null;
        }

        public static byte[] ExecuteLuaScript(string GUIDList)
        {
            using (Lua lua = new Lua())
            {
                try
                {
                    // Execute the Lua script
                    object[] returnValues = lua.DoString(@"local list = {PUT_GUID_LIST_HERE};
		                local normal = {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
		                local key1   = {'i','o','p','1','2','8','x','c','5','t','3','v','h','k','q','0'};
		                local key2   = {'a','d','1','g','h','4','y','u','8','o','p','2','5','9','e','i'};
		                local offsets = {0,-1,3,1,-2,4,-5};
		                local oIdx = 1;
		                local dash = string.byte('-');
		                local bytes = {};
		
		                for i=1, #normal do
			                normal[i] = string.byte(normal[i]);
			                key1[i] = string.byte(key1[i]);
			                key2[i] = string.byte(key2[i]);
			                --print(""N: ""..normal[i].."", K1: ""..key1[i].."", K2: ""..key2[i]);
		                end

		                local map1 = {};
		                local map2 = {};

		                for i=1, #normal do
			                map1[normal[i]]=key1[i];
			                map2[normal[i]]=key2[i];
		                end

		                for i=1,#list do
			                -- turn into a array of bytes
			                --print('Encoding: ', list[i]);
			                text = {string.byte(list[i], 1, 35)};
			                -- remove dashes
			                for j=35,1, -1 do
				                if(text[j]==dash) then
					                table.remove(text, j);
				                end
			                end
			                -- swap indeces
			                for j=8, 32, 8 do
				                text[j-7],text[j-6],text[j-5],text[j-4],text[j-3],text[j-2],text[j-1],text[j] = text[j],text[j-1], text[j-2],text[j-3],text[j-4],text[j-5],text[j-6],text[j-7];
			                end
			                -- remap and apply offset
			                for j = 1, 32 do
				                if((j-1)%2==0) then
					                text[j] = map1[text[j]] + offsets[oIdx]; 
				                else
					                text[j] = map2[text[j]] + offsets[oIdx];
				                end
				                oIdx = oIdx + 1;
				                if(oIdx>#offsets) then
					                oIdx = 1;
				                end
			                end
			
			                table.insert(bytes, text)
		                end
				
		                return bytes;".Replace("PUT_GUID_LIST_HERE", GUIDList));

                    // Accessing the returned values
                    if (returnValues != null && returnValues.Length > 0)
                    {
                        // Assuming the Lua script returns a table of byte arrays
                        if (returnValues[0] is LuaTable bytesTable)
                        {
                            List<byte> ReturnBytes = new List<byte>();

                            foreach (LuaTable val in bytesTable.Values)
                            {
                                foreach (long val1 in val.Values)
                                {
                                    ReturnBytes.Add((byte)val1);
                                }
                            }

                            return ReturnBytes.ToArray();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that might occur during script execution
                    LoggerAccessor.LogError("[ExecuteLuaScript] - Error executing Lua script: " + ex);
                }

                lua.Close();
            }

            return null;
        }
    }
}
