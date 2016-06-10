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

        /// <summary>
        /// Builds a dictionary of state licensing info for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="full"> is a flag that will add a bunch of mostly meaningless fields to the dictionary</param>
        /// <returns></returns>
        public static IList<IDictionary<string, string>> GetStateLicensing(User user, bool full)
        {
            IList<IDictionary<string, string>> licenses = new List<IDictionary<string, string>>();
            foreach (StateLicense l in user.StateLicenses)
            {
                IDictionary<string, string> license = new Dictionary<string, string>();
                license.Add("State", l.State);
                license.Add("LicenseNumber", l.LicenseNumber);
                license.Add("Enabled", l.Enabled + "");
                license.Add("Selected", l.Selected + "");
                license.Add("Exempt", l.Exempt + "");
                license.Add("LicenseStatus", l.LicenseStatus + "");
                license.Add("ExpirationDate", l.ExpirationDate + "");

                //These are usually blank or MAX DATE
                if (full)
                {
                    license.Add("IssueDate", l.IssueDate + "");
                    license.Add("StartDate", l.StartDate + "");
                    license.Add("StatusDate", l.StatusDate + "");
                    license.Add("LastChecked", l.LastChecked + "");
                }
                licenses.Add(license);
            }

            return licenses;
        }

        /// <summary>
        /// Similar to the json user extraction, but this function gives a little more control
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IDictionary<string, object> GetEncompassInfo(User user)
        {
            IDictionary<string, object> encompassInfo = new Dictionary<string, object>();
            encompassInfo.Add("CellPhone", user.CellPhone);
            encompassInfo.Add("CHUMID", user.CHUMID);
            encompassInfo.Add("EmployeeID", user.EmployeeID);
            encompassInfo.Add("Enabled", user.Enabled + "");
            encompassInfo.Add("EncompassId", user.ID);
            encompassInfo.Add("Fax", user.Fax);
            encompassInfo.Add("FirstName", user.FirstName);
            encompassInfo.Add("FullName", user.FullName);
            encompassInfo.Add("IsNew", user.IsNew);
            encompassInfo.Add("LastName", user.LastName);
            encompassInfo.Add("NMLSExpirationDate", user.NMLSExpirationDate);
            encompassInfo.Add("NMLSOriginatorID", user.NMLSOriginatorID);
            encompassInfo.Add("OrganizationID", user.OrganizationID);
            encompassInfo.Add("Phone", user.Phone);
            encompassInfo.Add("PeerLoanAccessRight", user.PeerLoanAccessRight);
            encompassInfo.Add("RequirePasswordChange", user.RequirePasswordChange);
            encompassInfo.Add("SubordinateLoanAccessRight", user.SubordinateLoanAccessRight);
            encompassInfo.Add("WorkingFolder", user.WorkingFolder);
            encompassInfo.Add("EncompassEmail", user.Email);
            encompassInfo.Add("StateLicenses", GetStateLicensing(user, true));

            IList<string> personas = new List<string>();
            foreach (Persona p in user.Personas)
            {
                personas.Add(p.Name);
            }
            encompassInfo.Add("Personas", personas);

            return encompassInfo;
        }

        /// <summary>
        /// Get both state and national licensing info
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IDictionary<string, object> GetLicensingInfo(User user)
        {
            IDictionary<string, object> licensingInfo = new Dictionary<string, object>();
            licensingInfo.Add("NMLSOriginatorID", user.NMLSOriginatorID);
            licensingInfo.Add("StateLicenses", GetStateLicensing(user, false));
            return licensingInfo;
        }

    }
}
