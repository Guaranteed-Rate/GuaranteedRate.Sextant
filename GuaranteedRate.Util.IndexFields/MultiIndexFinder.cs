using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using GuaranteedRate.Sextant.EncompassUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Util.IndexFields
{
    /**
     * This util lists the index based multi fields where the underlying index is unknown.
     * Iterating through fields with unknown boundry indexes causes 2 problems:
     * 1 - It is slower because time is wasted asking for non-existant fields
     * 2 - Some fields will throw exceptions when invalid indexes are requested.
     * Which is both slow and noisy
     */
    public class MultiIndexFinder
    {
        static int Main(string[] args)
        {
            if (args != null && args.Length == 4)
            {
                MultiIndexFinder finder = new MultiIndexFinder();
                Session session = SessionUtils.GetEncompassSession(args[0], args[1], args[2]);
                finder.GenerateUnknownMultiIndexDetails(session, args[3]);
                session.End();
                return 1;
            }
            else
            {
                Console.WriteLine("Usage [Encompass url] [Encompass User] [Encompass Password] [Loan guid]");
                return 0;
            }
        }

        public MultiIndexFinder()
        {
        }

        private void GenerateUnknownMultiIndexDetails(Session session, string guid)
        {
            GuaranteedRate.Sextant.EncompassUtils.FieldUtils.session = session;
            FieldUtils.AddFieldCollection(FieldUtils.session.Loans.FieldDescriptors.CustomFields);
            FieldUtils.AddFieldCollection(FieldUtils.session.Loans.FieldDescriptors.StandardFields);
            FieldUtils.AddFieldCollection(FieldUtils.session.Loans.FieldDescriptors.VirtualFields);

            Loan loan = SessionUtils.OpenLoan(session, guid);

            ISet<string> unknownMiddle = FieldUtils.MiddleIndexMulti();
            ISet<string> unknownEnd = FieldUtils.EndIndexMulti();

            //Write all the known index values....
            IDictionary<string, int> indexKeySizes = LoanDataUtils.IndexKeySizes(loan);
            foreach (string key in indexKeySizes.Keys)
            {
                Console.WriteLine(key + " == " + indexKeySizes[key]);
            }

            //For each element, itterate and print their values...
            foreach (string field in unknownMiddle.OrderBy(x => x.ToString()))
            {
                FindBoundaries(loan, field);
            }

            if (loan != null)
            {
                loan.Close();
            }
        }

        private void FindBoundaries(Loan loan, string field)
        {
            ISet<string> fieldIds = new HashSet<string>();
            fieldIds.Add(field);
            IDictionary<string, object> results = new Dictionary<string, object>();

            LoanDataUtils.ExtractMiddleIndexFields(loan, fieldIds, results);
            ISet<string> uniques = new HashSet<string>();
            foreach (string key in results.Keys)
            {
                uniques.Add((string)results[key]);
            }
            Console.WriteLine(field + " == [values = " + results.Count + " ][uniques = " + uniques.Count + "]");
        }
    }
}
