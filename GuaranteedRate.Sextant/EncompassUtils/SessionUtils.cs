using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using GuaranteedRate.Sextant.Exceptions;
using System;

namespace GuaranteedRate.Sextant.EncompassUtils
{
    /**
     * Convenience functions
     */

    public class SessionUtils
    {
        public static Session GetEncompassSession(string encompassUrl, string login, string pw)
        {
            try
            {
                Session session = new Session();
                session.Start(encompassUrl, login, pw);
                return session;
            }
            catch (ConnectionException ce)
            {
                throw new ServerConnectionException(ce.Message, ce);
            }
            catch (LoginException le)
            {
                // try get error type here, mapped from EM's login error type
                ServerLoginException.ErrorTypes errorType;
                try { errorType = (ServerLoginException.ErrorTypes)(int)le.ErrorType; }
                catch { errorType = ServerLoginException.ErrorTypes.Unspecified; }

                throw new ServerLoginException(le.Message, le, errorType);
            }
            catch
            {
                // unspecified, throw as-is
                throw;
            }
        }

        /// <summary>
        /// Opens provided loan guid using the Session provided
        /// </summary>
        /// <param name="session">Connected session for opening the loan</param>
        /// <param name="guid">Loan identifier</param>
        /// <param name="throwOnError">Indicates whether errors should be thrown or if null is returned when an error occurs</param>
        /// <returns>If success, the Loan object is returned. If unsuccessful and throwOnError is false, then null is returned</returns>
        public static Loan OpenLoan(Session session, string guid, bool throwOnError = false)
        {
            if (!guid.StartsWith("{"))
            {
                guid = "{" + guid;
            }

            if (!guid.EndsWith("}"))
            {
                guid = guid + "}";
            }
            try
            {
                var loan = session.Loans.Open(guid);
                return loan;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (throwOnError) throw;

                return null;
            }
        }

        /// <summary>
        /// Opens provided loan guid using the Session provided
        /// </summary>
        /// <param name="session">Connected session for opening the loan</param>
        /// <param name="guid">Loan identifier</param>
        /// <param name="throwOnError">Indicates whether errors should be thrown or if null is returned when an error occurs</param>
        /// <returns>If success, the Loan object is returned. If unsuccessful and throwOnError is false, then null is returned</returns>
        public static Loan OpenLoan(Session session, Guid guid, bool throwOnError = false)
        {
            return OpenLoan(session, guid.ToString("B"), throwOnError);
        }

        /// <summary>
        /// returns the guid for a given loan_number, using Encompass's search function
        /// </summary>
        /// <param name="session">an active Encompass session</param>
        /// <param name="loan_number">loan number of the loan</param>
        /// <returns>the guid (as a string) or String.Empty if not found.</returns>
        public static string GetGuidForLoanNumber(Session session, string loan_number)
        {
            StringFieldCriterion sfc = new StringFieldCriterion("Loan.LoanNumber", loan_number,
            StringFieldMatchType.Exact, true);
            LoanIdentityList loanList = session.Loans.Query(sfc);
            string guid = String.Empty;

            //Loan.LoanNumber SHOULD be unique, but there is a timing bug
            //If multiple loans found, retun the last one
            foreach (LoanIdentity id in loanList)
            {
                guid = id.Guid;
            }
            return guid;
        }

        public static Loan OpenLoanFromLoanNumber(Session session, string loan_number)
        {
            var guid = GetGuidForLoanNumber(session, loan_number);
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