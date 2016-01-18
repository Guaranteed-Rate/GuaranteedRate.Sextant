using EllieMae.Encompass.BusinessObjects.Users;
using EllieMae.Encompass.Client;
using GuaranteedRate.Sextant.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.EncompassUtils
{
    public static class UserUtils
    {
        public static ICollection<User> GetAllUsers(this Session session)
        {
            ICollection<User> users = new List<User>();

            foreach (User user in session.Users.GetAllUsers())
            {
                users.Add(user);
            }

            return users;
        }

        public static ICollection<User> GetAllActiveUsers(this Session session)
        {
            return GetAllUsers(session).Where(x => x.Enabled && !x.AccountLocked).ToList();
        }

        public static ICollection<User> GetAllUsersInWorkFolder(this Session session, string workFolder)
        {
            return GetAllUsers(session).Where(x => x.WorkingFolder.ToUpper().Replace(" ", "") == workFolder.ToUpper().Replace(" ", "")).ToList();
        }

        public static bool TryForceCompanyWidePasswordChange(this Session session, ICollection<string> exceptionUserIds, out string exception)
        {
            try
            {
                ICollection<User> targetUsers = GetAllActiveUsers(session).Where(x => !exceptionUserIds.Contains(x.ID)).ToList();
                foreach (User user in targetUsers)
                {
                    user.RequirePasswordChange = true;
                    user.Commit();
                }

                exception = "";
                return true;
            }

            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public static string ToJson(this ICollection<User> users)
        {
            ICollection<EncompassUser> myUsers = new List<EncompassUser>();
            foreach(User u in users)
            {
                myUsers.Add(new EncompassUser(u));
            }

            string json = JsonConvert.SerializeObject(myUsers);
            return json;
        }

        public static string ToJson(this User user)
        {
            string json = JsonConvert.SerializeObject(new EncompassUser(user));
            return json;
        }
    }
}
