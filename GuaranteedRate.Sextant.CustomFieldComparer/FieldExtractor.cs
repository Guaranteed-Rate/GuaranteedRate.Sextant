using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.CustomFieldComparer
{
    public class FieldExtractor
    {
        private readonly Session _session;
        private readonly string _filePath;
        private readonly bool _includeAll;

        public FieldExtractor(Session session, string baseFilePath, bool onlyCustomFields)
        {
            EncompassUtils.FieldUtils.session = session;
            _session = session;
            _filePath = baseFilePath;
            _includeAll = !onlyCustomFields;
        }

        public void Extract()
        {
            if (_includeAll)
            {
                Extract(_session.Loans.FieldDescriptors.StandardFields, _filePath);
                Extract(_session.Loans.FieldDescriptors.VirtualFields, _filePath);
            }
            Extract(_session.Loans.FieldDescriptors.CustomFields, _filePath);
        }

        private static bool WriteFieldToFile(string filePath, string content)
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

        private void Extract(FieldDescriptors fieldCollection, string outPath)
        {
            foreach (FieldDescriptor field in fieldCollection)
            {
                var filePath = outPath + field.FieldID + ".json";
                var fieldDesc = ExpandField(field);
                WriteFieldToFile(filePath, fieldDesc);
            }
        }

        private string ExpandField(FieldDescriptor field)
        {
            return JsonConvert.SerializeObject(field, Formatting.Indented);
        }
    }
}
