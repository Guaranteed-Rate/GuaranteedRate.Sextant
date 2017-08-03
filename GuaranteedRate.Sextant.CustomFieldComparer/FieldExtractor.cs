using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Reporting;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.CustomFieldComparer
{
    public class FieldExtractor
    {
        private readonly Session _session;
        private readonly string _fullFile;
        private readonly string _environmentPath;
        private readonly bool _includeAll;
        private readonly IDictionary<string, string> _fullExport; 

        public FieldExtractor(Session session, string baseFilePath, bool onlyCustomFields, string environment)
        {
            EncompassUtils.FieldUtils.session = session;
            _session = session;
            _fullFile = baseFilePath + environment + ".json";
            _environmentPath = baseFilePath + environment + "\\";
            _includeAll = !onlyCustomFields;
            _fullExport = new Dictionary<string, string>();
        }

        public void Extract()
        {
            if (_includeAll)
            {
                Extract(_session.Loans.FieldDescriptors.StandardFields, _environmentPath);
                Extract(_session.Loans.FieldDescriptors.VirtualFields, _environmentPath);
            }
            Extract(_session.Loans.FieldDescriptors.CustomFields, _environmentPath);
            ExtractReporting(_environmentPath);

            var fullMonty = MergeAndSort(_fullExport);

            WriteFieldToFile(_fullFile, fullMonty);
        }

        /// <summary>
        /// Transform the master content dictionary into a sorted list of
        /// field descriptions, then transform the list into a single giant
        /// json array of field description maps.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string MergeAndSort(IDictionary<string, string> content)
        {
            IList<string> sortedContent = new List<string>();
            var keys = content.Keys.ToList();
            keys.Sort();
            StringBuilder manualJson = new StringBuilder();
            manualJson.Append("[\n");
            foreach (var key in keys)
            {
                //sortedContent.Add(content[key]); 
                manualJson.Append("\t" + content[key] + ",\n");
            }
            manualJson.Append("]\n");
            return manualJson.ToString();
        }

        private bool WriteFieldToFile(string filePath, string content)
        {
            if (System.IO.File.Exists(filePath))
            {
                if (System.IO.File.ReadAllText(filePath) == content)
                {
                    return false;
                }
                System.IO.File.Delete(filePath);
            }
            using (var sw = System.IO.File.CreateText(filePath))
            {
                sw.Write(content);
                sw.Flush();
            }
            return true;
        }

        private void ExtractReporting(string outPath)
        {
            ReportingFieldDescriptorList reportableFields = _session.Reports.GetReportingDatabaseFields();
            foreach (ReportingFieldDescriptor field in reportableFields)
            {
                var filePath = outPath + "REPORTING-" + field.FieldID + ".json";
                var fieldDesc = ExpandField(field);
                _fullExport["REPORTING-" +  field.FieldID] = fieldDesc;
                WriteFieldToFile(filePath, fieldDesc);
            }
        }

        private void Extract(FieldDescriptors fieldCollection, string outPath)
        {
            foreach (FieldDescriptor field in fieldCollection)
            {
                var filePath = outPath + field.FieldID + ".json";
                var fieldDesc = ExpandField(field);
                _fullExport[field.FieldID] = fieldDesc;
                WriteFieldToFile(filePath, fieldDesc);
            }
        }

        private string ExpandField(FieldDescriptor field)
        {
            return JsonConvert.SerializeObject(field, Formatting.Indented);
        }

        private string ExpandField(ReportingFieldDescriptor field)
        {
            return JsonConvert.SerializeObject(field, Formatting.Indented);
        }
    }
}
