using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.DDL;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.PS3SparkServices
{
    [RMCService((ushort)RMCProtocolId.AccountManagementService)]
    public class AccountManagementService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult CreateAccount(string strPrincipalName, string strKey, uint uiGroups, string strEmail)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(2)]
        public RMCResult DeleteAccount()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(3)]
        public RMCResult DisableAccount()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(4)]
        public RMCResult ChangePassword()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(5)]
        public RMCResult TestCapability()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(6)]
        public RMCResult GetName()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(7)]
        public RMCResult GetAccountData()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(8)]
        public RMCResult GetPrivateData()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(9)]
        public RMCResult GetPublicData(uint PID) // Ghostbusters expect a result all the time.
        {
            return Result(new { retVal = true, oData = new AnyData<GhostbustersPS3Data>() });
        }

        [RMCMethod(10)]
        public RMCResult GetMultiplePublicData()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(11)]
        public RMCResult UpdateAccountName()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(12)]
        public RMCResult UpdateAccountEmail()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(13)]
        public RMCResult UpdateCustomData()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(14)]
        public RMCResult FindByNameRegex()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(15)]
        public RMCResult UpdateAccountExpiryDate()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(16)]
        public RMCResult UpdateAccountEffectiveDate()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(17)]
        public RMCResult UpdateStatus()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(18)]
        public RMCResult GetStatus()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(19)]
        public RMCResult GetLastConnectionStats()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(20)]
        public RMCResult ResetPassword()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(22)]
        public RMCResult RetrieveAccount()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(23)]
        public RMCResult UpdateAccount()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(24)]
        public RMCResult ChangePasswordByGuest()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(25)]
        public RMCResult FindByNameLike()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(26)]
        public RMCResult CustomCreateAccount()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(27)]
        public RMCResult NintendoCreateAccount()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(28)]
        public RMCResult LookupOrCreateAccount()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(29)]
        public RMCResult DisconnectPrincipal()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(30)]
        public RMCResult DisconnectAllPrincipals()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
    }
}
