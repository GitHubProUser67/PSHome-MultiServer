using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;
using RDVServices;

namespace QuazalServer.RDVServices.GameServices.PS3GFRSServices
{
    /// <summary>
    /// Secure connection service protocol
    /// </summary>
    [RMCService((ushort)RMCProtocolId.PrivilegesService)]
    public class PrivilegesService : RMCServiceBase
    {
        // TODO, properly handle all the privileges per account.

        [RMCMethod(1)]
        public RMCResult GetPrivileges(string localeCode)
        {
            var privileges = new Dictionary<uint, Privilege>();

            uint id = 1;
            privileges.Add(id++, new Privilege()
            {
                m_ID = 1,
                m_description = "Allow to play online"
            });

            int unlockFlags = 0;
            using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
            {
                var curPlayerId = Context.Client.PlayerInfo.PID;
                var curPlayer = db.Users.FirstOrDefault(x => x.Id == curPlayerId);
                if (curPlayer != null)
                {
                    unlockFlags = curPlayer.RewardFlags;
                }
            }

            if ((unlockFlags & 0x4000) != 0)
            {
                foreach (var priv in DeluxePrivileges.VehicleIds)
                    privileges.Add(id++, priv);
            }

            if ((unlockFlags & 0x8000) != 0)
            {
                foreach (var priv in DeluxePrivileges.ChallengeIds)
                    privileges.Add(id++, priv);
            }

            return Result(privileges);
        }

        [RMCMethod(2)]
        public RMCResult ActivateKey(string uniqueKey, string languageCode)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(3)]
        public RMCResult ActivateKeyWithExpectedPrivileges(string uniqueKey, string languageCode, IEnumerable<uint> expectedPrivilegeIDs)
        {
            var privilegeList = new List<Privilege>();

            using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
            {
                var curPlayerId = Context.Client.PlayerInfo.PID;
                var curPlayer = db.Users.FirstOrDefault(x => x.Id == curPlayerId);
                if (curPlayer != null)
                {
                    int unlockFlags = 0;
                    if (uniqueKey == "IWantDeluxeCars")
                    {
                        privilegeList.AddRange(DeluxePrivileges.VehicleIds);
                        unlockFlags = 0x4000;
                    }
                    else if (uniqueKey == "IWantDeluxeChallenges")
                    {
                        privilegeList.AddRange(DeluxePrivileges.ChallengeIds);
                        unlockFlags = 0x8000;
                    }
                    else
                        return Error(0);

                    if ((curPlayer.RewardFlags & unlockFlags) != 0)
                    {
                        return Error(0);
                    }

                    curPlayer.RewardFlags |= unlockFlags;
                    db.SaveChanges();
                }
                else
                {
                    return Error(2);
                }
            }

            var privilege = new PrivilegeGroup();
            privilege.m_description = uniqueKey + " unlock";
            privilege.m_privileges = privilegeList;

            return Result(privilege);
        }

        [RMCMethod(4)]
        public RMCResult GetPrivilegeRemainDuration(uint privilege_id)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(5)]
        ///<summary>
        /// Get Expired Privileges
        /// </summary>
        public RMCResult GetExpiredPrivileges()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(6)]
        public RMCResult GetPrivilegesEx(string locale_code)
        {
            UNIMPLEMENTED();
            return Error(0);
        }
    }
}
