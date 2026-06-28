using CustomLogger;

namespace WebAPIService.GameServices.PSHOME.OHS
{
    public class JaminProcessor
    {
        public static bool VerifyHash(string str, string referencehash)
        {
            return EncryptDecrypt
                .Hash32Str(str)
                .Equals(referencehash, StringComparison.InvariantCultureIgnoreCase);
        }

        public static (string, string) JaminDeFormatWithWriteKey(
            string dataforohs,
            bool hashed,
            int game,
            bool unescape = true
        )
        {
            try
            {
                var writekey = string.Empty;

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
                        LoggerAccessor.LogInfo(
                            $"[OHS] - JaminDeFormatWithWriteKey{(hashed ? " Hashed" : string.Empty)} Assembled Data : {dataforohs}"
                        );
#endif
                        if (hashed)
                        {
                            var InData = dataforohs[8..]; // We remove the hash.
                            if (VerifyHash(InData, dataforohs[..8]))
                            {
                                writekey = InData[..8];
                                InData = InData[8..]; // We remove the writekey.
                                returnValues = LuaUtils.ExecuteLuaScript(
                                    LUAJaminCode.LUAJaminDecryptor.Replace(
                                        "PUT_FORMATEDJAMINVALUE_HERE",
                                        LuaUtils.ToLiteral(InData)
                                    )
                                );
                            }
                        }
                        else
                        {
                            writekey = dataforohs[..8];
                            dataforohs = dataforohs[8..]; // We remove the writekey.
                            returnValues = LuaUtils.ExecuteLuaScript(
                                LUAJaminCode.LUAJaminDecryptor.Replace(
                                    "PUT_FORMATEDJAMINVALUE_HERE",
                                    LuaUtils.ToLiteral(dataforohs)
                                )
                            );
                        }

                        if (!string.IsNullOrEmpty(returnValues?[0].ToString()))
                        {
                            var endvalue = returnValues[0].ToString();
#if DEBUG
                            LoggerAccessor.LogInfo(
                                $"[OHS] - JaminDeFormatWithWriteKey De-Assembled Data : {endvalue}"
                            );
#endif
                            return (writekey, endvalue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[OHS] - JaminDeFormatWithWriteKey function JaminDeFormat failed - {ex}"
                );
            }

            return ("11111111", null);
        }

        public static string JaminDeFormat(
            string dataforohs,
            bool hashed,
            int game,
            bool unescape = true
        )
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
                        LoggerAccessor.LogInfo(
                            $"[OHS] - JaminDeFormat Assembled{(hashed ? " Hashed" : string.Empty)} Data : {dataforohs}"
                        );
#endif
                        if (hashed)
                        {
                            var InData = dataforohs[8..]; // We remove the hash.
                            if (VerifyHash(InData, dataforohs[..8]))
                                returnValues = LuaUtils.ExecuteLuaScript(
                                    LUAJaminCode.LUAJaminDecryptor.Replace(
                                        "PUT_FORMATEDJAMINVALUE_HERE",
                                        LuaUtils.ToLiteral(InData)
                                    )
                                );
                        }
                        else
                            returnValues = LuaUtils.ExecuteLuaScript(
                                LUAJaminCode.LUAJaminDecryptor.Replace(
                                    "PUT_FORMATEDJAMINVALUE_HERE",
                                    LuaUtils.ToLiteral(dataforohs)
                                )
                            );

                        if (!string.IsNullOrEmpty(returnValues?[0].ToString()))
                        {
                            var endvalue = returnValues[0].ToString();
#if DEBUG
                            LoggerAccessor.LogInfo(
                                $"[OHS] - JaminDeFormat De-Assembled Data : {endvalue}"
                            );
#endif
                            return endvalue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[OHS] - JaminDeFormat function JaminDeFormat failed - {ex}"
                );
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
                var returnValues = LuaUtils.ExecuteLuaScript(
                    LUAJaminCode.LUAJaminEncryptor.Replace(
                        "PUT_TABLEINPUT_HERE",
                        LuaUtils.ToLiteral(dataforohs)
                    )
                );

                var LuaReturn = returnValues[0].ToString();

                if (!string.IsNullOrEmpty(LuaReturn))
                {
#if DEBUG
                    LoggerAccessor.LogInfo($"[OHS] - JaminFormat Assembled Data : {LuaReturn}");
#endif
                    return game != 0
                        ? EncryptDecrypt.Encrypt(LuaReturn, new Random().Next(1, 9026), game)
                        : LuaReturn;
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
