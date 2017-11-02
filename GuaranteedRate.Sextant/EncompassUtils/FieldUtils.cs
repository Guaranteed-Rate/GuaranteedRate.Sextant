using EllieMae.Encompass.BusinessEnums;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using  GuaranteedRate.Sextant.Logging;

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
        private readonly ISet<string> SIMPLE_FIELDS;
        private readonly ISet<string> MIDDLE_INDEXED;
        private readonly ISet<string> END_INDEXED;
        private readonly ISet<string> DOCUMENT_MULTI;
        private readonly ISet<string> MILESTONE_TASK_MULTI;
        private readonly ISet<string> NONE_MULTI;
        private readonly ISet<string> POST_CLOSING_CONDITION_MULTI;
        private readonly ISet<string> UNDERWRITING_MULTI;
        private readonly ISet<string> ROLE_MULTI_KEYS;
        private readonly ISet<string> MILESTONE_MULTI_KEYS;

        private readonly ISet<string> BORROWER_EMPLOYERS_MULTI_KEYS;
        private readonly ISet<string> CO_BORROWER_EMPLOYERS_MULTI_KEYS;
        private readonly ISet<string> BORROWER_RESIDENCES_MULTI_KEYS;
        private readonly ISet<string> CO_BORROWER_RESIDENCES_MULTI_KEYS;
        private readonly ISet<string> LIABILITIES_MULTI_KEYS;
        private readonly ISet<string> DEPOSITS_MULTI_KEYS;
        private readonly ISet<string> MORTGAGES_MULTI_KEYS;
        private readonly ISet<string> VESTING_MULTI_KEYS;

        private readonly ISet<string> DISCLOSURES_MULTI_KEYS;

        private const string BORROWER_EMPLOYERS_STARTS = "BE";
        private const string CO_BORROWER_EMPLOYERS_STARTS = "CE";
        private const string BORROWER_RESIDENCES_STARTS = "BR";
        private const string CO_BORROWER_RESIDENCES_STARTS = "CR";
        private const string FD_DEPOSITS_STARTS = "FD";
        private const string DD_DEPOSITS_STARTS = "DD";
        private const string FL_LIABILITIES_STARTS = "FL";
        private const string IRS_MORTGAGE_INFO_STARTS = "IR";
        private const string MORTGAGES_STARTS = "FM";
        private const string VESTING_PARITIES_STARTS = "TR";

        private static readonly ISet<string> END_INDEX_EXCLUDE_LIST    = new HashSet<string>() {"IRS4506.X61", "IRS4506.X62"};
        private static readonly ISet<string> MIDDLE_INDEX_EXCLUDE_LIST = new HashSet<string>() { "AUSTRACKING.AUS.X100" };
      
        private const string DISCLOSURES_STARTS = "DISCLOSED";

        /**
         * Need to find indexes for:
         *  AR00xx  - Tax info
         *  AB00xx  - Affilitaed Businesses
         *  HC00xx  - Home counsoling info
         *  SP00xx  - Settlement provider
         *  TA00xx  - Trust transaction info
         */
        private readonly IDictionary<string, ISet<string>> INDEX_MULTI_SORTER;
        private readonly IDictionary<string, ISet<string>> INDEX_MULTI_SORTER_END;
        private readonly ISet<string> UNKNOWN_KEYS;

        private static volatile FieldUtils encompassFields;
        private static object syncRoot = new Object();

        private static ISet<FieldDescriptor> SELECTED_FIELDS;


        /// <summary>
        /// These *SEEM* to be the *Simple* fields that are affected by switching active borrower-pair.
        /// Have not found a good way to know which fields are affected by the active borrower-pair.
        /// </summary>
        public static readonly ISet<string> BORROWER_PAIR_FIELDS =
            new HashSet<string>
            {
                "36",
                "37",
                "38",
                "39",
                "52",
                "53",
                "54",
                "60",
                "65",
                "66",
                "67",
                "68",
                "69",
                "70",
                "71",
                "84",
                "97",
                "98",
                "100",
                "101",
                "102",
                "103",
                "104",
                "105",
                "106",
                "107",
                "108",
                "109",
                "110",
                "111",
                "112",
                "113",
                "114",
                "115",
                "116",
                "117",
                "118",
                "119",
                "120",
                "121",
                "122",
                "123",
                "124",
                "125",
                "126",
                "144",
                "145",
                "146",
                "147",
                "148",
                "149",
                "150",
                "151",
                "152",
                "153",
                "154",
                "155",
                "156",
                "157",
                "168",
                "169",
                "170",
                "171",
                "172",
                "173",
                "174",
                "175",
                "176",
                "177",
                "178",
                "179",
                "180",
                "181",
                "182",
                "183",
                "186",
                "188",
                "189",
                "191",
                "265",
                "266",
                "267",
                "268",
                "271",
                "272",
                "273",
                "403",
                "418",
                "461",
                "463",
                "464",
                "466",
                "467",
                "470",
                "471",
                "477",
                "478",
                "687",
                "900",
                "901",
                "902",
                "903",
                "904",
                "905",
                "906",
                "907",
                "908",
                "909",
                "910",
                "911",
                "912",
                "915",
                "919",
                "920",
                "921",
                "922",
                "923",
                "924",
                "933",
                "934",
                "936",
                "938",
                "940",
                "941",
                "942",
                "943",
                "981",
                "985",
                "1015",
                "1057",
                "1058",
                "1062",
                "1069",
                "1070",
                "1087",
                "1088",
                "1089",
                "1108",
                "1136",
                "1144",
                "1145",
                "1146",
                "1156",
                "1158",
                "1159",
                "168",
                "1169",
                "1170",
                "1171",
                "1178",
                "1179",
                "1188",
                "1197",
                "1240",
                "1241",
                "1268",
                "1300",
                "1306",
                "1307",
                "1308",
                "1309",
                "1310",
                "1311",
                "1312",
                "1313",
                "1314",
                "1315",
                "1316",
                "1317",
                "1318",
                "1319",
                "1320",
                "1321",
                "1323",
                "1325",
                "1343",
                "1389",
                "1402",
                "1403",
                "1414",
                "1415",
                "1416",
                "1417",
                "1418",
                "1419",
                "1450",
                "1452",
                "1480",
                "1484",
                "1490",
                "1502",
                "1519",
                "1520",
                "1521",
                "1522",
                "1523",
                "1524",
                "1525",
                "1526",
                "1527",
                "1528",
                "1529",
                "1530",
                "1531",
                "1532",
                "1533",
                "1534",
                "1535",
                "1536",
                "1537",
                "1538",
                "1758",
                "1759",
                "1815",
                "1816",
                "1817",
                "1818",
                "1819",
                "1820",
                "1868",
                "1873",
                "2849",
                "2850",
                "4000",
                "4001",
                "4002",
                "4003",
                "4004",
                "4005",
                "4006",
                "4007",
                "4008",
                "4009",
                "FE0102",
                "FE0103",
                "FE0104",
                "FE0105",
                "FE0106",
                "FE0107",
                "FE0109",
                "FE0110",
                "FE0113",
                "FE0115",
                "FE0116",
                "FE0117",
                "FE0133",
                "FE0198",
                "FE0199",
                "FE0202",
                "FE0203",
                "FE0204",
                "FE0205",
                "FE0206",
                "FE0207",
                "FE0209",
                "FE0210",
                "FE0213",
                "FE0215",
                "FE0216",
                "FE0217",
                "FE0233",
                "FE0298",
                "FE0299",
                "FR0104",
                "FR0106",
                "FR0107",
                "FR0108",
                "FR0112",
                "FR0115",
                "FR0124",
                "FR0198",
                "FR0199",
                "FR0204",
                "FR0206",
                "FR0207",
                "FR0208",
                "FR0212",
                "FR0215",
                "FR0224",
                "FR0298",
                "FR0299",
                "FR0304",
                "FR0306",
                "FR0307",
                "FR0308",
                "FR0312",
                "FR0315",
                "FR0324",
                "FR0398",
                "FR0399",
                "FR0404",
                "FR0406",
                "FR0407",
                "FR0408",
                "FR0412",
                "FR0415",
                "FR0424",
                "FR0498",
                "FR0499"
            };

        public static void RemoveFieldCollection(FieldDescriptor fieldDescriptor)
        {
            if (SELECTED_FIELDS == null)
            {
                return;
            }
            SELECTED_FIELDS.Remove(fieldDescriptor);
        }

        public static void RemoveFieldCollection(FieldDescriptors fieldDescriptors)
        {
            if (SELECTED_FIELDS == null)
            {
                return;
            }
            foreach (FieldDescriptor field in fieldDescriptors)
            {
                SELECTED_FIELDS.Remove(field);
            }
        }

        public static void AddFieldCollection(FieldDescriptors fieldDescriptors)
        {
            if (SELECTED_FIELDS == null)
            {
                SELECTED_FIELDS = new HashSet<FieldDescriptor>();
            }
            AddFieldDescriptors(fieldDescriptors, SELECTED_FIELDS);
        }

        public static void AddFieldCollection(FieldDescriptor fieldDescriptor)
        {
            if (SELECTED_FIELDS == null)
            {
                SELECTED_FIELDS = new HashSet<FieldDescriptor>();
            }
            SELECTED_FIELDS.Add(fieldDescriptor);
        }

        private FieldUtils()
        {
            SIMPLE_FIELDS = new HashSet<string>();
            MIDDLE_INDEXED = new HashSet<string>();
            END_INDEXED = new HashSet<string>();
            DOCUMENT_MULTI = new HashSet<string>();
            MILESTONE_TASK_MULTI = new HashSet<string>();
            NONE_MULTI = new HashSet<string>();
            POST_CLOSING_CONDITION_MULTI = new HashSet<string>();
            UNDERWRITING_MULTI = new HashSet<string>();

            BORROWER_EMPLOYERS_MULTI_KEYS = new HashSet<string>();
            CO_BORROWER_EMPLOYERS_MULTI_KEYS = new HashSet<string>();
            BORROWER_RESIDENCES_MULTI_KEYS = new HashSet<string>();
            CO_BORROWER_RESIDENCES_MULTI_KEYS = new HashSet<string>();
            LIABILITIES_MULTI_KEYS = new HashSet<string>();
            DEPOSITS_MULTI_KEYS = new HashSet<string>();
            MORTGAGES_MULTI_KEYS = new HashSet<string>();
            VESTING_MULTI_KEYS = new HashSet<string>();

            INDEX_MULTI_SORTER = new Dictionary<string, ISet<string>>
            {
                {BORROWER_EMPLOYERS_STARTS, BORROWER_EMPLOYERS_MULTI_KEYS},
                {CO_BORROWER_EMPLOYERS_STARTS, CO_BORROWER_EMPLOYERS_MULTI_KEYS},
                {BORROWER_RESIDENCES_STARTS, BORROWER_RESIDENCES_MULTI_KEYS},
                {CO_BORROWER_RESIDENCES_STARTS, CO_BORROWER_RESIDENCES_MULTI_KEYS},
                {FD_DEPOSITS_STARTS, DEPOSITS_MULTI_KEYS},
                {DD_DEPOSITS_STARTS, DEPOSITS_MULTI_KEYS},
                {FL_LIABILITIES_STARTS, LIABILITIES_MULTI_KEYS},
                {MORTGAGES_STARTS, MORTGAGES_MULTI_KEYS},
                {IRS_MORTGAGE_INFO_STARTS, MORTGAGES_MULTI_KEYS},
                {VESTING_PARITIES_STARTS, VESTING_MULTI_KEYS}
            };

            DISCLOSURES_MULTI_KEYS = new HashSet<string>();

            INDEX_MULTI_SORTER_END = new Dictionary<string, ISet<string>> {{DISCLOSURES_STARTS, DISCLOSURES_MULTI_KEYS}};

            ROLE_MULTI_KEYS = GetRoleMultiKeys();
            MILESTONE_MULTI_KEYS = GetMilestoneMultiKeys();

            UNKNOWN_KEYS = new HashSet<string>();

            GetAllFieldIds();
        }

        public static Session session = null;


        /// <summary>
        ///  Double-check locking to ensure the singleton is only created once.
        ///  Note the reporter is also volatile which is requried to make the double-check correct.
        /// </summary>
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
                                throw new Exception("Please assign session object to FieldUtils.session.");
                            }
                            encompassFields = new FieldUtils();
                        }
                    }
                }
                return encompassFields;
            }
        }

        private ISet<string> GetRoleMultiKeys()
        {
            ISet<string> keys = new HashSet<string>();
            foreach (Role role in session.Loans.Roles)
            {
                keys.Add(role.Name);
            }
            return keys;
        }

        private ISet<string> GetMilestoneMultiKeys()
        {
            ISet<string> keys = new HashSet<string>();
            foreach (Milestone m in session.Loans.Milestones)
            {
                keys.Add(m.Name);
            }
            return keys;
        }

        private void GetAllFieldIds()
        {
            ICollection<FieldDescriptor> fieldDescriptorsList = FieldUtils.SELECTED_FIELDS;
            if (fieldDescriptorsList == null)
            {
                fieldDescriptorsList = GetAllFieldDescriptors();
            }
            LoadFieldIdsFromFieldDescriptors(fieldDescriptorsList);
        }

        private static ISet<FieldDescriptor> AddFieldDescriptors(FieldDescriptors fieldCollection,
            ISet<FieldDescriptor> fieldList)
        {
            foreach (FieldDescriptor field in fieldCollection)
            {
                fieldList.Add(field);
            }
            return fieldList;
        }

        private ISet<FieldDescriptor> GetAllFieldDescriptors()
        {
            ISet<FieldDescriptor> fieldList = new HashSet<FieldDescriptor>();
            FieldUtils.AddFieldDescriptors(FieldUtils.session.Loans.FieldDescriptors.StandardFields, fieldList);
            FieldUtils.AddFieldDescriptors(FieldUtils.session.Loans.FieldDescriptors.CustomFields, fieldList);
            FieldUtils.AddFieldDescriptors(FieldUtils.session.Loans.FieldDescriptors.VirtualFields, fieldList);
            return fieldList;
        }

        /// <summary>
        /// Depending on the field type it is sometimes possible to know the field ids at the session
        /// level, sometimes the specific fieldIds depend on the loan file itself.
        ///
        /// Multi-value fieldIds whose indexes are defined at the session level will be unrolled into multiple
        /// simple value fields.
        ///
        /// Multi-value fieldIds whose indexes are defined at the loan level will be seperated into specific
        /// lists so that they can be handled on a loan-by-loan basis.
        /// </summary>
        /// <param name="fieldDescriptors"></param>
        private void LoadFieldIdsFromFieldDescriptors(ICollection<FieldDescriptor> fieldDescriptors)
        {
            foreach (FieldDescriptor fieldDescriptor in fieldDescriptors)
            {
                if (string.IsNullOrEmpty(fieldDescriptor.FieldID)) continue;

                var baseKey = fieldDescriptor.FieldID.Trim();

                if (!fieldDescriptor.MultiInstance)
                {
                    SIMPLE_FIELDS.Add(baseKey);
                }
                else
                {
                    switch (fieldDescriptor.InstanceSpecifierType)
                    {
                        case MultiInstanceSpecifierType.Index:
                            string starts2 = baseKey.Substring(0, 2);
                            string starts3 = baseKey.Substring(0, 3);
                            /**
                             * These 3 groups do not follow any known rules for extraction
                             * have no descriptions, and seem to consist of dupe data.
                             * 
                             * Ignore them
                             */
                            if (starts2 != "LP" && starts3 != "FBE" && starts3 != "FCE")
                            {
                                //the < 30 is to skip DISCLOSEDGFE.Snapshot.NEWHUD.X100...
                                if (baseKey.Contains("00") && baseKey.Length < 30)
                                {
                                    string key = baseKey.Substring(0, 2);
                                    if (INDEX_MULTI_SORTER.Keys.Contains(key))
                                    {
                                        INDEX_MULTI_SORTER[key].Add(baseKey);
                                    }
                                    else
                                    {
                                        UNKNOWN_KEYS.Add(key);
                                        if (!MIDDLE_INDEX_EXCLUDE_LIST.Contains(baseKey))
                                        {
                                            MIDDLE_INDEXED.Add(baseKey);
                                        }
                                    }
                                }
                                else
                                {
                                    bool matched = false;
                                    foreach (string key in INDEX_MULTI_SORTER_END.Keys)
                                    {
                                        if (baseKey.StartsWith(key))
                                        {
                                            INDEX_MULTI_SORTER_END[key].Add(baseKey);
                                            matched = true;
                                            break;
                                        }
                                    }
                                    if (!matched && !(END_INDEX_EXCLUDE_LIST.Contains(baseKey)))
                                    {
                                        END_INDEXED.Add(baseKey);
                                    }
                                }
                            }
                            break;
                        case MultiInstanceSpecifierType.Document:
                            DOCUMENT_MULTI.Add(baseKey);
                            break;
                        case MultiInstanceSpecifierType.Milestone:
                            UnrollMultiFieldIds(baseKey, MILESTONE_MULTI_KEYS);
                            break;
                        case MultiInstanceSpecifierType.MilestoneTask:
                            MILESTONE_TASK_MULTI.Add(baseKey);
                            break;
                        case MultiInstanceSpecifierType.PostClosingCondition:
                            POST_CLOSING_CONDITION_MULTI.Add(baseKey);
                            break;
                        case MultiInstanceSpecifierType.Role:
                            UnrollMultiFieldIds(baseKey, ROLE_MULTI_KEYS);
                            break;
                        case MultiInstanceSpecifierType.UnderwritingCondition:
                            UNDERWRITING_MULTI.Add(baseKey);
                            break;
                        default:
                            break;
                    }
                }
            }

            /*
            Console.WriteLine("UNKNOWN_KEYS\n------------");
            foreach (string key in UNKNOWN_KEYS)
            {
                Console.WriteLine(key);
            }
            */
        }

        private void UnrollMultiFieldIds(string fieldId, ISet<string> keys)
        {
            foreach (string key in keys)
            {
                SIMPLE_FIELDS.Add(fieldId + "." + key);
            }
        }

        /**
         * These method are expected to be called repeatedly, so the results are cached.
         */

        public static ISet<string> SimpleFieldNames()
        {
            return Instance.SIMPLE_FIELDS;
        }

        public static ISet<string> MiddleIndexMulti()
        {
            return Instance.MIDDLE_INDEXED;
        }

        public static ISet<string> EndIndexMulti()
        {
            return Instance.END_INDEXED;
        }

        public static ISet<string> DocumentMulti()
        {
            return Instance.DOCUMENT_MULTI;
        }

        public static ISet<string> MilestoneTaskMulti()
        {
            return Instance.MILESTONE_TASK_MULTI;
        }

        public static ISet<string> PostClosingMulti()
        {
            return Instance.POST_CLOSING_CONDITION_MULTI;
        }

        public static ISet<string> RoleMultiKeys()
        {
            return Instance.ROLE_MULTI_KEYS;
        }

        public static ISet<string> UnderwritingMulti()
        {
            return Instance.UNDERWRITING_MULTI;
        }

        public static ISet<string> BorrowerEmployers()
        {
            return Instance.BORROWER_EMPLOYERS_MULTI_KEYS;
        }

        public static ISet<string> CoBorrowerEmployers()
        {
            return Instance.CO_BORROWER_EMPLOYERS_MULTI_KEYS;
        }

        public static ISet<string> BorrowerResidences()
        {
            return Instance.BORROWER_RESIDENCES_MULTI_KEYS;
        }

        public static ISet<string> CoBorrowerResidences()
        {
            return Instance.CO_BORROWER_RESIDENCES_MULTI_KEYS;
        }

        public static ISet<string> LiabilitiesMulti()
        {
            return Instance.LIABILITIES_MULTI_KEYS;
        }

        public static ISet<string> DepostisMulti()
        {
            return Instance.DEPOSITS_MULTI_KEYS;
        }

        public static ISet<string> MortgagesMulti()
        {
            return Instance.MORTGAGES_MULTI_KEYS;
        }

        public static ISet<string> VestingPartiesMulti()
        {
            return Instance.VESTING_MULTI_KEYS;
        }

        public static ISet<string> DisclosureMulti()
        {
            return Instance.DISCLOSURES_MULTI_KEYS;
        }

        /**
         * This method is not expected to be called in most cases, and if it is called, won't be called repeatedly.
         * So not caching results.
         */

        public static IDictionary<string, string> GetFieldsAndDescriptions()
        {
            IDictionary<string, string> fieldsAndDescriptions = new Dictionary<string, string>();
            ISet<FieldDescriptor> fieldDescriptorsList = Instance.GetAllFieldDescriptors();

            foreach (var fieldDescriptor in fieldDescriptorsList)
            {
                if (!fieldsAndDescriptions.ContainsKey(fieldDescriptor.FieldID))
                {
                    fieldsAndDescriptions.Add(fieldDescriptor.FieldID, fieldDescriptor.Description);
                }
                else
                {
                    Logger.Error("FieldUtils", $"Duplicate field Ids ({fieldDescriptor.FieldID}) found in GetAllFieldDescriptors. "+ 
                        $"Existing value: {fieldsAndDescriptions[fieldDescriptor.FieldID]}, New value: {fieldDescriptor.Description}");
                }
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
