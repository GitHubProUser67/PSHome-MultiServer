using Horizon.RT.Cryptography;
using Horizon.RT.Cryptography.RSA;
using Horizon.RT.Models;

namespace Horizon.LIBRARY.Pipeline.Attribute
{
    public class ScertClientAttribute
    {
        public static RsaKeyPair DefaultRsaAuthKey = null;

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
                CipherService.SetCipher(CipherContext.RSA_AUTH, (RsaAuthKey ?? DefaultRsaAuthKey).ToPS3());
            }
            else
            {
                CipherService = new CipherService(new PS2CipherFactory());
                CipherService.SetCipher(CipherContext.RSA_AUTH, (RsaAuthKey ?? DefaultRsaAuthKey).ToPS2());
            }
        }
        #endregion
    }
}