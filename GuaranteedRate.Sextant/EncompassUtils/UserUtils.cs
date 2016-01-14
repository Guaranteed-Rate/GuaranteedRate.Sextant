using EllieMae.Encompass.BusinessObjects.Users;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.EncompassUtils
{
    public static class UserUtils
    {
        public static List<User> GetAllUsers(this Session session)
        {
            List<User> users = new List<User>();

            foreach( User user in session.Users.GetAllUsers())
            {
                users.Add(user);
            }

            return users;
        }

        public static List<User> GetAllActiveUsers(this Session session)
        {
            return GetAllUsers(session).Where(x => x.Enabled && !x.AccountLocked).ToList();
        }

        public static List<User> GetAllUsersInWorkFolder(this Session session, string workFolder)
        {
            return GetAllUsers(session).Where(x => x.WorkingFolder.ToUpper().Replace(" ","") == workFolder.ToUpper().Replace(" ","")).ToList();
        }

        public static bool TryForceCompanyWidePasswordChange(this Session session, List<string> exceptionUserIds, out string exception)
        {
            try
            {
                List<User> targetUsers = GetAllActiveUsers(session).Where(x => !exceptionUserIds.Contains(x.ID)).ToList();
                foreach (User user in targetUsers)
                {
                    user.RequirePasswordChange = true;
                    user.Commit();
                }

                exception = "";
                return true;
            }

            catch(Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }


    }
}
