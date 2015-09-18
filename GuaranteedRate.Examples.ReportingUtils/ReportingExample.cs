using EllieMae.Encompass.Client;
using GuaranteedRate.Sextant.EncompassUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Examples.ReportingUtils
{
    public class ReportingExample
    {
        static int Main(string[] args)
        {
            if (args!=null && args.Length == 5)
            {
                ReportingExample loanExtractor = new ReportingExample();
                Session session = SessionUtils.GetEncompassSession(args[0], args[1], args[2]);
                loanExtractor.DoExtraction(session, DateTime.Parse(args[3]), DateTime.Parse(args[4]));
                session.End();
                return 1;
            }
            else
            {
                Console.WriteLine("Usage [Encompass url] [Encompass User] [Encompass Password] [Start Date] [End Date]");
                return 0;
            }
        }

        public ReportingExample()
        {
        }

        public void DoExtraction(Session session, DateTime startDate, DateTime endDate)
        {
            Console.WriteLine("Finding Loans last modified between " + startDate + " and " + endDate);
            IList<string> loanGuids = Reporting.LoansLastModifiedBetween(session, startDate, endDate);
            Console.WriteLine("Found: " + loanGuids.Count);
            
            foreach(string guid in loanGuids) {
                Console.Write(guid + ", ");
            }
            Console.WriteLine();
        }
    }

}
