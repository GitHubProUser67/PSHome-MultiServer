using Horizon.RT.Cryptography;
using Horizon.RT.Cryptography.RSA;
using Horizon.RT.Models;
using Org.BouncyCastle.Math;

namespace Horizon.LIBRARY.Pipeline.Attribute
{
    public class ScertClientAttribute
    {
        /// <summary>
        /// Key used to authenticate clients.
        /// </summary>
        public static RsaKeyPair DefaultRsaAuthKey { get; set; } =
            new(
                new BigInteger(
                    "10315955513017997681600210131013411322695824559688299373570246338038100843097466504032586443986679280716603540690692615875074465586629501752500179100369237",
                    10
                ),
                new BigInteger("17", 10),
                new BigInteger(
                    "4854567300243763614870687120476899445974505675147434999327174747312047455575182761195687859800492317495944895566174677168271650454805328075020357360662513",
                    10
                )
            );

        public int? MediusVersion { get; set; }
        public int ApplicationID { get; set; }
        public bool IsPS3Client => MediusVersion >= 112;
        public CipherService CipherService { get; set; } = null;
        public RsaKeyPair RsaAuthKey { get; set; } = null;

        public ScertClientAttribute()
        {
            // default
            MediusVersion = 108;
            OnMediusVersionChanged();
        }

        public ScertClientAttribute(int MediusVersion)
        {
            // default
            this.MediusVersion = MediusVersion;
            OnMediusVersionChanged();
        }

        #region OnMessage
        public bool OnMessage(BaseScertMessage message)
        {
            if (message is RT_MSG_CLIENT_HELLO clientHello)
            {
                MediusVersion = clientHello.Parameters[1];
                OnMediusVersionChanged();
                return true;
            }
            else if (message is RT_MSG_CLIENT_CONNECT_TCP clientConnectTcp && MediusVersion == 0)
            {
                ApplicationID = clientConnectTcp.AppId;
                MediusVersion = 108;
                OnMediusVersionChanged();
                return true;
            }

            return false;
        }
        #endregion

        #region OnMediusVersionChanged
        private void OnMediusVersionChanged()
        {
            if (IsPS3Client)
            {
                CipherService = new CipherService(new PS3CipherFactory());
                CipherService.SetCipher(
                    CipherContext.RSA_AUTH,
                    (RsaAuthKey ?? DefaultRsaAuthKey).ToPS3()
                );
            }
            else
            {
                CipherService = new CipherService(new PS2CipherFactory());
                CipherService.SetCipher(
                    CipherContext.RSA_AUTH,
                    (RsaAuthKey ?? DefaultRsaAuthKey).ToPS2()
                );
            }
        }
        #endregion
    }
}
