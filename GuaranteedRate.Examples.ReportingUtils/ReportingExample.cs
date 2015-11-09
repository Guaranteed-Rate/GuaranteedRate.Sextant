using EllieMae.Encompass.Client;
using GuaranteedRate.Sextant.EncompassUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                DateTime start = DateTime.Parse(args[3]);
                DateTime end = DateTime.Parse(args[4]);
                Console.WriteLine("Superfast query that only returns guids:");
                Stopwatch loansSw = new Stopwatch();
                loansSw.Start();
                loanExtractor.DoExtraction(session, start, end);
                loansSw.Stop();

                Console.WriteLine("Slower pipeline query that returns guids and lastmodified date:");
                Stopwatch loansPl = new Stopwatch();
                loansPl.Start();
                loanExtractor.DoExtractionPipeline(session, start, end);
                loansPl.Stop();

                Console.WriteLine("Slowest report query that returns guids and lastmodified date:");
                Stopwatch loansR = new Stopwatch();
                loansR.Start();
                loanExtractor.DoExtractionReport(session, start, end);
                loansR.Stop();

                session.End();

                Console.WriteLine("LoanId time = " + loansSw.ElapsedMilliseconds);
                Console.WriteLine("LoanPipeline time = " + loansPl.ElapsedMilliseconds);
                Console.WriteLine("LoanReporting time = " + loansR.ElapsedMilliseconds);
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

        public void DoExtractionPipeline(Session session, DateTime startDate, DateTime endDate)
        {
            Console.WriteLine("Finding Loans last modified between " + startDate + " and " + endDate);
            IDictionary<string, DateTime> loanGuids = Reporting.LoansAndLastModifiedPipeline(session, startDate, endDate);
            Console.WriteLine("Found: " + loanGuids.Count);

            foreach (string guid in loanGuids.Keys)
            {
                Console.WriteLine(guid + " last modified " + loanGuids[guid]);
            }
        }

        public void DoExtractionReport(Session session, DateTime startDate, DateTime endDate)
        {
            Console.WriteLine("Finding Loans last modified between " + startDate + " and " + endDate);
            IDictionary<string, DateTime> loanGuids = Reporting.LoansAndLastModifiedReport(session, startDate, endDate);
            Console.WriteLine("Found: " + loanGuids.Count);

            foreach (string guid in loanGuids.Keys)
            {
                Console.WriteLine(guid + " last modified " + loanGuids[guid]);
            }
        }
    }

}
