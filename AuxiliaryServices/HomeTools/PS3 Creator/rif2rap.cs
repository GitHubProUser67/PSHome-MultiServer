using MultiServerLibrary.Extension;
using System;
using System.IO;
using System.Reflection;

namespace HomeTools.PS3_Creator
{
    class rif2rap
    {
        public string rif;
        int i = 0;
        private static byte[] rapKey = {
        (byte) 0x86, (byte) 0x9F, (byte) 0x77, (byte) 0x45,
        (byte) 0xC1, (byte) 0x3F, (byte) 0xD8, (byte) 0x90,
        (byte) 0xCC, (byte) 0xF2, (byte) 0x91, (byte) 0x88,
        (byte) 0xE3, (byte) 0xCC, (byte) 0x3E, (byte) 0xDF
    };
        private static int[] indexTable = {
        (byte) 0x0C, (byte) 0x03, (byte) 0x06, (byte) 0x04,
        (byte) 0x01, (byte) 0x0B, (byte) 0x0F, (byte) 0x08,
        (byte) 0x02, (byte) 0x07, (byte) 0x00, (byte) 0x05,
        (byte) 0x0A, (byte) 0x0E, (byte) 0x0D, (byte) 0x09
    };
        private static byte[] key1 = {
        (byte) 0xA9, (byte) 0x3E, (byte) 0x1F, (byte) 0xD6,
        (byte) 0x7C, (byte) 0x55, (byte) 0xA3, (byte) 0x29,
        (byte) 0xB7, (byte) 0x5F, (byte) 0xDD, (byte) 0xA6,
        (byte) 0x2A, (byte) 0x95, (byte) 0xC7, (byte) 0xA5
    };
        private static byte[] key2 = {
        (byte) 0x67, (byte) 0xD4, (byte) 0x5D, (byte) 0xA3,
        (byte) 0x29, (byte) 0x6D, (byte) 0x00, (byte) 0x6A,
        (byte) 0x4E, (byte) 0x7C, (byte) 0x53, (byte) 0x7B,
        (byte) 0xF5, (byte) 0x53, (byte) 0x8C, (byte) 0x74
    };
        private static byte[] RIFKEY = {
         (byte) 0xDA, (byte) 0x7D, (byte) 0x4B, (byte) 0x5E,
         (byte) 0x49, (byte) 0x9A, (byte) 0x4F, (byte) 0x53, 
         (byte) 0xB1, (byte) 0xC1, (byte) 0xA1, (byte) 0x4A,
         (byte) 0x74, (byte) 0x84, (byte) 0x44, (byte) 0x3B
     };

        public byte[] getrifKey(String rifIn, String actIn, String idps)
        {
            if (rifIn == null || actIn == null) return null;
            byte[] result = null;
            FileStream rifFile = File.Open(rifIn, FileMode.Open);
            byte[] rif0x40 = new byte[0x10];
            byte[] rif0x50 = new byte[0x10];
            byte[] encrif0x40 = new byte[0x10];
            byte[] encrif0x50 = new byte[0x10];
            rifFile.Seek(0x40, SeekOrigin.Begin);
            rifFile.Read(encrif0x40, 0, encrif0x40.Length);
            rifFile.Read(encrif0x50, 0, encrif0x50.Length);
            rifFile.Close();
            ToolsImpl.AesecbDecrypt(RIFKEY, encrif0x40, 0x00, rif0x40, 0, 0x10);
            long index = ConversionUtils.Be32(rif0x40, 0xC); //
            if (index < 0x80)
            {
                byte[] actDat = decryptACTDAT(actIn, idps);
                byte[] datKey = new byte[0x10];
                result = new byte[0x10];
                ConversionUtils.Arraycopy(actDat, (int)index * 16, datKey, 0, 0x10);
                ToolsImpl.AesecbDecrypt(datKey, encrif0x50, 0, result, 0, 0x10);

            }
            return result;
        }

