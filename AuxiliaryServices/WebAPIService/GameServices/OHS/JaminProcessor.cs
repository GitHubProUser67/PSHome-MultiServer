using System;
using CustomLogger;

namespace WebAPIService.GameServices.OHS
{
    public class JaminProcessor
    {
        public static bool VerifyHash(string str, string referencehash)
        {
            if (EncryptDecrypt.Hash32Str(str).Equals(referencehash, StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        public static (string, string) JaminDeFormatWithWriteKey(string dataforohs, bool hashed, int game, bool unescape = true)
        {
            try
            {
                string writekey = string.Empty;

                // Execute the Lua script and get the result
                object[] returnValues = null;

                if (!string.IsNullOrEmpty(dataforohs))
                {
                    if (unescape)
                        dataforohs = EncryptDecrypt.UnEscape(dataforohs);

                    if (game != 0)
                        dataforohs = EncryptDecrypt.Decrypt(dataforohs, game);

                    if (!string.IsNullOrEmpty(dataforohs))
                    {
#if DEBUG
                        LoggerAccessor.LogInfo($"[OHS] - JaminDeFormatWithWriteKey{(hashed ? " Hashed" : string.Empty)} Assembled Data : {dataforohs}");
#endif
                        if (hashed)
                        {
                            string InData = dataforohs.Substring(8); // We remove the hash.
                            if (VerifyHash(InData, dataforohs.Substring(0, 8)))
                            {
                                writekey = InData.Substring(0, 8);
                                InData = InData.Substring(8); // We remove the writekey.
                                returnValues = LuaUtils.ExecuteLuaScript(LUAJaminCode.LUAJaminDecryptor.Replace("PUT_FORMATEDJAMINVALUE_HERE", LuaUtils.ToLiteral(InData)));
                            }
                        }
                        else
                        {
                            writekey = dataforohs.Substring(0, 8);
                            dataforohs = dataforohs.Substring(8); // We remove the writekey.
                            returnValues = LuaUtils.ExecuteLuaScript(LUAJaminCode.LUAJaminDecryptor.Replace("PUT_FORMATEDJAMINVALUE_HERE", LuaUtils.ToLiteral(dataforohs)));
                        }

                        if (!string.IsNullOrEmpty(returnValues?[0].ToString()))
                        {
                            string endvalue = returnValues[0].ToString();
#if DEBUG
                            LoggerAccessor.LogInfo($"[OHS] - JaminDeFormatWithWriteKey De-Assembled Data : {endvalue}");
#endif
                            return (writekey, endvalue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[OHS] - JaminDeFormatWithWriteKey function JaminDeFormat failed - {ex}");
            }

            return ("11111111", null);
        }

        public static string JaminDeFormat(string dataforohs, bool hashed, int game, bool unescape = true)
        {
            try
            {
                // Execute the Lua script and get the result
                object[] returnValues = null;

                if (!string.IsNullOrEmpty(dataforohs))
                {
                    if (unescape)
                        dataforohs = EncryptDecrypt.UnEscape(dataforohs);

                    if (game != 0)
                        dataforohs = EncryptDecrypt.Decrypt(dataforohs, game);

                    if (!string.IsNullOrEmpty(dataforohs))
                    {
#if DEBUG
                        LoggerAccessor.LogInfo($"[OHS] - JaminDeFormat Assembled{(hashed ? " Hashed" : string.Empty)} Data : {dataforohs}");
#endif
                        if (hashed)
                        {
                            string InData = dataforohs.Substring(8); // We remove the hash.
                            if (VerifyHash(InData, dataforohs.Substring(0, 8)))
                                returnValues = LuaUtils.ExecuteLuaScript(LUAJaminCode.LUAJaminDecryptor.Replace("PUT_FORMATEDJAMINVALUE_HERE", LuaUtils.ToLiteral(InData)));
                        }
                        else
                            returnValues = LuaUtils.ExecuteLuaScript(LUAJaminCode.LUAJaminDecryptor.Replace("PUT_FORMATEDJAMINVALUE_HERE", LuaUtils.ToLiteral(dataforohs)));

                        if (!string.IsNullOrEmpty(returnValues?[0].ToString()))
                        {
                            string endvalue = returnValues[0].ToString();
#if DEBUG
                            LoggerAccessor.LogInfo($"[OHS] - JaminDeFormat De-Assembled Data : {endvalue}");
#endif
                            return endvalue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[OHS] - JaminDeFormat function JaminDeFormat failed - {ex}");
            }

            return null;
        }

        public static string JaminFormat(string dataforohs, int game)
        {
            try
            {

                dataforohs = LuaUtils.HotfixBooleanValuesForLUA(dataforohs); // We lowercase boolean attributes.

#if DEBUG
                LoggerAccessor.LogInfo($"[OHS] - JaminFormat Input Data: {dataforohs}");
#endif
                // Execute the Lua script and get the result
                object[] returnValues = LuaUtils.ExecuteLuaScript(LUAJaminCode.LUAJaminEncryptor.Replace("PUT_TABLEINPUT_HERE", dataforohs));

                string LuaReturn = returnValues[0].ToString();

                if (!string.IsNullOrEmpty(LuaReturn))
                {
#if DEBUG
                    LoggerAccessor.LogInfo($"[OHS] - JaminFormat Assembled Data : {LuaReturn}");
#endif
                    if (game != 0)
                        return EncryptDecrypt.Encrypt(LuaReturn, new Random().Next(1, 9026), game);
                    else
                        return LuaReturn;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[OHS] - JaminFormat function JaminFormat failed - {ex}");
            }

            return null;
        }
    }
}
