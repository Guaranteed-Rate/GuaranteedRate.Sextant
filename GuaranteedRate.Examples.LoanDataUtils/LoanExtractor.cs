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
            if (args.Length == 4)
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
                Console.WriteLine("Usage [Loan Guid] [Encompass url] [Encompass User] [Encompass Password]");
                return 0;
            }
        }

        public LoanExtractor()
        {
        }

        public void DoExtraction(Session session, string guid)
        {
            Loan loan = SessionUtils.OpenLoan(session, guid);
            IDictionary<string, object> fields = GuaranteedRate.Sextant.EncompassUtils.LoanDataUtils.ExtractLoanFields(loan);
            string json = JsonConvert.SerializeObject(fields);
            try
            {
                loan.Close();
            }
            catch (Exception e)
            {
                //Timeout or other error
                Debug.WriteLine("Exception closing loan: " + e);
            }
            Debug.WriteLine(json);
        }
    }
}