        private static byte[] decryptACTDAT(String actIn, String IDPSFile)
        {
            FileStream actFile = File.Open(actIn, FileMode.Open);
            byte[] actdat = new byte[0x800];
            byte[] result = new byte[actdat.Length];
            actFile.Seek(0x10, SeekOrigin.Begin);
            actFile.Read(actdat, 0, actdat.Length);
            actFile.Close();
            byte[] key = getPerConsoleKey(IDPSFile);
            ToolsImpl.AesecbDecrypt(key, actdat, 0, result, 0, actdat.Length);
            return result;
        }
        static byte[] ACTDAT_KEY = { (byte)0x5E, (byte)0x06, (byte)0xE0, (byte)0x4F, (byte)0xD9, (byte)0x4A, (byte)0x71, (byte)0xBF, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x01 };

        private static byte[] getPerConsoleKey(String IDPSFile)
        {
            FileStream raf = File.Open(IDPSFile, FileMode.Open);
            byte[] idps = new byte[0x10];
            raf.Read(idps, 0, idps.Length);
            raf.Close();
            byte[] result = new byte[0x10];
            ToolsImpl.AesecbEncrypt(idps, ACTDAT_KEY, 0, result, 0, ACTDAT_KEY.Length);
            return result;
        }

        public string makerap(String rifIn)
        {
            if (!File.Exists(rifIn))
            {
                CustomLogger.LoggerAccessor.LogWarn("[PS3 Creator] - rif2rap - " + rifIn + " not found");
                return rifIn;
            }
            else
            {
                String strAppDir = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().GetName().CodeBase);
                string path = strAppDir.Replace("file:\\", "");
                string actIn1 = strAppDir + "/data/act.dat";
                string idps1 = strAppDir + "/data/idps";
                string actIn = actIn1.Replace("file:\\", "");
                string idps = idps1.Replace("file:\\", "");
                byte[] intermediate = getrifKey(rifIn, actIn, idps);

                for (int loop = 0; loop < 5; loop++)
                {
                    int acum = 0;
                    int acum2 = 0;
                    for (int loop2 = 0; loop2 < 0x10; loop2++)
                    {
                        int index = indexTable[loop2];

                        byte current = (byte)(intermediate[index] + acum + acum2);
                        if (acum != 1 || current != 0xFF)
                        {
                            int aux1 = (current + key2[index]) & 0xFF;
                            int aux2 = key2[index] & 0xFF;
                            int aux3 = (intermediate[index]);
                            acum = (aux1 < aux2) ? 1 : 0;
                            acum2 = (aux3 == 0xFF) ? 1 : 0;
                            intermediate[index] = (byte)(current + key2[index]);
                        }
                        else if (current == 0xFF)
                        { intermediate[index] = (byte)(current + key2[index]); }
                        else
                        { intermediate[index] = current; }

                    }
                    for (int loop2 = 1; loop2 < 0x10; loop2++)
                    {
                        int index1 = indexTable[loop2];
                        int index2 = indexTable[loop2 - 1];
                        intermediate[index1] = (byte)(intermediate[index2] ^ intermediate[index1]);

                    }
                    for (int loop2 = 0; loop2 < 0x10; loop2++)
                    {
                        int index = indexTable[loop2];
                        intermediate[index] = (byte)(key1[index] ^ intermediate[index]);
                    }
                }
                string cid = "\\".GetSubstringByString(".", rifIn);

                string outFile = null;
                if (path != null)
                {
                    outFile = path + "/raps/" + cid + ".rap";
                }
                else
                {
                    outFile = "raps/" + cid + ".rap";
                }

                byte[] rk = new byte[0x10];
                ToolsImpl.AesecbEncrypt(rapKey, intermediate, 0, rk, 0, 0x10);
                DirectoryInfo di = Directory.CreateDirectory(path + "/raps");
                FileStream o = File.Open(outFile, FileMode.Create);
                o.Write(rk, 0, 0x10);
                o.Close();

                while (i == 0)
                {
                    string outFile2 = "test.rif";
                    raptest instance = new raptest();
                    rif = instance.makerif(outFile, outFile2);
                    i++;
                }
                
                byte[] first = getrifKey(rifIn, actIn, idps);
                byte[] second = getrifKey(rif, actIn, idps);
                if (first[0] == second[0] && first[1] == second[1] && first[2] == second[2] && first[3] == second[3] && first[4] == second[4] && first[5] == second[5] && first[6] == second[6] && first[7] == second[7] && first[8] == second[8] && first[9] == second[9] && first[10] == second[10] && first[11] == second[11] && first[12] == second[12] && first[13] == second[13] && first[14] == second[14] && first[15] == second[15])
                {
                    return outFile;
                }
                else { getKey(rifIn); }

                if (Directory.Exists("temp"))
                {
                    Directory.Delete("temp", true);
                }
                return outFile;
            }
        }

