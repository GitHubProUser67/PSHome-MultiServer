using CustomLogger;
using NetworkLibrary.Extension;
using System;

namespace HomeTools.PS3_Creator
{
    class AppLoader {

        private Decryptor dec;
        private Hash hash;
        private bool hashDebug = false;

        public bool doAll(int hashFlag, bool v4, int cryptoFlag, byte[] i, int inOffset, byte[] o, int outOffset, int len, byte[] key, byte[] iv, byte[] hash, byte[] expectedHash, int hashOffset) 
        {
            doInit(hashFlag, v4, cryptoFlag, key, iv, hash);
            doUpdate(i, inOffset, o, outOffset, len);
            return doFinal(expectedHash, hashOffset);
        }

        public void doInit(int hashFlag, bool v4, int cryptoFlag, byte[] key, byte[] iv, byte[] hashKey) {
            byte[] calculatedKey = new byte[key.Length];
            byte[] calculatedIV = new byte[iv.Length];
            byte[] calculatedHash = new byte[hashKey.Length];
            getCryptoKeys(cryptoFlag, v4, calculatedKey, calculatedIV, key, iv);
            getHashKeys(hashFlag, v4, calculatedHash, hashKey);
            setDecryptor(cryptoFlag);
            setHash(hashFlag);
#if DEBUG
            LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - ERK:  " + calculatedKey.ToHexString());
            LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - IV:   " + calculatedIV.ToHexString());
            LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - HASH: " + calculatedHash.ToHexString());
#endif
            dec.doInit(calculatedKey, calculatedIV);
            hash.doInit(calculatedHash);
        }

        public void doUpdate(byte[] i, int inOffset, byte[] o, int outOffset, int len) {
            hash.doUpdate(i, inOffset, len);
            dec.doUpdate(i, inOffset, o, outOffset, len);
        }

        public bool doFinal(byte[] expectedhash, int hashOffset) {
            return hash.doFinal(expectedhash, hashOffset, hashDebug);
        }

        public bool doFinalButGetHash(byte[] generatedHash)
        {
            return hash.doFinalButGetHash(generatedHash);
        }

        private void getCryptoKeys(int cryptoFlag, bool v4, byte[] calculatedKey, byte[] calculatedIV, byte[] key, byte[] iv) {
            uint mode = (uint) cryptoFlag & 0xF0000000;
            switch (mode) {
                case 0x10000000:
                    ToolsImpl.aescbcDecrypt(v4 ? EDATKeys.EDATKEY1 : EDATKeys.EDATKEY0, EDATKeys.EDATIV, key, 0, calculatedKey, 0, calculatedKey.Length);
                    ConversionUtils.arraycopy(iv, 0, calculatedIV, 0, calculatedIV.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - MODE: Encrypted ERK");
#endif
                    break;
                case 0x20000000:
                    ConversionUtils.arraycopy(v4 ? EDATKeys.EDATKEY1 : EDATKeys.EDATKEY0, 0, calculatedKey, 0, calculatedKey.Length);
                    ConversionUtils.arraycopy(EDATKeys.EDATIV, 0, calculatedIV, 0, calculatedIV.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - MODE: Default ERK");
#endif
                    break;
                case 0x00000000:
                    ConversionUtils.arraycopy(key, 0, calculatedKey, 0, calculatedKey.Length);
                    ConversionUtils.arraycopy(iv, 0, calculatedIV, 0, calculatedIV.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - MODE: Unencrypted ERK");
#endif
                    break;
                default:
                    throw new Exception("Crypto mode is not valid: Undefined keys calculator");
            }
        }

        private void getHashKeys(int hashFlag, bool v4, byte[] calculatedHash, byte[] hash) {
            uint mode = (uint) hashFlag & 0xF0000000;
            switch (mode) {
                case 0x10000000:
                    ToolsImpl.aescbcDecrypt(v4 ? EDATKeys.EDATKEY1 : EDATKeys.EDATKEY0, EDATKeys.EDATIV, hash, 0, calculatedHash, 0, calculatedHash.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - MODE: Encrypted HASHKEY");
#endif
                    break;
                case 0x20000000:
                    ConversionUtils.arraycopy(v4 ? EDATKeys.EDATHASH1 : EDATKeys.EDATHASH0, 0, calculatedHash, 0, calculatedHash.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - MODE: Default HASHKEY");
#endif
                    break;
                case 0x00000000:
                    ConversionUtils.arraycopy(hash, 0, calculatedHash, 0, calculatedHash.Length);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - MODE: Unencrypted HASHKEY");
#endif
                    break;
                default:
                    throw new Exception("Hash mode is not valid: Undefined keys calculator");
            }
        }

        private void setDecryptor(int cryptoFlag) {
            int aux = cryptoFlag & 0xFF;
            switch (aux) {
                case 0x01:
                    dec = new NoCrypt();
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - MODE: Decryption Algorithm NONE");
#endif
                    break;
                case 0x02:
                    dec = new AESCBC128Decrypt();
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - MODE: Decryption Algorithm AESCBC128");
#endif
                    break;
                default:
                    throw new Exception("Crypto mode is not valid: Undefined decryptor");

            }
//            if ((cryptoFlag & 0x0F000000) != 0) cryptoDebug = true;

        }

        private void setHash(int hashFlag) {
            int aux = hashFlag & 0xFF;
            switch (aux) {
                case 0x01:
                    hash = new HMAC();
                    hash.setHashLen(0x14);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - MODE: Hash HMAC Len 0x14");
#endif
                    break;
                case 0x02:
                    hash = new CMAC();
                    hash.setHashLen(0x10);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - MODE: Hash CMAC Len 0x10");
#endif
                    break;
                case 0x04:
                    hash = new HMAC();
                    hash.setHashLen(0x10);
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - AppLoader - MODE: Hash HMAC Len 0x10");
#endif
                    break;
                default:
                    throw new Exception("Hash mode is not valid: Undefined hash algorithm");
            }
            if ((hashFlag & 0x0F000000) != 0) hashDebug = true;
        }

    }
}
