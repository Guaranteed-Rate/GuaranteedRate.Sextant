using EllieMae.Encompass.BusinessObjects.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Models
{
    public class EncompassUser
    {
        public bool AccountLocked { get; set; }
        public string CellPhone { get; set; }
        public string CHUMID { get; set; }
        public string Email { get; set; }
        public string EmployeeID { get; set; }
        public bool Enabled { get; set; }
        public string Fax { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string ID { get; set; }
        public bool IsNew { get; set; }
        public string LastName { get; set; }
        public DateTime NMLSExpirationDate { get; set; }
        public string NMLSOriginatorID { get; set; }
        public int OrganizationID { get; set; }
        public string Password { get; set; }
        public PeerLoanAccessRight PeerLoanAccessRight { get; set; }
        public ICollection<string> Personas { get; set; }
        public string Phone { get; set; }
        public bool RequirePasswordChange { get; set; }
        public ICollection<StateLicense> StateLicenses { get; set; }
        public SubordinateLoanAccessRight SubordinateLoanAccessRight { get; set; }

        public string WorkingFolder { get; set; }

        public EncompassUser(User user)
        {
            AccountLocked = user.AccountLocked;
            CellPhone = user.CellPhone;
            CHUMID = user.CHUMID;
            Email = user.Email;
            EmployeeID = user.EmployeeID;
            Enabled = user.Enabled;
            Fax = user.Fax;
            FirstName = user.FirstName;
            FullName = user.FullName;
            ID = user.ID;
            IsNew = user.IsNew;
            LastName = user.LastName;
            NMLSExpirationDate = user.NMLSExpirationDate;
            NMLSOriginatorID = user.NMLSOriginatorID;
            OrganizationID = user.OrganizationID;
            Password = user.Password;
            PeerLoanAccessRight = user.PeerLoanAccessRight;

            Personas = new List<string>();
            foreach (Persona p in user.Personas)
            {
                Personas.Add(p.Name);
            }

            Phone = user.Phone;
            RequirePasswordChange = user.RequirePasswordChange;

            StateLicenses = new List<StateLicense>();
            foreach (StateLicense sl in user.StateLicenses)
            {
                StateLicenses.Add(sl);
            }

            SubordinateLoanAccessRight = user.SubordinateLoanAccessRight;
            WorkingFolder = user.WorkingFolder;
        }
    }
}