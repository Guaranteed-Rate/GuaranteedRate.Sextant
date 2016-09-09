using EllieMae.Encompass.Client;
using GuaranteedRate.Sextant.EncompassUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Examples.FieldUtils
{
    public class FieldDescriptions
    {
        private readonly string outputFileName;

        static int Main(string[] args)
        {
            if (args != null && args.Length == 4)
            {
                FieldDescriptions fieldDescriptions = new FieldDescriptions(args[0]);
                Session session = SessionUtils.GetEncompassSession(args[1], args[2], args[3]);
                //fieldDescriptions.WriteReportableVirtual(session);
                fieldDescriptions.WriteFieldsAndDescriptionsToFile(session);
                return 1;
            }
            else
            {
                Console.WriteLine("Usage [output file name] [Encompass url] [Encompass User] [Encompass Password]");
                return 0;
            }
        }

        public FieldDescriptions(string outputFileName)
        {
            this.outputFileName = outputFileName;
        }

        public void WriteFieldsAndDescriptionsToFile(Session session)
        {
            GuaranteedRate.Sextant.EncompassUtils.FieldUtils.session = session;
            IDictionary<string, string> fieldsAndDescriptions =
                GuaranteedRate.Sextant.EncompassUtils.FieldUtils.GetFieldsAndDescriptions();
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(outputFileName, true))
            {
                foreach (string fieldId in fieldsAndDescriptions.Keys)
                {
                    file.WriteLine("\"" + fieldId + "\",\"" + fieldsAndDescriptions[fieldId].Replace("\"", "") + "\"");
                }
            }
        }

        public void WriteReportableVirtual(Session session)
        {
            GuaranteedRate.Sextant.EncompassUtils.FieldUtils.session = session;
            ISet<string> fields = GuaranteedRate.Sextant.EncompassUtils.FieldUtils.ReportableVirtualFields();
            Console.WriteLine();
            foreach (string fieldId in fields)
            {
                Console.Write(fieldId + ", ");
            }
        }
    }
}