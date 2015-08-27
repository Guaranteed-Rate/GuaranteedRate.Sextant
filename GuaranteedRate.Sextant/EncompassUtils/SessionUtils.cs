using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.EncompassUtils
{
    /**
     * Convenience functions
     */
    public class SessionUtils
    {
        public static Session GetEncompassSession(string encompassUrl, string login, string pw)
        {
            Session session = new Session();
            session.Start(encompassUrl, login, pw);
            return session;
        }

        public static Loan OpenLoan(Session session, string guid)
        {
            return session.Loans.Open(guid);
        }
    }
}
