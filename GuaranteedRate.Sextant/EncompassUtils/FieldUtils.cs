using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessEnums;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.BusinessObjects.Loans.Logging;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.EncompassUtils
{
    /**
     * Singleton to make it easy to push the initialization off into another thread
     * 
     * Creating an instance of EncompassFields takes about 1.4s.
     * 
     * When using as a plugin you do not need to set a session.
     * When using with an SDK app you need to set a session.
     * 
     * Fields are broken apart into collections by type, each type seems to have slightly
     * different parsing rules.
     */
    public class FieldUtils
    {
        private readonly IList<string> SIMPLE_FIELDS;
        private readonly IList<string> MIDDLE_INDEXED;
        private readonly IList<string> END_INDEXED;
        private readonly IList<string> DOCUMENT_MULTI;
        private readonly IList<string> MILESTONE_TASK_MULTI;
        private readonly IList<string> NONE_MULTI;
        private readonly IList<string> POST_CLOSING_CONDITION_MULTI;
        private readonly IList<string> UNDERWRITING_MULTI;
        private readonly IList<string> ROLE_MULTI_KEYS;
        private readonly IList<string> MILESTONE_MULTI_KEYS;

        private readonly IList<string> BORROWER_EMPLOYERS_MULTI_KEYS;
        private readonly IList<string> CO_BORROWER_EMPLOYERS_MULTI_KEYS;
        private readonly IList<string> BORROWER_RESIDENCES_MULTI_KEYS;
        private readonly IList<string> CO_BORROWER_RESIDENCES_MULTI_KEYS;
        private readonly IList<string> LIABILITIES_MULTI_KEYS;
        private readonly IList<string> MORTGAGES_MULTI_KEYS;

        private const string BORROWER_EMPLOYERS_STARTS = "BE";
        private const string CO_BORROWER_EMPLOYERS_STARTS = "CE";
        private const string BORROWER_RESIDENCES_STARTS = "BR";
        private const string CO_BORROWER_RESIDENCES_STARTS = "CR";
        private const string FD_LIABILITIES_STARTS = "FD";
        private const string FL_LIABILITIES_STARTS = "FL";
        private const string MORTGAGES_STARTS = "FM";

        private readonly IDictionary<string, IList<string>> INDEX_MULTI_SORTER;

        private static volatile FieldUtils encompassFields;
        private static object syncRoot = new Object();

        private static IList<FieldDescriptor> SELECTED_FIELDS;

        public static readonly IList<string> BORROWER_PAIR_FIELDS = 
            new List<string> { "4000", "4001", "4002", "4003", "4004", "4005", "4006", "4007", "65", "66", "97", 
                                "1240", "FE0116", "FR0104", "FR0106", "FR0107", "FR0108", "1268" };


        public static void AddFieldCollection(FieldDescriptors fieldDescriptors) 
        {
            if (SELECTED_FIELDS == null)
            {
                SELECTED_FIELDS = new List<FieldDescriptor>();
            }
            AddFieldDescriptors(fieldDescriptors, SELECTED_FIELDS);
        }

        public static void AddFieldCollection(FieldDescriptor fieldDescriptor)
        {
            if (SELECTED_FIELDS == null)
            {
                SELECTED_FIELDS = new List<FieldDescriptor>();
            }
            SELECTED_FIELDS.Add(fieldDescriptor);
        }

        private FieldUtils()
        {
            SIMPLE_FIELDS = new List<string>();
            MIDDLE_INDEXED = new List<string>();
            END_INDEXED = new List<string>();
            DOCUMENT_MULTI = new List<string>();
            MILESTONE_TASK_MULTI = new List<string>();
            NONE_MULTI = new List<string>();
            POST_CLOSING_CONDITION_MULTI = new List<string>();
            UNDERWRITING_MULTI = new List<string>();

            BORROWER_EMPLOYERS_MULTI_KEYS = new List<string>();
            CO_BORROWER_EMPLOYERS_MULTI_KEYS = new List<string>();
            BORROWER_RESIDENCES_MULTI_KEYS = new List<string>();
            CO_BORROWER_RESIDENCES_MULTI_KEYS = new List<string>();
            LIABILITIES_MULTI_KEYS = new List<string>();
            MORTGAGES_MULTI_KEYS = new List<string>();

            INDEX_MULTI_SORTER = new Dictionary<string, IList<string>>();
            INDEX_MULTI_SORTER.Add(BORROWER_EMPLOYERS_STARTS, BORROWER_EMPLOYERS_MULTI_KEYS);
            INDEX_MULTI_SORTER.Add(CO_BORROWER_EMPLOYERS_STARTS, CO_BORROWER_EMPLOYERS_MULTI_KEYS);
            INDEX_MULTI_SORTER.Add(BORROWER_RESIDENCES_STARTS, BORROWER_RESIDENCES_MULTI_KEYS);
            INDEX_MULTI_SORTER.Add(CO_BORROWER_RESIDENCES_STARTS, CO_BORROWER_RESIDENCES_MULTI_KEYS);
            INDEX_MULTI_SORTER.Add(FD_LIABILITIES_STARTS, LIABILITIES_MULTI_KEYS);
            INDEX_MULTI_SORTER.Add(FL_LIABILITIES_STARTS, LIABILITIES_MULTI_KEYS);
            INDEX_MULTI_SORTER.Add(MORTGAGES_STARTS, MORTGAGES_MULTI_KEYS);

            ROLE_MULTI_KEYS = GetRoleMultiKeys();
            MILESTONE_MULTI_KEYS = GetMilestoneMultiKeys();

            GetAllFieldIds();
        }

        public static Session session = null;

        /**
         * Double-check locking to ensure the singleton is only created once.
         * Note the reporter is also volatile which is requried to make the double-check correct.
         */
        private static FieldUtils Instance
        {
            get
            {
                if (encompassFields == null)
                {
                    lock (syncRoot)
                    {
                        if (encompassFields == null)
                        {
                            if (session == null)
                            {
                                session = EncompassApplication.Session;
                            }
                            encompassFields = new FieldUtils();
                        }
                    }
                }
                return encompassFields;
            }
        }

        private IList<string> GetRoleMultiKeys()
        {
            IList<string> keys = new List<string>();
            foreach (Role role in session.Loans.Roles)
            {
                keys.Add(role.Name);
            }
            return keys;
        }

        private IList<string> GetMilestoneMultiKeys()
        {
            IList<string> keys = new List<string>();
            foreach (Milestone m in session.Loans.Milestones)
            {
                keys.Add(m.Name);
            }
            return keys;
        }

        private void GetAllFieldIds()
        {
            IList<FieldDescriptor> fieldDescriptorsList = FieldUtils.SELECTED_FIELDS;
            if (fieldDescriptorsList == null)
            {
                fieldDescriptorsList = GetAllFieldDescriptors();
            }
            LoadFieldIdsFromFieldDescriptors(fieldDescriptorsList);
        }

        private static IList<FieldDescriptor> AddFieldDescriptors(FieldDescriptors fieldCollection, IList<FieldDescriptor> fieldList)
        {
            foreach (FieldDescriptor field in fieldCollection)
            {
                fieldList.Add(field);
            }
            return fieldList;
        }

        private IList<FieldDescriptor> GetAllFieldDescriptors()
        {
            IList<FieldDescriptor> fieldList = new List<FieldDescriptor>();
            FieldUtils.AddFieldDescriptors(FieldUtils.session.Loans.FieldDescriptors.StandardFields, fieldList);
            FieldUtils.AddFieldDescriptors(FieldUtils.session.Loans.FieldDescriptors.CustomFields, fieldList);
            FieldUtils.AddFieldDescriptors(FieldUtils.session.Loans.FieldDescriptors.VirtualFields, fieldList);
            return fieldList;
        }

        /***
         * Depending on the field type it is sometimes possible to know the field ids at the session
         * level, sometimes the specific fieldIds depend on the loan file itself.
         * 
         * Multi-value fieldIds whose indexes are defined at the session level will be unrolled into multiple
         * simple value fields.
         * 
         * Multi-value fieldIds whose indexes are defined at the loan level will be seperated into specific
         * lists so that they can be handled on a loan-by-loan basis.
         */
        private void LoadFieldIdsFromFieldDescriptors(IList<FieldDescriptor> fieldDescriptors)
        {
            foreach (FieldDescriptor fieldDescriptor in fieldDescriptors)
            {
                if (!fieldDescriptor.MultiInstance)
                {
                    SIMPLE_FIELDS.Add(fieldDescriptor.FieldID);
                }
                else
                {
                    switch (fieldDescriptor.InstanceSpecifierType)
                    {
                        case MultiInstanceSpecifierType.Index:
                            //TODO: Still working the new way
                            string key = fieldDescriptor.FieldID.Substring(0, 2);
                            if (INDEX_MULTI_SORTER.Keys.Contains(key))
                            {
                                INDEX_MULTI_SORTER[key].Add(fieldDescriptor.FieldID);
                            }
                            
                            //TODO: The old way
                            if (fieldDescriptor.FieldID.Substring(2, 2) == "00")
                            {
                                MIDDLE_INDEXED.Add(fieldDescriptor.FieldID);
                            }
                            else if (fieldDescriptor.FieldID.Contains("00"))
                            {
                                //FIXME: Not sure how to handle these fields
                                // MIDDLE_INDEXED.Add(fieldDescriptor.FieldID);
                            }
                            else
                            {
                                END_INDEXED.Add(fieldDescriptor.FieldID);
                            }
                            break;
                        case MultiInstanceSpecifierType.Document:
                            DOCUMENT_MULTI.Add(fieldDescriptor.FieldID);
                            break;
                        case MultiInstanceSpecifierType.Milestone:
                            UnrollMultiFieldIds(fieldDescriptor.FieldID, MILESTONE_MULTI_KEYS);
                            break;
                        case MultiInstanceSpecifierType.MilestoneTask:
                            MILESTONE_TASK_MULTI.Add(fieldDescriptor.FieldID);
                            break;
                        case MultiInstanceSpecifierType.PostClosingCondition:
                            POST_CLOSING_CONDITION_MULTI.Add(fieldDescriptor.FieldID);
                            break;
                        case MultiInstanceSpecifierType.Role:
                            UnrollMultiFieldIds(fieldDescriptor.FieldID, ROLE_MULTI_KEYS);
                            break;
                        case MultiInstanceSpecifierType.UnderwritingCondition:
                            UNDERWRITING_MULTI.Add(fieldDescriptor.FieldID);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void UnrollMultiFieldIds(string fieldId, IList<string> keys)
        {
            foreach (string key in keys)
            {
                SIMPLE_FIELDS.Add(fieldId + "." + key);
            }
        }

        /**
         * These method are expected to be called repeatedly, so the results are cached.
         */
        public static IList<string> SimpleFieldNames()
        {
            return Instance.SIMPLE_FIELDS;
        }

        public static IList<string> MiddleIndexMulti()
        {
            return Instance.MIDDLE_INDEXED;
        }

        public static IList<string> EndIndexMulti()
        {
            return Instance.END_INDEXED;
        }

        public static IList<string> DocumentMulti()
        {
            return Instance.DOCUMENT_MULTI;
        }

        public static IList<string> MilestoneTaskMulti()
        {
            return Instance.MILESTONE_TASK_MULTI;
        }

        public static IList<string> PostClosingMulti()
        {
            return Instance.POST_CLOSING_CONDITION_MULTI;
        }

        public static IList<string> RoleMultiKeys()
        {
            return Instance.ROLE_MULTI_KEYS;
        }

        public static IList<string> UnderwritingMulti()
        {
            return Instance.UNDERWRITING_MULTI;
        }

        /**
         * This method is not expected to be called in most cases, and if it is called, won't be called repeatedly.
         * So not caching results.
         */
        public static IDictionary<string, string> GetFieldsAndDescriptions()
        {
            IDictionary<string, string> fieldsAndDescriptions = new Dictionary<string, string>();
            IList<FieldDescriptor> fieldDescriptorsList = Instance.GetAllFieldDescriptors();

            foreach (FieldDescriptor fieldDescriptor in fieldDescriptorsList)
            {
                fieldsAndDescriptions.Add(fieldDescriptor.FieldID, fieldDescriptor.Description);
            }
            return fieldsAndDescriptions;
        }

        public static ISet<string> ReportableVirtualFields()
        {
            ISet<string> rvf = new HashSet<string>();

            ReportingFieldDescriptorList reportable = FieldUtils.session.Reports.GetReportingDatabaseFields();
            FieldDescriptors virtualFields = FieldUtils.session.Loans.FieldDescriptors.VirtualFields;

            ISet<string> rep = new HashSet<string>();
            foreach (ReportingFieldDescriptor f in reportable)
            {
                rep.Add(f.FieldID);
            }

            foreach (IFieldDescriptor f in virtualFields)
            {
                if (rep.Contains(f.FieldID))
                {
                    rvf.Add(f.FieldID);
                }
            }

            return rvf;
        }
    }
}