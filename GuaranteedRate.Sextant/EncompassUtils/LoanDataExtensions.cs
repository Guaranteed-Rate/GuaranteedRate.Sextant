using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.BusinessObjects.Users;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncompassLoanDataCollector
{
    public static class LoanDataExtensions
    {
        #region get loan field map

        ///<summary>
        ///Generates a dictionary of loan values from a list of field ids
        ///</summary>
        public static Dictionary<string, string> GetLoanFieldMap(this Loan loan, List<string> fieldIds)
        {
            fieldIds = fieldIds.Distinct().ToList();
            Dictionary<string, string> fields = new Dictionary<string, string>();

            foreach (string fieldId in fieldIds)
            {
                string value = GetFieldValueAsString(loan, fieldId);
                fields.Add(fieldId, value);
            }

            return fields;
        }

        ///<summary>
        ///Generates a dictionary of loan values from a list of field ids specific to a particular borrower pair
        ///</summary>
        public static Dictionary<string, string> GetLoanFieldMap(this Loan loan, BorrowerPair borrowerPair, List<string> fieldIds)
        {
            fieldIds = fieldIds.Distinct().ToList();
            Dictionary<string, string> fields = new Dictionary<string, string>();

            foreach (string fieldId in fieldIds)
            {
                string value = GetFieldValueAsString(loan, borrowerPair, fieldId);
                fields.Add(fieldId, value);
            }

            return fields;
        }

        ///<summary>
        ///Generates a dictionary of field values from the current client's loan from a list of field ids specific to a particular borrower pair
        ///</summary>
        public static Dictionary<string, string> GetLoanFieldMap(this Loan loan, List<string> fieldIds, string borrowerSSN)
        {
            fieldIds = fieldIds.Distinct().ToList();
            Dictionary<string, string> fields = new Dictionary<string, string>();

            BorrowerPair borrowerPair = loan.GetBorrowerPairFromPrimaryBorrowerSSN(borrowerSSN);

            foreach (string fieldId in fieldIds)
            {
                string value = GetFieldValueAsString(loan, borrowerPair, fieldId);
                fields.Add(fieldId, value);
            }

            return fields;
        }

        #endregion

        ///<summary>
        ///Get all borrower pairs
        ///</summary>
        public static List<BorrowerPair> GetBorrowerPairs(this Loan loan)
        {
            List<BorrowerPair> borrowerPairs = new List<BorrowerPair>();

            foreach(BorrowerPair borrowerPair in loan.BorrowerPairs)
            {
                borrowerPairs.Add(borrowerPair);
            }

            return borrowerPairs;
        }

        ///<summary>
        ///Get field value from a specific loan
        ///</summary>
        public static string GetFieldValueAsString(this Loan loan, string fieldId)
        {
            object fieldValue = loan.Fields[fieldId].Value;
            if (fieldValue != null)
                return fieldValue.ToString();

            throw new Exception("Field Id does not exist.");
        }

        ///<summary>
        ///Get field value from a specific loan for a specific borrower pair
        ///</summary>
        public static string GetFieldValueAsString(this Loan loan, BorrowerPair borrowerPair, string fieldId)
        {
            object fieldValue = loan.Fields[fieldId].GetValueForBorrowerPair(borrowerPair);
            if (fieldValue != null)
                return fieldValue.ToString();

            throw new Exception("Field Id does not exist.");
        }

        public static string GetCurrentUserID(this Loan loan)
        {
            return loan.Session.GetCurrentUser().ID;
        }

        public static User GetUser(this Loan loan, string userId)
        {
            return loan.Session.Users.GetUser(userId);
        }

        #region get emails

        ///<summary>
        ///Get current logged in user's email
        ///</summary>
        public static string GetCurrentUserEmail(this Loan loan)
        {
            return loan.Session.GetCurrentUser().Email;
        }

        ///<summary>
        ///Get LO Email for a specific loan
        ///</summary>
        public static string GetLoanOfficerEmail(this Loan loan)
        {
            string loanOfficerEmailFieldId = "1407";
            return loan.GetFieldValueAsString(loanOfficerEmailFieldId);
        }

        /// <summary>
        ///Get LC Email for a specific loan
        /// </summary>
        public static string GetLoanCoordinatorEmail(this Loan loan)
        {
            string loanCoordinatorEmailFieldId = "LoanTeamMember.UserID.Loan Coordinator";
            return loan.GetFieldValueAsString(loanCoordinatorEmailFieldId);
        }

        ///<summary>
        ///Get MC Email for a specific loan
        ///</summary>
        public static string GetMortgageConsultantEmail(this Loan loan)
        {
            string mortgageConsultantEmailFieldId = "LoanTeamMember.UserID.Mortgage Consultant";
            return loan.GetFieldValueAsString(mortgageConsultantEmailFieldId);
        }

        ///<summary>
        ///Get PM Email for a specific loan
        ///</summary>
        public static string GetProcessManagerEmail(this Loan loan)
        {
            string processManageEmailFieldId = "LoanTeamMember.Email.ProcessManager";
            return loan.GetFieldValueAsString(processManageEmailFieldId);
        }

        #endregion

        #region utility

        ///<summary>
        ///Returns a borrowerpair object from a borrower's email address
        ///</summary>
        public static BorrowerPair GetBorrowerPairFromPrimaryBorrowerEmail(this Loan loan, string borrowerEmail)
        {
            string borrowerEmailAddressFieldId = "1240";

            foreach (BorrowerPair borrowerPair in loan.BorrowerPairs)
            {
                if (GetFieldValueAsString(loan, borrowerPair, borrowerEmailAddressFieldId) == borrowerEmail)
                    return borrowerPair;
            }

            throw new Exception("No borrower pair found with borrower email of " + borrowerEmail + ".");
        }

        ///<summary>
        ///Returns a borrowerpair object from a borrower's social security number
        ///</summary>
        public static BorrowerPair GetBorrowerPairFromPrimaryBorrowerSSN(this Loan loan, string ssn)
        {
            string borrowerEmailAddressFieldId = "65";

            foreach (BorrowerPair borrowerPair in loan.BorrowerPairs)
            {
                if (GetFieldValueAsString(loan, borrowerPair, borrowerEmailAddressFieldId) == ssn)
                    return borrowerPair;
            }

            throw new Exception("No borrower pair found with ssn of " + ssn + ".");
        }

        #endregion
    }
}
