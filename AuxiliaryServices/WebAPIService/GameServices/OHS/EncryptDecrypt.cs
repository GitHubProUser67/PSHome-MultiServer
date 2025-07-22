using CustomLogger;
using System;
using System.Text;

namespace WebAPIService.GameServices.OHS
{
    public class EncryptDecrypt
    {
        private const string LUAHttpDecryptor = @"local cipher = PUT_KEYVALS_HERE

        function wrapped( index, max )
            if index > max then
                return 1 + (index % (max+1))
            else
                return index
            end
        end

        function encrypt( str, offset )
            assert(offset <= 95 * 95)
            local chars = {}

            local offsethi = math.floor((offset - 1) / 95)
            local offsetlo = ((offset - 1) % 95)
            -- print(""offsethi: "", offsethi, "" offsetlo: "", offsetlo)
            table.insert(chars, string.char(offsethi + 32))
            table.insert(chars, string.char(offsetlo + 32))

            -- print(""encoded offset: "", chars[1], chars[2])

            for i = 1, #str do
                local srcbyte = str:byte(i) - 32
                assert(0 <= srcbyte and srcbyte <= 95)
                local cipherbyte = cipher[wrapped(i + offset, #cipher)]

                local encbyte = ((srcbyte + cipherbyte) % 95) + 32

                -- print(""src: "", srcbyte, ""cipher: "", cipherbyte, "" enc: "", encbyte)

                table.insert(chars, string.char(encbyte))
            end

            return table.concat(chars)
        end

        function decrypt( str )
            local chars = {}

            -- print(""encoded offset: "", str:sub(1, 1), str:sub(2, 2))

            local offsethi = str:byte(1) - 32
            local offsetlo = str:byte(2) - 32
            -- print(""decrypt offsethi: "", offsethi, "" offsetlo: "", offsetlo)

            local offset  = (offsethi * 95) + offsetlo + 1
            -- print(""decrypt offset: "", offset)

            for i = 3, #str do
                local srcbyte = str:byte(i) - 32
                assert(0 <= srcbyte and srcbyte <= 95)
                local cipherbyte = cipher[wrapped((i-2) + offset, #cipher)]

                local decbyte = ((srcbyte - cipherbyte) % 95) + 32

                table.insert(chars, string.char(decbyte))
            end

            return table.concat(chars)
        end

        return decrypt(PUT_ENCRYPTEDVALUE_HERE)";

        private const string LUAHttpEncryptor = @"local cipher = PUT_KEYVALS_HERE

        function wrapped( index, max )
            if index > max then
                return 1 + (index % (max+1))
            else
                return index
            end
        end

        function encrypt( str, offset )
            assert(offset <= 95 * 95)
            local chars = {}

            local offsethi = math.floor((offset - 1) / 95)
            local offsetlo = ((offset - 1) % 95)
            -- print(""offsethi: "", offsethi, "" offsetlo: "", offsetlo)
            table.insert(chars, string.char(offsethi + 32))
            table.insert(chars, string.char(offsetlo + 32))

            -- print(""encoded offset: "", chars[1], chars[2])

            for i = 1, #str do
                local srcbyte = str:byte(i) - 32
                assert(0 <= srcbyte and srcbyte <= 95)
                local cipherbyte = cipher[wrapped(i + offset, #cipher)]

                local encbyte = ((srcbyte + cipherbyte) % 95) + 32

                -- print(""src: "", srcbyte, ""cipher: "", cipherbyte, "" enc: "", encbyte)

                table.insert(chars, string.char(encbyte))
            end

            return table.concat(chars)
        end

        function decrypt( str )
            local chars = {}

            -- print(""encoded offset: "", str:sub(1, 1), str:sub(2, 2))

            local offsethi = str:byte(1) - 32
            local offsetlo = str:byte(2) - 32
            -- print(""decrypt offsethi: "", offsethi, "" offsetlo: "", offsetlo)

            local offset  = (offsethi * 95) + offsetlo + 1
            -- print(""decrypt offset: "", offset)

            for i = 3, #str do
                local srcbyte = str:byte(i) - 32
                assert(0 <= srcbyte and srcbyte <= 95)
                local cipherbyte = cipher[wrapped((i-2) + offset, #cipher)]

                local decbyte = ((srcbyte - cipherbyte) % 95) + 32

                table.insert(chars, string.char(decbyte))
            end

            return table.concat(chars)
        end

        return encrypt(PUT_DECRYPTEDVALUE_HERE, PUT_OFFSET_HERE)";

        public static string Encrypt(string str, int offset, int game)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (offset > 9025)
                {
                    LoggerAccessor.LogError($"[OHS - Encrypt] - Offset:{offset} is too large.");
                    return null;
                }

                string keyvakStr = null;
                switch (game)
                {
                    case 1:
                        keyvakStr = StaticKeys.version1cipher;
                        break;
                    case 2:
                        keyvakStr = StaticKeys.version2cipher;
                        break;
                    default:
                        LoggerAccessor.LogError($"[OHS - Encrypt] - Game:{game} doesn't have a cipher associated with it.");
                        return null;
                }

                try
                {
                    // Execute the Lua script and get the result
                    return LuaUtils.ExecuteLuaScript(LUAHttpEncryptor.Replace("PUT_DECRYPTEDVALUE_HERE", LuaUtils.ToLiteral(str)).Replace("PUT_OFFSET_HERE", offset.ToString()).Replace("PUT_KEYVALS_HERE", keyvakStr))[0].ToString();
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[OHS - Encrypt] - lua script failed - {ex}");
                }
            }

            return null;
        }

        public static string Decrypt(string str, int game)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string keyvakStr = null;
                switch (game)
                {
                    case 1:
                        keyvakStr = StaticKeys.version1cipher;
                        break;
                    case 2:
                        keyvakStr = StaticKeys.version2cipher;
                        break;
                    default:
                        LoggerAccessor.LogError($"[OHS - Decrypt] - Game:{game} doesn't have a cipher associated with it.");
                        return null;
                }

                try
                {
                    // Execute the Lua script and get the result
                    return LuaUtils.ExecuteLuaScript(LUAHttpDecryptor.Replace("PUT_ENCRYPTEDVALUE_HERE", LuaUtils.ToLiteral(str)).Replace("PUT_KEYVALS_HERE", keyvakStr))[0].ToString();
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[OHS - Decrypt] - lua script failed - {ex}");
                }
            }

            return null;
        }

        private static ushort Hash16(string str, ushort histart, ushort lostart)
        {
            ushort hi = histart, lo = lostart;
            for (int i = 0; i < str.Length; i++)
            {
                byte b = (byte)str[i];
                lo = (ushort)((b + lo) % 255);
                hi = (ushort)((255 - b + hi) % 255);

                ushort lolo = (ushort)(lo % 16);
                ushort lohi = (ushort)(lo / 16);
                ushort hilo = (ushort)(hi % 16);
                ushort hihi = (ushort)(hi / 16);

                lo = (ushort)(hilo * 16 + lolo);
                hi = (ushort)(hihi * 16 + lohi);

                ushort temp = lo;
                lo = hi;
                hi = temp;
            }

            return (ushort)(hi * 255 + lo);
        }

        private static (ushort, ushort) Hash32(string str)
        {
            return (Hash16(str, 170, 204), Hash16(str, 11, 252));
        }

        public static string Hash32Str(string str)
        {
            (ushort hi, ushort lo) = Hash32(str);
            return string.Format("{0:X4}{1:X4}", hi, lo);
        }

        public static string UnEscape(string str)
        {
            // Replace '&a' with '&' and '&m' with '%'
            return str.Replace("&a", "&").Replace("&m", "%");
        }
    }
}
