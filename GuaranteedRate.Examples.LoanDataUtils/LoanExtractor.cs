using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using GuaranteedRate.Sextant.EncompassUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Examples.LoanDataUtils
{
    public class LoanExtractor
    {
        static int Main(string[] args)
        {
            if (args != null && args.Length == 4)
            {
                LoanExtractor loanExtractor = new LoanExtractor();
                Session session = SessionUtils.GetEncompassSession(args[1], args[2], args[3]);
                FieldUtils.session = session;
                loanExtractor.DoExtraction(session, (args[0]));
                session.End();
                return 1;
            }
            else
            {
                Console.WriteLine(
                    "Usage [Loan Guid or Loan Number] [Encompass url] [Encompass User] [Encompass Password]");
                return 0;
            }
        }

        public LoanExtractor()
        {
        }

        /// <summary>
        /// Method for testing extraction.
        /// </summary>
        /// <param name="session">Active session</param>
        /// <param name="id">Loan ID or Guid</param>
        public void DoExtraction(Session session, string id)
        {
            FieldUtils.AddFieldCollection(FieldUtils.session.Loans.FieldDescriptors.CustomFields);
            FieldUtils.AddFieldCollection(FieldUtils.session.Loans.FieldDescriptors.StandardFields);
            FieldUtils.AddFieldCollection(FieldUtils.session.Loans.FieldDescriptors.VirtualFields);

            Loan loan;
            if (id.StartsWith("{"))
            {
                loan = SessionUtils.OpenLoan(session, id);
            }
            else
            {
                loan = SessionUtils.OpenLoanFromLoanNumber(session, id);
            }

            IDictionary<string, object> loanData =
                GuaranteedRate.Sextant.EncompassUtils.LoanDataUtils.ExtractEverything(loan);
            string json = JsonConvert.SerializeObject(loanData);
            try
            {
                if (loan != null)
                {
                    loan.Close();
                }
            }
            catch (Exception e)
            {
                //Timeout or other error
                Debug.WriteLine("Exception closing loan: " + e);
                Console.WriteLine("Exception closing loan: " + e);
            }

            Debug.WriteLine(json);
            Console.WriteLine(json);
        }
    }
}