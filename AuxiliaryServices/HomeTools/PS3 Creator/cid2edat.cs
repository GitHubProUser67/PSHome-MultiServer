using System;
using System.Text;
using System.IO;
using System.Reflection;
using MultiServerLibrary.Extension;

namespace HomeTools.PS3_Creator
{
    public class cid2edat
    {
        public string outFile;
        public static byte[] pad = {
        (byte) 0x47, (byte) 0x4F, (byte) 0x4D, (byte) 0x41,
        (byte) 0x00, (byte) 0x01, (byte) 0x00, (byte) 0x00,
        (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
        (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00
        };

        public string makeedat(String inFile)
        {
            String strAppDir = Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().GetName().CodeBase);
            string path = strAppDir.Replace("file:\\", "");
            byte[] CID = Encoding.UTF8.GetBytes(inFile);
            byte[] contentID = new byte[48];
            byte[] contentID2 = new byte[inFile.Length];
            ConversionUtils.Arraycopy(CID, 0, contentID, 0, CID.Length);
            ConversionUtils.Arraycopy(CID, 0, contentID2, 0, inFile.Length);
            string cid3 = Encoding.UTF8.GetString(contentID2);
            if (path != null) 
            {
                outFile = path + "/edats/" + inFile + ".edat";
            }
            else { outFile = "edats/" + inFile + ".edat";
            }

            FileStream dat = File.Open(inFile + ".dat", FileMode.Create);
            dat.Write(pad, 0, 0x10);
            dat.Write(contentID, 0, contentID.Length);
            dat.Close();
            String input = inFile + ".dat";
            DirectoryInfo di = Directory.CreateDirectory(path + "/edats");
            byte[] flags = "0C".HexStringToByteArray();
            byte[] type = "00".HexStringToByteArray();
            byte[] version = "02".HexStringToByteArray();
            byte[] devKLic = "72F990788F9CFF745725F08E4C128387".HexStringToByteArray();
            byte[] keyFromRif = null;
            EDAT instance = new EDAT();
            instance.encryptFile(input, outFile, devKLic, keyFromRif, contentID, flags, type, version);
            if (File.Exists(inFile + ".dat"))
            {
                File.Delete(inFile + ".dat");
            }

            return inFile + ".edat";
        }
        }
    }

