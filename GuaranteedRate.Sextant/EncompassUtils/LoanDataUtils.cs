using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.BusinessObjects.Loans.Logging;
using GuaranteedRate.Sextant.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.EncompassUtils
{
    /**
     * Encompass has a LoanUtils class
     */
    public class LoanDataUtils
    {
        public const int MULTI_MAX = 10;

        /**
         * Still a work in progress - ideally this function will iterate 
         * through a loan's fields and return a Dictonary representation.
         * 
         * Works fine with SimpleFields, but not most types of multi-field values
         */
        public static IDictionary<string, object> ExtractLoanFields(Loan currentLoan)
        {
            IDictionary<string, object> fieldValues = new Dictionary<string, object>();

            ExtractEndIndexFields(currentLoan, FieldUtils.EndIndexMulti(), fieldValues);
            ExtractMiddleIndexFields(currentLoan, FieldUtils.MiddleIndexMulti(), fieldValues);
            ExtractSimpleFields(currentLoan, FieldUtils.SimpleFieldNames(), fieldValues);

            //This is a subset of the borrower pair information, there does not seem to be an efficient method for
            //extracting all of this data programmatically.
            fieldValues.Add("borrower-pairs", ExtractBorrowerPairs(currentLoan));

            ExtractProperties(currentLoan, fieldValues);

            //TODO: These do not seem to hit anything...logic is most likely incorrect
            ExtractEndIndexFields(currentLoan, FieldUtils.PostClosingMulti(), fieldValues);
            ExtractEndIndexFields(currentLoan, FieldUtils.RoleMulti(), fieldValues);
            ExtractEndIndexFields(currentLoan, FieldUtils.UnderwritingMulti(), fieldValues);
            ExtractEndIndexFields(currentLoan, FieldUtils.MilestoneTaskMulti(), fieldValues);
            ExtractEndIndexFields(currentLoan, FieldUtils.MilestoneMulti(), fieldValues);
            ExtractEndIndexFields(currentLoan, FieldUtils.DocumentMulti(), fieldValues);

            return fieldValues;
        }

        public static IDictionary<string, object> ExtractEverything(Loan loan)
        {
            IDictionary<string, object> loanData = new Dictionary<string, object>();
            loanData.Add("lastmodified", loan.LastModified.ToString());
            loanData.Add("fields", ExtractLoanFields(loan));
            loanData.Add("milestones",ExtractMilestones(loan));
            return loanData;
        }

        public static IDictionary<string, object> ExtractProperties(Loan loan, IDictionary<string, object> fieldValues)
        {
            fieldValues.Add("LoanFolder", loan.LoanFolder);
            return fieldValues;
        }

        /**
         * There's no specific list of fields affected by borrower pairs.
         * We've defined a set that's useful to us, but you can override with your own
         */ 
        public static IList<IDictionary<string, object>> ExtractBorrowerPairs(Loan loan)
        {
            return ExtractBorrowerPairs(loan, FieldUtils.BORROWER_PAIR_FIELDS);
        }

        public static IList<IDictionary<string, object>> ExtractBorrowerPairs(Loan loan, IList<string> fields)
        {
            IList<IDictionary<string, object>> borrowerPairs = new List<IDictionary<string, object>>();
            string primarySsn = FormatSSN(ParseField(loan.Fields["65"].Value));
            foreach (BorrowerPair pair in loan.BorrowerPairs)
            {
                IDictionary<string, object> fieldDictionary = new Dictionary<string, object>();
                borrowerPairs.Add(ExtractSimpleFields(loan, pair, fields, fieldDictionary));
                if (FormatSSN(fieldDictionary["65"].ToString()) == primarySsn)
                {
                    fieldDictionary.Add("PrimaryPair", true);
                }
                else
                {
                    fieldDictionary.Add("PrimaryPair", false);
                }
            }
            return borrowerPairs;
        }

        public static IList<IDictionary<string, string>> ExtractMilestones(Loan loan)
        {
            IList<IDictionary<string, string>> milestones = new List<IDictionary<string, string>>();
            DateTime lastModified = loan.LastModified;
            foreach (MilestoneEvent milestone in loan.Log.MilestoneEvents)
            {
                IDictionary<string, string> localMilestone = new Dictionary<string, string>();
                localMilestone.Add("milestoneName", ParseField(milestone.MilestoneName));
                localMilestone.Add("completed", milestone.Completed.ToString());
                localMilestone.Add("completedDate", ParseField(milestone.Date.ToString()));
                string comments = ParseField(milestone.Comments);
                if (!String.IsNullOrWhiteSpace(comments))
                {
                    localMilestone.Add("comments", comments);
                }
                if ((milestone.LoanAssociate != null) && (milestone.LoanAssociate.User != null))
                {
                    localMilestone.Add("userId", ParseField(milestone.LoanAssociate.User.ID));
                }
                milestones.Add(localMilestone);
            }
            return milestones;
        }

        private static string FormatSSN(string ssn)
        {
            if (ssn == null)
            {
                return null;
            }
            if (ssn.Length ==9 ) 
            {
                return ssn.Insert(5, "-").Insert(3, "-");
            }
            return ssn;
        }
        
        /*
        public static IList<IBorrowerPair> ExtractBorrowerResidences(Loan currentLoan)
        {
            currentLoan.Fields[2].
            //currentLoan.BorrowerPairs.Current = 
            //IList<BorroerR> pairs = new List<IBorrowerPair>();
            //foreach (LoanResidences residence in currentLoan.BorrowerResidences)
            //foreach (ILoanEmployers employer in currentLoan.BorrowerEmployers)
            {
                //pairs.Add(pair);
            }
            return null;
        }
         */

        public static IDictionary<string, object> ExtractEndIndexFields(Loan currentLoan, IList<string> fieldIds, IDictionary<string, object> fieldDictionary)
        {
            foreach (string fieldId in fieldIds)
            {
                int index = 0;
                try
                {
                    IDictionary<string, string> values = new Dictionary<string, string>();

                    for (index = 1; index < MULTI_MAX; index++)
                    {
                        string indexPad = IntPad(index);
                        object fieldObject = currentLoan.Fields[fieldId + "." + indexPad].Value;
                        string value = ParseField(fieldObject);
                        if (value != null)
                        {
                            values.Add(indexPad, value);
                        }
                    }
                    if (values.Count > 0)
                    {
                        fieldDictionary.Add(fieldId, values);
                    }
                }
                catch (Exception e)
                {
                    //Debug.WriteLine("Failed to pull: " + fieldId + " index=" + index + " Exception: " + e);
                    Loggly.Error("LoandataUtils", "Failed to pull: " + fieldId + " index=" + index + " Exception: " + e);
                }
            }
            return fieldDictionary;
        }

        private static string IntPad(int x)
        {
            if (x < 10)
            {
                return "0" + x;
            }
            else
            {
                return x + "";
            }
        }

        public static IDictionary<string, object> ExtractMiddleIndexFields(Loan currentLoan, IList<string> fieldIds, IDictionary<string, object> fieldDictionary)
        {
            foreach (string fieldId in fieldIds)
            {
                int index = 0;
                try
                {
                    int offset = fieldId.IndexOf("00");
                    string pre = fieldId.Substring(0, offset);
                    string post = fieldId.Substring(offset + 2);
                    IDictionary<string, string> values = new Dictionary<string, string>();

                    //Requesting 00 SHOULD always return null.  
                    for (index = 0; index < MULTI_MAX; index++)
                    {
                        string indexPad = IntPad(index);
                        object fieldObject = currentLoan.Fields[pre + indexPad + post].Value;
                        string value = ParseField(fieldObject);
                        if (value != null)
                        {
                            values.Add(indexPad, value);
                        }
                    }
                    if (values.Count > 0)
                    {
                        fieldDictionary.Add(fieldId, values);
                    }
                }
                catch (Exception e)
                {
                    //Debug.WriteLine("Failed to pull: " + fieldId + " index=" + index + " Exception: " + e);
                    Loggly.Error("LoandataUtils", "Failed to pull: " + fieldId + " index=" + index + " Exception: " + e);
                }
            }
            return fieldDictionary;
        }

        public static IDictionary<string, object> ExtractSimpleFields(Loan currentLoan, IList<string> fieldIds, IDictionary<string, object> fieldDictionary)
        {
            foreach (string fieldId in fieldIds)
            {
                try
                {
                    object fieldObject;
                    fieldObject = currentLoan.Fields[fieldId].Value;
                    string value = ParseField(fieldObject);
                    if (value != null)
                    {
                        fieldDictionary.Add(fieldId, value);
                    }
                }
                catch (Exception e)
                {
                    //Debug.WriteLine("Failed to pull: " + fieldId + " Exception: " + e);
                    Loggly.Error("LoandataUtils", "Failed to pull: " + fieldId + " Exception: " + e);
                }
            }
            return fieldDictionary;
        }

        public static IDictionary<string, object> ExtractSimpleFields(Loan currentLoan, BorrowerPair borrowerPair, IList<string> fieldIds, IDictionary<string, object> fieldDictionary)
        {
            foreach (string fieldId in fieldIds)
            {
                try
                {
                    object fieldObject;
                    fieldObject = currentLoan.Fields[fieldId].GetValueForBorrowerPair(borrowerPair);
                    string value = ParseField(fieldObject);
                    if (value != null)
                    {
                        fieldDictionary.Add(fieldId, value);
                    }
                }
                catch (Exception e)
                {
                    //Debug.WriteLine("Failed to pull: " + fieldId + " Exception: " + e);
                    Loggly.Error("LoandataUtils", "Failed to pull: " + fieldId + " Exception: " + e);
                }
            }
            return fieldDictionary;
        }

        public static string ParseField(object fieldObject)
        {
            if (fieldObject != null)
            {
                string fieldValue = fieldObject.ToString();
                if (!String.IsNullOrWhiteSpace(fieldValue))
                {
                    return fieldValue;
                }
            }
            return null;
        }

    }
}