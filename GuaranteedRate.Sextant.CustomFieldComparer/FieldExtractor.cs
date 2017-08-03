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
        private readonly IDictionary<string, IList<IReportingFieldDescriptor>> _reportableFields;
        private readonly IDictionary<string, string> _fullExport; 

        public FieldExtractor(Session session, string baseFilePath, bool onlyCustomFields, string environment)
        {
            EncompassUtils.FieldUtils.session = session;
            _session = session;
            _fullFile = baseFilePath + environment + ".json";
            _environmentPath = baseFilePath + environment + "\\";
            _includeAll = !onlyCustomFields;
            _fullExport = new Dictionary<string, string>();
            _reportableFields = new Dictionary<string, IList<IReportingFieldDescriptor>>();

            InitReportableFields();

        }

        public void Extract()
        {
            if (_includeAll)
            {
                Extract(_session.Loans.FieldDescriptors.StandardFields, _environmentPath);
                Extract(_session.Loans.FieldDescriptors.VirtualFields, _environmentPath);
            }
            Extract(_session.Loans.FieldDescriptors.CustomFields, _environmentPath);

            var fullMonty = BuildGiantJson(_fullExport);

            WriteFieldToFile(_fullFile, fullMonty);
        }

        /// <summary>
        /// Content is a dictionary of fieldIds and json serializations of
        /// field descriptions.
        /// 
        /// Rather than double serialize, or deal with double escaping, this
        /// function is manually converting the descriptions into a json array
        /// of descriptions.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>A json array of field description maps, about 20megs</returns>
        private string BuildGiantJson(IDictionary<string, string> content)
        {
            var keys = content.Keys.ToList();
            keys.Sort();
            StringBuilder manualJson = new StringBuilder();
            manualJson.AppendLine("[");
            foreach (var key in keys)
            {
                manualJson.Append(content[key]).AppendLine(",");
            }
            manualJson.AppendLine("]");
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
         
        private void InitReportableFields()
        {
            ReportingFieldDescriptorList reportableFields = _session.Reports.GetReportingDatabaseFields();
            foreach (ReportingFieldDescriptor field in reportableFields)
            {
                IList<IReportingFieldDescriptor> fields;
                if (_reportableFields.ContainsKey(field.FieldID))
                {
                    fields = _reportableFields[field.FieldID];
                }
                else
                {
                    fields = new List<IReportingFieldDescriptor>();
                    _reportableFields.Add(field.FieldID, fields);
                }
                fields.Add(field);
            }
        }

        private void Extract(FieldDescriptors fieldCollection, string outPath)
        {
            foreach (FieldDescriptor field in fieldCollection)
            {
                var filePath = outPath + field.FieldID + ".json";
                IList<IReportingFieldDescriptor> reportingDesc = null;
                if (_reportableFields.ContainsKey(field.FieldID))
                {
                    reportingDesc = _reportableFields[field.FieldID];
                }
                var fullField = MergeFieldDescriptions(field, reportingDesc);
                var fieldDesc = ExpandField(fullField);
                _fullExport[field.FieldID] = fieldDesc;
                WriteFieldToFile(filePath, fieldDesc);
            }
        }

        private FullFieldDescription MergeFieldDescriptions(FieldDescriptor field, IList<IReportingFieldDescriptor> reportingFields)
        {
            FullFieldDescription fullField = new FullFieldDescription();

            fullField.fieldDescriptor = field;
            fullField.reportingFields = reportingFields;
            return fullField;
        }

        private string ExpandField(FullFieldDescription fullField)
        {
            return JsonConvert.SerializeObject(fullField, Formatting.Indented);
        }
   }
}
