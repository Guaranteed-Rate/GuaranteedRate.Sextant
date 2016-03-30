using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
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

        public static Loan OpenLoanFromLoanNumber(Session session, string loan_number)
        {
            StringFieldCriterion sfc = new StringFieldCriterion("Loan.LoanNumber", loan_number, StringFieldMatchType.Exact, true);
            LoanIdentityList loanList = session.Loans.Query(sfc);
            string guid = null;

            //Loan.LoanNumber SHOULD be unique, but there is a timing bug
            //If multiple loans found, retun the last one
            foreach (LoanIdentity id in loanList)
            {
                guid = id.Guid;
            }
            if (!String.IsNullOrWhiteSpace(guid))
            {
                return session.Loans.Open(guid);
            }
            else
            {
                return null;
            }
        }

    }
}
