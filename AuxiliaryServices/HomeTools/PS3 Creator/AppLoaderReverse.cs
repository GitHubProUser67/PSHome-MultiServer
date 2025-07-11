using System;
using CustomLogger;
using MultiServerLibrary.Extension;

namespace HomeTools.PS3_Creator
{
    class AppLoaderReverse
    {
        private Decryptor dec;
        private HashGenerator hash;

        public bool doAll(int hashFlag, bool v4, int cryptoFlag, byte[] i, int inOffset, byte[] o, int outOffset, int len, byte[] key, byte[] iv, byte[] hash, byte[] generatedHash, int hashOffset)
        {
            doInit(hashFlag, v4, cryptoFlag, key, iv, hash);
            doUpdate(i, inOffset, o, outOffset, len);
            return doFinal(generatedHash);
        }

        public void doInit(int hashFlag, bool v4, int cryptoFlag, byte[] key, byte[] iv, byte[] hashKey)
        {
            byte[] calculatedKey = new byte[key.Length];
            byte[] calculatedIV = new byte[iv.Length];
            byte[] calculatedHash = new byte[hashKey.Length];
            getCryptoKeys(cryptoFlag, v4, calculatedKey, calculatedIV, key, iv);
            getHashKeys(hashFlag, v4, calculatedHash, hashKey);
            setDecryptor(cryptoFlag);
            setHash(hashFlag);
#if DEBUG
            LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - ERK:  " + calculatedKey.ToHexString());
            LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - IV:   " + calculatedIV.ToHexString());
            LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - HASH: " + calculatedHash.ToHexString());
#endif
            dec.doInit(calculatedKey, calculatedIV);
            hash.doInit(calculatedHash);
        }

        public void doUpdate(byte[] i, int inOffset, byte[] o, int outOffset, int len)
        {
            dec.doUpdate(i, inOffset, o, outOffset, len);
            hash.doUpdate(o, outOffset, len);
        }

        public bool doFinal(byte[] generatedHash)
        {
            return hash.doFinal(generatedHash);
        }

        private void getCryptoKeys(int cryptoFlag, bool v4, byte[] calculatedKey, byte[] calculatedIV, byte[] key, byte[] iv)
        {
            uint mode = (uint)cryptoFlag & 0xF0000000;
            switch (mode)
            {
                case 0x10000000:
                    ToolsImpl.aescbcDecrypt(v4 ? EDATKeys.EDATKEY1 : EDATKeys.EDATKEY0, EDATKeys.EDATIV, key, 0, calculatedKey, 0, calculatedKey.Length);
                    ConversionUtils.arraycopy(iv, 0, calculatedIV, 0, calculatedIV.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Encrypted ERK");
#endif
                    break;
                case 0x20000000:
                    ConversionUtils.arraycopy(v4 ? EDATKeys.EDATKEY1 : EDATKeys.EDATKEY0, 0, calculatedKey, 0, calculatedKey.Length);
                    ConversionUtils.arraycopy(EDATKeys.EDATIV, 0, calculatedIV, 0, calculatedIV.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Default ERK");
#endif
                    break;
                case 0x00000000:
                    ConversionUtils.arraycopy(key, 0, calculatedKey, 0, calculatedKey.Length);
                    ConversionUtils.arraycopy(iv, 0, calculatedIV, 0, calculatedIV.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Unencrypted ERK");
#endif
                    break;
                default:
                    throw new Exception("Crypto mode is not valid: Undefined keys calculator");
            }
        }

        private void getHashKeys(int hashFlag, bool v4, byte[] calculatedHash, byte[] hash)
        {
            uint mode = (uint)hashFlag & 0xF0000000;
            switch (mode)
            {
                case 0x10000000:
                    ToolsImpl.aescbcDecrypt(v4 ? EDATKeys.EDATKEY1 : EDATKeys.EDATKEY0, EDATKeys.EDATIV, hash, 0, calculatedHash, 0, calculatedHash.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Encrypted HASHKEY");
#endif
                    break;
                case 0x20000000:
                    ConversionUtils.arraycopy(v4 ? EDATKeys.EDATHASH1 : EDATKeys.EDATHASH0, 0, calculatedHash, 0, calculatedHash.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Default HASHKEY");
#endif                   
                    break;
                case 0x00000000:
                    ConversionUtils.arraycopy(hash, 0, calculatedHash, 0, calculatedHash.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Unencrypted HASHKEY");
#endif                   
                    break;
                default:
                    throw new Exception("Hash mode is not valid: Undefined keys calculator");
            }
        }

        private void setDecryptor(int cryptoFlag)
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
            //            if ((cryptoFlag & 0x0F000000) != 0) cryptoDebug = true;

        }

        private void setHash(int hashFlag)
        {
            int aux = hashFlag & 0xFF;
            switch (aux)
            {
                case 0x01:
                    hash = new HMACGenerator();
                    hash.setHashLen(0x14);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Hash HMAC Len 0x14");
#endif                    
                    break;
                case 0x02:
                    hash = new CMACGenerator();
                    hash.setHashLen(0x10);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoaderReverse - MODE: Hash CMAC Len 0x10");
#endif
                    break;
                case 0x04:
                    hash = new HMACGenerator();
                    hash.setHashLen(0x10);
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
