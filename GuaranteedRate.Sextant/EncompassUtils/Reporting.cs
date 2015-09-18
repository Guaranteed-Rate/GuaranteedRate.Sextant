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
    public class Reporting
    {
        private const string LAST_MODIFIED_FIELD = "LASTMODIFIED";

        /**
         * Returns an IList of loan guids last modified between the start and end date
         * NOTE: There is NO sorting implied in the list
         */
        public static IList<string> LoansLastModifiedBetween(Session session, DateTime start, DateTime end)
        {
            DateFieldCriterion startDate = new DateFieldCriterion();
            startDate.FieldName = LAST_MODIFIED_FIELD;
            startDate.MatchType = OrdinalFieldMatchType.GreaterThanOrEquals;
            startDate.Value = start;
            startDate.Precision = DateFieldMatchPrecision.Day;

            DateFieldCriterion endDate = new DateFieldCriterion();
            endDate.FieldName = LAST_MODIFIED_FIELD;
            endDate.MatchType = OrdinalFieldMatchType.LessThan;
            endDate.Value = end;
            endDate.Precision = DateFieldMatchPrecision.Day;

            QueryCriterion joinCriterion = startDate.And(endDate);
            LoanIdentityList loanList = session.Loans.Query(joinCriterion);
            
            IList<string> loans = new List<string>();

            foreach (LoanIdentity id in loanList) {
                loans.Add(id.Guid);
            }
            return loans;
        }

    }
}
