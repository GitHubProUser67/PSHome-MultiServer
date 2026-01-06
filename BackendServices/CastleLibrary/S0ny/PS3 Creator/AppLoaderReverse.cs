using System;
#if DEBUG
using CastleLibrary.Utils;
#endif
using CustomLogger;

namespace CastleLibrary.S0ny.PS3_Creator
{
    internal class AppLoaderReverse
    {
        private Decryptor dec;
        private HashGenerator hash;

        public bool DoAll(int hashFlag, bool v4, int cryptoFlag, byte[] i, int inOffset, byte[] o, int outOffset, int len, byte[] key, byte[] iv, byte[] hash, byte[] generatedHash, int hashOffset)
        {
            DoInit(hashFlag, v4, cryptoFlag, key, iv, hash);
            DoUpdate(i, inOffset, o, outOffset, len);
            return DoFinal(generatedHash);
        }

        public void DoInit(int hashFlag, bool v4, int cryptoFlag, byte[] key, byte[] iv, byte[] hashKey)
        {
            byte[] calculatedKey = new byte[key.Length];
            byte[] calculatedIV = new byte[iv.Length];
            byte[] calculatedHash = new byte[hashKey.Length];
            GetCryptoKeys(cryptoFlag, v4, calculatedKey, calculatedIV, key, iv);
            GetHashKeys(hashFlag, v4, calculatedHash, hashKey);
            SetDecryptor(cryptoFlag);
            SetHash(hashFlag);
#if DEBUG
            LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - ERK:  " + calculatedKey.BytesToHexStr());
            LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - IV:   " + calculatedIV.BytesToHexStr());
            LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - HASH: " + calculatedHash.BytesToHexStr());
#endif
            dec.DoInit(calculatedKey, calculatedIV);
            hash.DoInit(calculatedHash);
        }

        public void DoUpdate(byte[] i, int inOffset, byte[] o, int outOffset, int len)
        {
            dec.DoUpdate(i, inOffset, o, outOffset, len);
            hash.DoUpdate(o, outOffset, len);
        }

        public bool DoFinal(byte[] generatedHash)
        {
            return hash.DoFinal(generatedHash);
        }

        private static void GetCryptoKeys(int cryptoFlag, bool v4, byte[] calculatedKey, byte[] calculatedIV, byte[] key, byte[] iv)
        {
            uint mode = (uint)cryptoFlag & 0xF0000000;
            switch (mode)
            {
                case 0x10000000:
                    CreatorUtils.AescbcDecrypt(v4 ? EDATKeys.EDATKEY1 : EDATKeys.EDATKEY0, EDATKeys.EDATIV, key, 0, calculatedKey, 0, calculatedKey.Length);
                    ConversionUtils.Arraycopy(iv, 0, calculatedIV, 0, calculatedIV.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Encrypted ERK");
#endif
                    break;
                case 0x20000000:
                    ConversionUtils.Arraycopy(v4 ? EDATKeys.EDATKEY1 : EDATKeys.EDATKEY0, 0, calculatedKey, 0, calculatedKey.Length);
                    ConversionUtils.Arraycopy(EDATKeys.EDATIV, 0, calculatedIV, 0, calculatedIV.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Default ERK");
#endif
                    break;
                case 0x00000000:
                    ConversionUtils.Arraycopy(key, 0, calculatedKey, 0, calculatedKey.Length);
                    ConversionUtils.Arraycopy(iv, 0, calculatedIV, 0, calculatedIV.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Unencrypted ERK");
#endif
                    break;
                default:
                    throw new Exception("Crypto mode is not valid: Undefined keys calculator");
            }
        }

        private static void GetHashKeys(int hashFlag, bool v4, byte[] calculatedHash, byte[] hash)
        {
            uint mode = (uint)hashFlag & 0xF0000000;
            switch (mode)
            {
                case 0x10000000:
                    CreatorUtils.AescbcDecrypt(v4 ? EDATKeys.EDATKEY1 : EDATKeys.EDATKEY0, EDATKeys.EDATIV, hash, 0, calculatedHash, 0, calculatedHash.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Encrypted HASHKEY");
#endif
                    break;
                case 0x20000000:
                    ConversionUtils.Arraycopy(v4 ? EDATKeys.EDATHASH1 : EDATKeys.EDATHASH0, 0, calculatedHash, 0, calculatedHash.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Default HASHKEY");
#endif                   
                    break;
                case 0x00000000:
                    ConversionUtils.Arraycopy(hash, 0, calculatedHash, 0, calculatedHash.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Unencrypted HASHKEY");
#endif                   
                    break;
                default:
                    throw new Exception("Hash mode is not valid: Undefined keys calculator");
            }
        }

        private void SetDecryptor(int cryptoFlag)
        {
            int aux = cryptoFlag & 0xFF;
            switch (aux)
            {
                case 0x01:
                    dec = new NoCrypt();
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Encrypting Algorithm NONE");
#endif                    
                    break;
                case 0x02:
                    dec = new AESCBC128Encrypt();
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Encrypting Algorithm AESCBC128");
#endif
                    break;
                default:
                    throw new Exception("Crypto mode is not valid: Undefined decryptor");

            }
        }

        private void SetHash(int hashFlag)
        {
            int aux = hashFlag & 0xFF;
            switch (aux)
            {
                case 0x01:
                    hash = new HMACGenerator();
                    hash.SetHashLen(0x14);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Hash HMAC Len 0x14");
#endif                    
                    break;
                case 0x02:
                    hash = new CMACGenerator();
                    hash.SetHashLen(0x10);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Hash CMAC Len 0x10");
#endif
                    break;
                case 0x04:
                    hash = new HMACGenerator();
                    hash.SetHashLen(0x10);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Hash HMAC Len 0x10");
#endif
                    break;
                default:
                    throw new Exception("Hash mode is not valid: Undefined hash algorithm");
            }
        }

    }
}