        public string getKey(String rifIn)
        {
            if (!File.Exists(rifIn))
            {
                CustomLogger.LoggerAccessor.LogWarn("[PS3 Creator] - rif2rap - " + rifIn + " not found");
                return rifIn;
            }
            else
            {
                String strAppDir = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().GetName().CodeBase);
                string path = strAppDir.Replace("file:\\", "");
                string actIn1 = strAppDir + "/data/act.dat";
                string idps1 = strAppDir + "/data/idps";
                string actIn = actIn1.Replace("file:\\", "");
                string idps = idps1.Replace("file:\\", "");

                byte[] intermediate = getrifKey(rifIn, actIn, idps);
                for (int loop = 0; loop < 5; loop++)
                {
                    int acum = 0;
                    for (int loop2 = 0; loop2 < 0x10; loop2++)
                    {
                        int index = indexTable[loop2];
                        byte current = (byte)(intermediate[index] + acum);
                        if (acum != 1 || current != 0xFF)
                        {
                            int aux1 = (current + key2[index]) & 0xFF;
                            int aux2 = key2[index] & 0xFF;
                            acum = (aux1 < aux2) ? 1 : 0;
                            intermediate[index] = (byte)(current + key2[index]);
                        }
                        else if (current == 0xFF)
                        { intermediate[index] = (byte)(current + key2[index]); }
                        else
                        { intermediate[index] = current; }
                    }
                    for (int loop2 = 1; loop2 < 0x10; loop2++)
                    {
                        int index1 = indexTable[loop2];
                        int index2 = indexTable[loop2 - 1];
                        //intermediate[index1] = (byte)(intermediate[index1] ^ intermediate[index2]);
                        intermediate[index1] = (byte)(intermediate[index2] ^ intermediate[index1]);
                    }
                    for (int loop2 = 0; loop2 < 0x10; loop2++)
                    {
                        int index = indexTable[loop2];
                        // intermediate[index] = (byte)(intermediate[index] ^ key1[index]);
                        intermediate[index] = (byte)(key1[index] ^ intermediate[index]);
                    }
                }
                string cid = "\\".GetSubstringByString(".", rifIn);
                string outFile = null;
                if (path != null)
                {
                    outFile = path + "/raps/" + cid + ".rap";
                }
                else
                {
                    outFile = "raps/" + cid + ".rap";
                }
                byte[] rk = new byte[0x10];
                ToolsImpl.AesecbEncrypt(rapKey, intermediate, 0, rk, 0, 0x10);
                DirectoryInfo di = Directory.CreateDirectory("raps");
                FileStream o = File.Open(outFile, FileMode.Create);
                o.Write(rk, 0, 0x10);

                o.Close();
                if (Directory.Exists("temp"))
                {
                    Directory.Delete("temp", true);
                }
                
                return outFile;
            }
        }
    }
}


