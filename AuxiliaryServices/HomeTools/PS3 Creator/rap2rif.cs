using System;
using System.Text;
using System.IO;
using System.Reflection;
using MultiServerLibrary.Extension;

namespace HomeTools.PS3_Creator
{
    public class rap2rif
    {
        public string outFile;

        private static byte[] rif_header = {
	    (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x01, 
        (byte) 0x00, (byte) 0x01, (byte) 0x00, (byte) 0x02, 
        //(byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00, 
        //(byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x02
    };
        private static byte[] rif_footer = {
	    (byte) 0x00, (byte) 0x00, (byte) 0x01, (byte) 0x2F, 
        (byte) 0x41, (byte) 0x5C, (byte) 0x00, (byte) 0x00, 
        (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00, 
        (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00
    };
        private static byte[] rif_junk = {
        (byte) 0x11                       
    };
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
        //Test Field : 
        private static byte[] RIFKEY = {
         (byte) 0x65, (byte) 0xF4, (byte) 0xC9, (byte) 0x57,
         (byte) 0xC8, (byte) 0x4F, (byte) 0x8D, (byte) 0x02, 
         (byte) 0xDD, (byte) 0x4C, (byte) 0x60, (byte) 0x21,
         (byte) 0x1C, (byte) 0x43, (byte) 0x35, (byte) 0xCB
     };
        static byte[] ACTDAT_KEY = { 
        (byte)0x5E, (byte)0x06, (byte)0xE0, (byte)0x4F, 
        (byte)0xD9, (byte)0x4A, (byte)0x71, (byte)0xBF, 
        (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, 
        (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x01 
     };

        public byte[] getKey(String rapFile)
        {
            BinaryReader raf = new BinaryReader(File.OpenRead(rapFile));
            byte[] read = raf.ReadBytes(0x10);
            raf.Close();
            byte[] intermediate = new byte[read.Length];
            ToolsImpl.AesecbDecrypt(rapKey, read, 0, intermediate, 0, read.Length);
            for (int loop = 0; loop < 5; loop++)
            {

                for (int loop2 = 0; loop2 < 0x10; loop2++)
                {
                    int index = indexTable[loop2];
                    intermediate[index] = (byte)(intermediate[index] ^ key1[index]);
                }
                for (int loop2 = 0xF; loop2 > 0; loop2--)
                {
                    int index1 = indexTable[loop2];
                    int index2 = indexTable[loop2 - 1];
                    intermediate[index1] = (byte)(intermediate[index1] ^ intermediate[index2]);
                }
                int acum = 0;
                for (int loop2 = 0; loop2 < 0x10; loop2++)
                {
                    int index = indexTable[loop2];
                    byte current = (byte)(intermediate[index] - acum);
                    intermediate[index] = current;
                    if (acum != 1 || current != 0xFF)
                    {
                        int aux1 = current & 0xFF;
                        int aux2 = key2[index] & 0xFF;
                        acum = (aux1 < aux2) ? 1 : 0;
                    }
                    current = (byte)(current - key2[index]);
                    intermediate[index] = current;
                }
            }
            return intermediate;
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

        public string makerif(String inFile, String outFile)
        {
            if (!File.Exists(inFile))
            {
                CustomLogger.LoggerAccessor.LogWarn("[PS3 Creator] - rap2rif - " + inFile + " not found");
                return inFile;
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

                FileStream actFile = File.Open(actIn, FileMode.Open);
                byte[] actid = new byte[0x08];
                actFile.Seek(0x08, SeekOrigin.Begin);
                actFile.Read(actid, 0, 0x8);
                actFile.Close();

                string cid = "\\".GetSubstringByString(".", inFile);

                if (path != null)
                {
                    outFile = path + "/rifs/" + cid + ".rif";
                }
                else
                {
                    outFile = "rifs/" + cid + ".rif";
                }

                byte[] content_id = new byte[0x30];
                byte[] encrif0x40 = new byte[0x10];
                byte[] rif0x40 = new byte[0x10];
                byte[] rif0x50 = null;

                byte[] keyFromRif = getKey(inFile);
                DirectoryInfo di = Directory.CreateDirectory(path + "/rifs");

                FileStream o = File.Open(outFile, FileMode.Create);

                o.Write(rif_header, 0, 0x08);
                o.Write(actid, 0, 0x08);
                byte[] CID = Encoding.UTF8.GetBytes(cid);
                ConversionUtils.Arraycopy(CID, 0, content_id, 0, CID.Length);

                o.Write(content_id, 0, 0x30);

                ToolsImpl.AesecbEncrypt(RIFKEY, rif0x40, 0x00, encrif0x40, 0, 0x10);
                o.Write(encrif0x40, 0, 0x10);
                long index = 0;
                byte[] actDat = decryptACTDAT(actIn, idps);
                byte[] datKey = new byte[0x10];
                rif0x50 = new byte[0x10];
                byte[] signature = new byte[0x28];

                ConversionUtils.Arraycopy(actDat, (int)index * 16, datKey, 0, 0x10);
                ToolsImpl.AesecbEncrypt(datKey, keyFromRif, 0, rif0x50, 0, 0x10);
                o.Write(rif0x50, 0, 0x10);
                o.Write(rif_footer, 0, 0x10);

                while (o.Length < 0x98)
                    o.Write(rif_junk, 0, 0x1);

                o.Close();
                return outFile;
            }
        }
    }
}
