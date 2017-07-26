using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using GuaranteedRate.Sextant.EncompassUtils;
using System;
using System.Collections.Generic;
using System.Linq;

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

            //Things that maybe are index values
            /*
            Console.WriteLine("Disclosures2015 == " + loan.Log.Disclosures2015.Count);
            Console.WriteLine("DocumentOrders == " + loan.Log.DocumentOrders.Count);
            Console.WriteLine("EDMTransactions == " + loan.Log.EDMTransactions.Count);
            Console.WriteLine("HtmlEmailMessages == " + loan.Log.HtmlEmailMessages.Count);
            Console.WriteLine("InvestorRegistrations == " + loan.Log.InvestorRegistrations.Count);
            Console.WriteLine("LockCancellationRequests == " + loan.Log.LockCancellationRequests.Count);
            Console.WriteLine("LockCancellations == " + loan.Log.LockCancellations.Count);
            Console.WriteLine("LockConfirmations == " + loan.Log.LockConfirmations.Count);
            Console.WriteLine("LockDenials == " + loan.Log.LockDenials.Count);
            Console.WriteLine("LockRequests == " + loan.Log.LockRequests.Count);
            Console.WriteLine("MilestoneEvents == " + loan.Log.MilestoneEvents.Count);
            Console.WriteLine("MilestoneTasks == " + loan.Log.MilestoneTasks.Count);
            Console.WriteLine("PostClosingConditions == " + loan.Log.PostClosingConditions.Count);
            Console.WriteLine("PreliminaryConditions == " + loan.Log.PreliminaryConditions.Count);
            Console.WriteLine("PrintEvents == " + loan.Log.PrintEvents.Count);
            Console.WriteLine("ReceivedDownloads == " + loan.Log.ReceivedDownloads.Count);
            Console.WriteLine("StatusOnlineUpdates == " + loan.Log.StatusOnlineUpdates.Count);
            Console.WriteLine("TrackedDocuments == " + loan.Log.TrackedDocuments.Count);
            Console.WriteLine("UnderwritingConditions == " + loan.Log.UnderwritingConditions.Count);

            Console.WriteLine("Servicing.IsStarted() == " + loan.Servicing.IsStarted());
            PaymentSchedule schedule = loan.Servicing.GetPaymentSchedule();

            foreach (ScheduledPayment payment in schedule.Payments)
                Console.WriteLine(payment.DueDate + ": P = " + payment.Principal + ", I = " + payment.Interest);
            */


            //Console.WriteLine("Transactions.GetEnumerator().Current == " + loan.Servicing.Transactions.GetEnumerator().Current);
            //Console.WriteLine("Servicing.IsStarted() == " + loan.Servicing.GetPaymentSchedule().Payments[0].);

            //For each element, itterate and print their values...
            /*
            foreach (string field in unknownMiddle.OrderBy(x => x.ToString()))
            {
                FindBoundariesMiddle(loan, field);
            }
            */

            foreach (string field in unknownEnd.OrderBy(x => x.ToString()))
            {
                FindBoundariesEnd(loan, field);
            }

            if (loan != null)
            {
                loan.Close();
            }
        }

        private void FindBoundariesMiddle(Loan loan, string field)
        {
            ISet<string> fieldIds = new HashSet<string>();
            fieldIds.Add(field);
            IDictionary<string, object> results = new Dictionary<string, object>();

            LoanDataUtils.ExtractMiddleIndexFields(loan, fieldIds, results);
            ISet<string> uniques = new HashSet<string>();
            foreach (string key in results.Keys)
            {
                uniques.Add((string) results[key]);
            }
            Console.WriteLine(field + " == [values = " + results.Count + " ][uniques = " + uniques.Count + "]");
        }

        private void FindBoundariesEnd(Loan loan, string field)
        {
            ISet<string> fieldIds = new HashSet<string>();
            fieldIds.Add(field);
            IDictionary<string, object> results = new Dictionary<string, object>();

            LoanDataUtils.ExtractEndIndexFields(loan, fieldIds, results);
            ISet<string> uniques = new HashSet<string>();
            foreach (string key in results.Keys)
            {
                uniques.Add((string) results[key]);
            }
            Console.WriteLine(field + " == [values = " + results.Count + " ][uniques = " + uniques.Count + "]");
        }
    }
}