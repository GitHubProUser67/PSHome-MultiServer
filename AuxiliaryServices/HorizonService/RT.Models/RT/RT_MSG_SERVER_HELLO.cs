using Org.BouncyCastle.Math;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System;
using MultiServerLibrary.Extension;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_HELLO)]
    public class RT_MSG_SERVER_HELLO : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_HELLO;

        //PS2
        public ushort protocolVersion = 0x0071;
        public ushort encryptionVersion = 0x0001;

        //PS3 
        public bool MLS = false;
        public byte EncryptFlagDisable = 0x02;

        public byte[] Certificate = "71000006e702308202e3308201cba00302010202140100000000000000000000001100000000000001300d06092a864886f70d0101050500308196310b3009060355040613025553310b3009060355040813024341311230100603550407130953616e20446965676f3131302f060355040a1328534f4e5920436f6d707574657220456e7465727461696e6d656e7420416d657269636120496e632e31143012060355040b130b53434552542047726f7570311d301b06035504031314534345525420526f6f7420417574686f72697479301e170d3035303432363231303133385a170d3335303432353233353935395a308187310b3009060355040613025553310b3009060355040813024341311230100603550407130953616e20446965676f3131302f060355040a1328534f4e5920436f6d707574657220456e7465727461696e6d656e7420416d657269636120496e632e31143012060355040b130b53434552542047726f7570310e300c060355040313054d41532031305c300d06092a864886f70d0101010500034b003048024100c4f75716ec835d2325689f91ff85ed9bfc3211db9c164f41852e264e569d2802008054a0ef459e7e3eabb87fae576e735434d1d124b30b11bd6de098148601550203000011300d06092a864886f70d010105050003820101006c91abeeb59ac01dfbb080646e4df616f833c36a5a448773f7c1acb8ec162ff811ab11f8051294e20754361827b259a534b010dfbb42e56b571ae453779682ca8650ac5dd0b3888cfb16fb858e5c39dff094380bc4f6f0268ade80c22878afc4c16099c64d435a9ab67101e63b0f5336febb1f71683ba0b0ac7eab2ef0d10d9324b6ce5683d1ab359deda17c47f2a253162674be37c2ce185d90c76b7fd7d9983c289747ad10828b385b82d7eb18f52f2eced4c3a65b0dd63dd8c83c5f92203829fdbf1a85c78b869283b0b1d5fe1bb5e85749abf50e46d9decca190c2d954b6e442e58fbde9958af397e9af575e1f76d63f35ee5987406c109db7da50557e8600000000".HexStringToByteArray();
        public byte[] MLSCert = "71000006e702308202e3308201cba00302010202140100000000000000000000001100000000000004300d06092a864886f70d0101050500308196310b3009060355040613025553310b3009060355040813024341311230100603550407130953616e20446965676f3131302f060355040a1328534f4e5920436f6d707574657220456e7465727461696e6d656e7420416d657269636120496e632e31143012060355040b130b53434552542047726f7570311d301b06035504031314534345525420526f6f7420417574686f72697479301e170d3035303432373231303233335a170d3335303432363233353935395a308187310b3009060355040613025553310b3009060355040813024341311230100603550407130953616e20446965676f3131302f060355040a1328534f4e5920436f6d707574657220456e7465727461696e6d656e7420416d657269636120496e632e31143012060355040b130b53434552542047726f7570310e300c060355040313054d4c532030305c300d06092a864886f70d0101010500034b003048024100cf16b818a204ba6db8fc85d866e4f708e6cfa754a5a2399d08eafdfdbbff852d3f1c86944e157dd8f6408d7cd9cfdab409d32fddee05bdde8cff303187b374690203000011300d06092a864886f70d010105050003820101007cc5ccb73e8bffb1888d870279767063a8ea2a619fdd3bbc0b1209b5384853408ec61aafa8b9071f9e41ab93bb56dbcea59ebf18ca113775fd146c3e97fb673db572f849dc906e9f6ee6817cdcf104c4ac4758020ff2443b770d0979fce7cd8807c69ef787e51660e22e35ca19f43da41346ee619d1a707c335684f183ea432c38aaf5dbb277c8527ad98412d7624362d89d52af9f39459db0c5159a8d737262b4a9abdd95b1b8d9d586230bc3cef9fafab68b0fa8e516a89672aa4f0b3956c0a1fbb392b4b7cfca233bee6b83a4b90fe9a8211803f35f3ab83ef81dd2077e185cfc86204adc2538225951a6e9473540d647cbca5d2f6d189644006f2af7d85386dbb9b3d4885887fbb394268da005317eff6eed".HexStringToByteArray();
        public BigInteger RsaPublicKey = BigInteger.Zero;

        public RT_MSG_SERVER_HELLO()
        {

        }

        public override void Deserialize(MessageReader reader)
        {
            protocolVersion = reader.ReadUInt16();
            encryptionVersion = reader.ReadUInt16();
        }

        public override void Serialize(MessageWriter writer)
        {
            //If PS3 Medius Version 112/113, Send PS3 MAS Cert for Encryption/Decryption
            if ((writer.MediusVersion == 112 || writer.MediusVersion == 113) && !MLS)
            {
                // serialize rsa modulus
                // this is sent in server hello at offset 0x194
                // we're going to overwrite the cert at that offset to store the rsa modulus
                var rsakey = RsaPublicKey.ToByteArrayUnsigned();

                // fix to 64 bytes (512 bit)
                Array.Resize(ref rsakey, 0x40);

                // copy to cert
                Array.Copy(rsakey, 0, Certificate, 0x194, rsakey.Length);

                // write
                writer.Write(Certificate);
            }
            //If PS3 Medius Version 112/113, and is MLS, Send PS3 MLS Cert for Encryption/Decryption
            else if ((writer.MediusVersion == 112 || writer.MediusVersion == 113) && MLS)
            {
                // serialize rsa modulus
                // this is sent in server hello at offset 0x194
                // we're going to overwrite the cert at that offset to store the rsa modulus
                var rsakey = RsaPublicKey.ToByteArrayUnsigned();

                // fix to 64 bytes (512 bit)
                Array.Resize(ref rsakey, 0x40);

                // copy to cert
                Array.Copy(rsakey, 0, MLSCert, 0x194, rsakey.Length);

                // write
                writer.Write(MLSCert);
            }
            else //Send PS2 Server Hello
            {
                writer.Write(protocolVersion);
                writer.Write(encryptionVersion);
            }
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"protocolVersion: {protocolVersion} " +
                $"encryptionVersion: {encryptionVersion} " +
                $"rsakey: {RsaPublicKey}";
        }
    }
}
