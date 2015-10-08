using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
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
        private readonly IList<string> MILESTONE_MULTI;
        private readonly IList<string> MILESTONE_TASK_MULTI;
        private readonly IList<string> NONE_MULTI;
        private readonly IList<string> ROLE_MULTI;
        private readonly IList<string> POST_CLOSING_CONDITION_MULTI;
        private readonly IList<string> UNDERWRITING_MULTI;

        private static volatile FieldUtils encompassFields;
        private static object syncRoot = new Object();

        private static IList<FieldDescriptors> FIELD_COLLECTIONS;

        public static readonly IList<string> BORROWER_PAIR_FIELDS = 
            new List<string> { "4000", "4001", "4002", "4003", "4004", "4005", "4006", "4007", "65", "66", "97", 
                                "1240", "FE0116", "FR0104", "FR0106", "FR0107", "FR0108", "1268" };


        public static void AddFieldCollection(FieldDescriptors fieldDescriptors) 
        {
            if (FIELD_COLLECTIONS == null)
            {
                FIELD_COLLECTIONS = new List<FieldDescriptors>();
            }
            FIELD_COLLECTIONS.Add(fieldDescriptors);
        }

        private FieldUtils()
        {
            SIMPLE_FIELDS = new List<string>();
            MIDDLE_INDEXED = new List<string>();
            END_INDEXED = new List<string>();
            DOCUMENT_MULTI = new List<string>();
            MILESTONE_MULTI = new List<string>();
            MILESTONE_TASK_MULTI = new List<string>();
            NONE_MULTI = new List<string>();
            ROLE_MULTI = new List<string>();
            POST_CLOSING_CONDITION_MULTI = new List<string>();
            UNDERWRITING_MULTI = new List<string>();

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

        private void GetAllFieldIds()
        {
            IList<FieldDescriptors> fieldDescriptorsList = FieldUtils.FIELD_COLLECTIONS;
            if (fieldDescriptorsList == null)
            {
                fieldDescriptorsList = GetAllFieldDescriptors();
            }
            foreach (FieldDescriptors fieldDescriptors in fieldDescriptorsList)
            {
                GetFieldIdsFromFieldDescriptors(fieldDescriptors);
            }
        }

        private IList<FieldDescriptors> GetAllFieldDescriptors()
        {
            IList<FieldDescriptors> fieldDescriptorsList = new List<FieldDescriptors>() 
            {
                FieldUtils.session.Loans.FieldDescriptors.CustomFields,
                FieldUtils.session.Loans.FieldDescriptors.StandardFields,
                FieldUtils.session.Loans.FieldDescriptors.VirtualFields
            };

            return fieldDescriptorsList;
        }

        private void GetFieldIdsFromFieldDescriptors(FieldDescriptors fieldDescriptors)
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
                            MILESTONE_MULTI.Add(fieldDescriptor.FieldID);
                            break;
                        case MultiInstanceSpecifierType.MilestoneTask:
                            MILESTONE_TASK_MULTI.Add(fieldDescriptor.FieldID);
                            break;
                        case MultiInstanceSpecifierType.PostClosingCondition:
                            POST_CLOSING_CONDITION_MULTI.Add(fieldDescriptor.FieldID);
                            break;
                        case MultiInstanceSpecifierType.Role:
                            ROLE_MULTI.Add(fieldDescriptor.FieldID);
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

        public static IList<string> MilestoneMulti()
        {
            return Instance.MILESTONE_MULTI;
        }

        public static IList<string> MilestoneTaskMulti()
        {
            return Instance.MILESTONE_TASK_MULTI;
        }

        public static IList<string> PostClosingMulti()
        {
            return Instance.POST_CLOSING_CONDITION_MULTI;
        }

        public static IList<string> RoleMulti()
        {
            return Instance.ROLE_MULTI;
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
            IList<FieldDescriptors> fieldDescriptorsList = Instance.GetAllFieldDescriptors();
            foreach (FieldDescriptors fieldDescriptors in fieldDescriptorsList)
            {
                foreach (FieldDescriptor fieldDescriptor in fieldDescriptors)
                {
                    fieldsAndDescriptions.Add(fieldDescriptor.FieldID, fieldDescriptor.Description);
                }
            }
            return fieldsAndDescriptions;
        }
    }
}