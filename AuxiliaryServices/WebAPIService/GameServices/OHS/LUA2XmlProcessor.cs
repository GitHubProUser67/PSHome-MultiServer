using CustomLogger;
using System;

namespace WebAPIService.GameServices.OHS
{
    public class LUA2XmlProcessor
    {
        public static string TransformLuaTableToXml(string luaTableStr)
        {
            try
            {
                luaTableStr = LuaUtils.HotfixBooleanValuesForLUA(luaTableStr); // We lowercase boolean attributes.

                // Execute the Lua script and get the result
                object[] returnValues = LuaUtils.ExecuteLuaScript(LUA2XmlCode.lua2xml.Replace("PUT_CODE_HERE", luaTableStr));

                string LuaReturn = returnValues[0].ToString();

                if (!string.IsNullOrEmpty(LuaReturn))
                    return LuaReturn;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[OHS] - LUA2XmlProcessor function TransformLuaTable failed - {ex}");
            }

            return null;
        }
    }
}
